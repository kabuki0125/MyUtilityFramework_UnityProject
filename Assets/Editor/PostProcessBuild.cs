using UnityEngine;
using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.XCodeEditor;
using MiniJSON;

/// <summary>
/// Unityのビルド後に実行する処理（XCodeのプロジェクト設定など）.
/// </summary>
public static class PostProcessBuild
{
	/// <summary>
	/// post process build event.
	/// </summary>
	/// <param name="target">Target（iPhone/Androidなど）</param>
	/// <param name="path">出力先のパス（ビルド時に指定するパス）</param>
	[PostProcessBuild (100)]
	public static void OnPostProcessBuild (BuildTarget target, string path)
	{
		if (target == BuildTarget.iPhone) {
			PostProcessBuild_iOS (path);
		}
	}

	/// <summary>
	/// iOSのビルド設定自動化処理.
	/// </summary>
	/// <param name="path">出力先のパス（ビルド時に指定するパス）</param>
	private static void PostProcessBuild_iOS (string path)
	{
		// XCode プロジェクトファイルの設定をする.
		CreateModFile( path );
		ProcessXCodeProject( path );
		UpdatePlist( path );
	}
	// すべての設定の直接の親であるdictエレメントを取得
	private static XmlNode FindPlistDictNode( XmlDocument doc )
	{
		var cur = doc.FirstChild;
		while ( cur != null )
		{
			if( cur.Name.Equals ( "plist" ) && cur.ChildNodes.Count == 1 )
			{
				var dict = cur.FirstChild;
				if( dict.Name.Equals ( "dict" ) )
				{
					return dict;
				}
			}
			cur = cur.NextSibling;
		}
		return null;
	}
	
	// すでにそのkeyが存在しているか？
	// dict:親ノード
	private static bool HasKey( XmlNode dict, string keyName )
	{
		var cur = dict.FirstChild;
		while( cur != null )
		{
			if( cur.Name.Equals ( "key" ) && cur.InnerText.Equals ( keyName ) )
			{
				return true;
			}
			cur = cur.NextSibling;
		}
		return false;
	}
	
	// 子エレメントを追加
	// elementName:<...>の<>の中の文字列
	// innerText:<key>...</key>のタグで囲まれた文字列
	private static XmlElement AddChildElement( XmlDocument doc, XmlNode parent, string elementName, string innerText = null )
	{
		var newElement = doc.CreateElement ( elementName );
		if( !string.IsNullOrEmpty ( innerText ) )
		{
			newElement.InnerText = innerText;
		}
		parent.AppendChild ( newElement );
		return newElement;
	}
	
	// 指定したkeyに対応する値を更新する
	// <key>KEY_TEXT</key>
	// <ELEMENT_NAME>VALUE</ELEMENT_NAME>
	// 以上の構造の場合のみ正常に動作
	// key:KEY_TEXT
	// elementName:ELEMENT_NAME
    // value:VALUE
	private static XmlNode UpdateKeyValue( XmlNode node, string key, string elementName, string value )
	{
		// まず<key>...</key>のノードを取得
		var keyNode = GetChildElement ( node, "key", key );
		if( keyNode.NextSibling != null && keyNode.NextSibling.Name.Equals ( elementName ) )
		{
			// 取得したkeyノードの次のノードのelementNameが指定された文字列だった場合、値を更新する
			keyNode.NextSibling.InnerText = value;
			return keyNode;
		}
		return null;
	}
	
	// 子エレメントを取得
	// elementName:<...>の<>の中の文字列
	// innerText:<key>...</key>のタグで囲まれた文字列
	private static XmlNode GetChildElement( XmlNode node, string elementName, string innerText=null )
	{
		var cur = node.FirstChild;
		while ( cur != null )
		{
			if( cur.Name.Equals ( elementName ) )
			{
				if( ( innerText == null && cur.InnerText == null ) || ( innerText != null && cur.InnerText.Equals ( innerText ) ) )
				{
					return cur;
				}
			}
			cur = cur.NextSibling;
		}
		return null;
	}
	
	// info.plistのあるディレクトリパスと設定値を受け取り、info.plistに設定を登録する
	public static void UpdatePlist( string path )
	{
		// info.plistを読み込む
		string fullPath = Path.Combine ( path, "info.plist" );
		var doc = new XmlDocument();
		doc.Load ( fullPath );
 
		// すべての設定の直接の親であるdictエレメントを取得する
		var dict = FindPlistDictNode ( doc );
		if( dict == null )
		{
			Debug.LogError ( "Error plist:" + fullPath );
			return;
		}
 
		// 1. 設定値
		// 登録後の例
		// <key>sample_key</key>
        // <string>val</string>

		// ローカライズ設定
		if( !HasKey ( dict, "CFBundleDevelopmentRegion" ) )
		{
			AddChildElement ( doc, dict, "key", "CFBundleDevelopmentRegion" );
			AddChildElement ( doc, dict, "string", "ja_JP" );
		}else
		{
			UpdateKeyValue ( dict, "CFBundleDevelopmentRegion", "string", "ja_JP" );
		}

		// URLスキーマの設定
		XmlNode urlSchemeTop = null;
		if (!HasKey (dict, "CFBundleURLTypes")) {
			AddChildElement (doc, dict, "key", "CFBundleURLTypes");
			urlSchemeTop = AddChildElement (doc, dict, "array");
		} else {
			//すでにkey:CFBundleURLTypesが存在している
			//key:CFBundleURLTypesを取得
			var urlScheme = GetChildElement (dict, "key", "CFBundleURLTypes");
			urlSchemeTop = urlScheme.NextSibling;
		}
		//存在確認・更新
		bool isExist = false;
		foreach (XmlNode urlDict in urlSchemeTop.ChildNodes) {
			if (urlDict.Name.Equals ("dict") && urlDict.HasChildNodes) {
				//子がdict構造であり、更に子を持っている
				var urlUrlName = GetChildElement (urlDict, "key", "CFBundleURLName");
				if (urlUrlName != null && urlUrlName.NextSibling != null) {
					//key:CFBundleURLNameの要素があり、その次の要素も存在する
					var urlUrlString = urlUrlName.NextSibling;
					if (urlUrlString.Name.Equals ("string") &&
					    urlUrlString.InnerText.Equals (PlayerSettings.bundleIdentifier)) {
						//同じBundleIDの設定が見つかった
						isExist = true;
						//設定の上書き
						urlUrlString.InnerText = PlayerSettings.bundleIdentifier;
						break;
					}
				}
			}
		}
		if (!isExist) {
#if false	//追加したいURLスキーマがあれば.
			//存在していない場合のみ追加
			var urlSchemeDict = AddChildElement (doc, urlSchemeTop, "dict");
			//設定内容追加.状況に応じ複数追加可能.
			AddChildElement (doc, urlSchemeDict, "key", "CFBundleURLName");
			AddChildElement (doc, urlSchemeDict, "string", ""/*PlayerSettings.bundleIdentifier*/);
			AddChildElement (doc, urlSchemeDict, "key", "CFBundleURLSchemes");
			var innerArray = AddChildElement (doc, urlSchemeDict, "array");
            AddChildElement (doc, innerArray, "string", "");
#endif
        }

	    // 保存
	    doc.Save( fullPath );
	    
	    // <!DOCTYPE　の行を書き換えて保存してしまうため、修正する
	    string textPlist = string.Empty;
	    using ( var reader = new StreamReader ( fullPath ) )
		{
			textPlist = reader.ReadToEnd (  );
		}
		
		// 本来の行が存在していれば処理終了
		int fixupStart = textPlist.IndexOf ( "<!DOCTYPE plist PUBLIC", System.StringComparison.Ordinal );
		if( fixupStart <= 0 )
		{
			return;
		}
		int fixupEnd = textPlist.IndexOf ( '>', fixupStart );
		if( fixupEnd <= 0 )
		{
			return;
		}
		
		// 修正処理
		string fixedPlist = textPlist.Substring ( 0, fixupStart );
		fixedPlist += "<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">";
		fixedPlist += textPlist.Substring ( fixupEnd+1 );
		
		using ( var writer = new StreamWriter ( fullPath, false ) )
		{
			writer.Write ( fixedPlist );
		}
	}

	/// <summary>
	/// Mod ファイルの作成.
	/// </summary>
	/// <param name="path">出力先のパス（ビルド時に指定するパス）</param>
	private static void CreateModFile (string path)
	{
		Dictionary<string, object> mod = new Dictionary<string, object> ();

		List<string> patches = new List<string> ();
		List<string> librarysearchpaths = new List<string> ();
		List<string> folders = new List<string> ();
		List<string> excludes = new List<string> ();

		// libs
		List<string> libs = new List<string> ();

		// フレームワークサーチパス
		List<string> frameworksearchpaths = new List<string> ();

		// フレームワーク(Systemディレクトリ以下に配置してあるSDK付随のフレームワークを追加する際はこちらから.)
		List<string> frameworks = new List<string> ();
		frameworks.Add( "AdSupport.framework:weak" );
		frameworks.Add( "Security.framework" );
		
		// files(カスタムフレームワーク、その他ファイルを追加する場合はこちらから.)
		List<string> files = new List<string> ();

		// headerpaths
		List<string> headerpaths = new List<string> ();

		// ビルド設定
		Dictionary<string,List<string>> buildSettings = new Dictionary<string,List<string>> ();
		
		List<string> debugInformationFormat = new List<string> ();
		debugInformationFormat.Add ("dwarf");
		buildSettings.Add ("DEBUG_INFORMATION_FORMAT", debugInformationFormat);

		mod.Add ("group", "");
		mod.Add ("patches", patches);
		mod.Add ("libs", libs);
		mod.Add ("librarysearchpaths", librarysearchpaths);
		mod.Add ("frameworksearchpaths", frameworksearchpaths);
		mod.Add ("frameworks", frameworks);
		mod.Add ("headerpaths", headerpaths);
		mod.Add ("files", files);
		mod.Add ("folders", folders);
		mod.Add ("excludes", excludes);
		mod.Add ("buildSettings", buildSettings);

		// mod から projmods ファイルを生成する
		string jsonMod = Json.Serialize (mod);

		string file = System.IO.Path.Combine (path, "CustomXCode.projmods");

		if (File.Exists (file)) {
			File.Delete (file);
		}

		using (StreamWriter streamWriter = File.CreateText (file)) {
			streamWriter.Write (jsonMod);
		}
	}

	/// <summary>
	/// projmods ファイルの設定値を XCode プロジェクト設定へ反映する.
	/// </summary>
	/// <param name="path">出力先のパス（ビルド時に指定するパス）</param>
	private static void ProcessXCodeProject (string path)
	{
		XCProject project = new XCProject (path);

		string[] files = System.IO.Directory.GetFiles (path, "*.projmods", System.IO.SearchOption.AllDirectories);

		foreach (string file in files) {
			project.ApplyMod ( System.IO.Path.Combine (Application.dataPath, file));
		}
		// 上書きビルド設定
		project.overwriteBuildSetting("ENABLE_BITCODE", "NO");	// Xcode7.0から実装されたBitCode対応しているかどうかの設定.デフォルトでYESになっている.
		project.overwriteBuildSetting("CODE_SIGN_RESOURCE_RULES_PATH", "$(SDKROOT)/ResourceRules.plist");	// XCode6.1 のアップデートでいろいろ動かなくなった所の一つ対処用.

		project.Save ();
	}
}