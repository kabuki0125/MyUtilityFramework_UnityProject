#if UNITY_5
#define UNITY_5_AND_LATER
#endif

using UnityEngine;
using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.XCodeEditor;
using UnityEditor.iOS.Xcode;


/// <summary>
/// Unityのビルド後に実行する処理（XCodeのプロジェクト設定など）
/// </summary>
public static class LobiPostProcessor
{
	private const string URL_SCHEME_PREFIX_LOBI = "nakamapapp-";

	/// <summary>
	/// Raises the post process build event.
	/// </summary>
	/// <param name="target">Target.</param>
	/// <param name="path">出力先のパス（ビルド時に指定するパス）</param>
	[PostProcessBuild (100)]
	public static void OnPostProcessBuild (BuildTarget target, string path)
	{
		LobiSettings settings = (LobiSettings)Resources.Load (LobiSettingsEditor.settingsFile);

		if (settings == null) {
			Debug.Log ("Lobi SDK auto build settings will be disabled because client id or base name was not valid.\nLobi SDK Settings: [Edit]-[Lobi Settings]");
			return;
		}

		if (settings == null || settings.IsEnabled == false) {
			return;
		}

		if (settings.IsValid) {
			#if UNITY_5_AND_LATER
            if (target == BuildTarget.iOS) {
            #else
            if (target == BuildTarget.iPhone) {
            #endif
				PostProcessBuild_iOS (path, settings);
			} else if (target == BuildTarget.Android) {
				// TODO: Android
				//PostProcessBuild_Android(path, settings.clientId);
			}
		} else {
			Debug.LogError ("Lobi will be disabled because client id or base name was not valid.");
		}
	}

	/// <summary>
	/// iOSのビルド設定自動化処理
	/// </summary>
	/// <param name="path">出力先のパス（ビルド時に指定するパス）</param>
	/// <param name="settings">Lobiの設定</param>
	private static void PostProcessBuild_iOS (string path, LobiSettings settings)
	{
		// ObjC ファイルを書き換える
		ReWriteObjCFiles (path, settings);

		// XCode プロジェクトファイルの設定をする
		CreateModFile (path, settings);
		ProcessXCodeProject (path);

		// Info.plist ファイルを書き換える
		ReWriteInfoPList (path, settings.clientId);
	}

	/// <summary>
	/// ObjC ファイルを書き換える
	/// </summary>
	/// <param name="path">出力先のパス（ビルド時に指定するパス）</param>
	/// <param name="settings">Lobiの設定</param>
	private static void ReWriteObjCFiles (string path, LobiSettings settings)
	{
		// LobiUnityAppController.mm ファイルを書き換える
		string filePath = string.Empty;
		#if UNITY_5_AND_LATER
		filePath = System.IO.Path.Combine (path, "Libraries/Plugins/iOS/LobiUnityAppController.mm");
		#else
		filePath = System.IO.Path.Combine (path, "Libraries/LobiUnityAppController.mm");
		#endif
		if (File.Exists (filePath)) {
			UpdateStringInFile (filePath, "[LobiCore setupClientId:@\"\"", "[LobiCore setupClientId:@\"" + settings.clientId + "\"");
			UpdateStringInFile (filePath, "accountBaseName:@\"\"];", "accountBaseName:@\"" + settings.baseName + "\"];");

			if (settings.chatEnabled == false) {
				UpdateStringInFile (filePath, "#define LOBI_CHAT", "");
			}

			if (settings.recEnabled == false) {
				UpdateStringInFile (filePath, "#define LOBI_REC", "");
			}
		}	

		if (settings.recEnabled == true) {
			ReWriteMetalFiles(path);
		}
	}

	/// <summary>
	/// Metal 関連のファイルを書き換える
	/// </summary>
	/// <param name="path">出力先のパス（ビルド時に指定するパス）</param>
	private static void ReWriteMetalFiles (string path)
	{
		// Rewrite to MetalHelper.mm
		string helperFilePath = System.IO.Path.Combine(path, "Classes/Unity/MetalHelper.mm");
		if (File.Exists(helperFilePath)) {
			// ファイル内に`Lobi`の文字が見つからない == updateされていない場合に内容を書き換える
			if (!HasStringInFile(helperFilePath, "Lobi")) {
				UpdateStringInFile(helperFilePath, "#include \"CVTextureCache.h\"", ("#include \"CVTextureCache.h\"\n" +
					"#import <LobiRec/LobiRec.h>\n" +
					"#import <LobiRec/LobiRec+Metal.h>\n"
				));
				UpdateStringInFile(helperFilePath, "surface->systemColorRB	= [surface->drawable texture];", "surface->systemColorRB	= [surface->drawable texture];\n[LobiRec setCurrentDrawable:surface->drawable];\n");

				#if UNITY_5_0 || UNITY_4_6 || UNITY_4_7
				UpdateStringInFile (helperFilePath, "[commandBuffer commit];", "[LobiRec addMetalCommands:commandBuffer];\n[commandBuffer commit];");
				#endif
			}
		}

		#if UNITY_5_AND_LATER && !UNITY_5_0
		string renderingFilePath = System.IO.Path.Combine(path, "Classes/UnityAppController+Rendering.mm");
		if (File.Exists(renderingFilePath)) {
			// ファイル内に`Lobi`の文字が見つからない == updateされていない場合に内容を書き換える
			if (!HasStringInFile(renderingFilePath, "Lobi")) {
				UpdateStringInFile (renderingFilePath, "#include \"UI/UnityView.h\"", ("#include \"UI/UnityView.h\"\n" + 
					"#import <LobiRec/LobiRec.h>\n" +
					"#import <LobiRec/LobiRec+Metal.h>\n"
				));
				UpdateStringInFile (renderingFilePath, "UnityPlayerLoop();", "UnityPlayerLoop();\n[LobiRec addMetalCommands:UnityCurrentMTLCommandBuffer()];");
			}
		}
		#endif
	}

	/// <summary>
	/// Mod ファイルの作成
	/// </summary>
	/// <param name="path">出力先のパス（ビルド時に指定するパス）</param>
	/// <param name="settings">Lobiの設定</param>
	private static void CreateModFile (string path, LobiSettings settings)
	{
		Dictionary<string, object> mod = new Dictionary<string, object> ();
		
		List<string> patches = new List<string> ();
		List<string> librarysearchpaths = new List<string> ();
		List<string> folders = new List<string> ();
		List<string> excludes = new List<string> ();

		// libs
		List<string> libs = new List<string> ();
		libs.Add ("libsqlite3.0.dylib");

		// フレームワークサーチパス
		List<string> frameworksearchpaths = new List<string> ();
		#if !UNITY_5_AND_LATER
		frameworksearchpaths.Add ("Plugins/Lobi/iOS");
		#endif

		// フレームワーク
		List<string> frameworks = new List<string> ();
		frameworks.Add ("OpenGLES.framework");
		frameworks.Add ("QuartzCore.framework");
		frameworks.Add ("MediaPlayer.framework");
		frameworks.Add ("MessageUI.framework");
		frameworks.Add ("CoreData.framework");
		frameworks.Add ("CoreMedia.framework");
		frameworks.Add ("Security.framework");

		frameworks.Add ("CoreImage.framework:weak");
		frameworks.Add ("StoreKit.framework:weak");
		frameworks.Add ("AVFoundation.framework:weak");
		frameworks.Add ("Foundation.framework:weak");
		frameworks.Add ("AudioToolbox.framework:weak");
		frameworks.Add ("AssetsLibrary.framework:weak");
		frameworks.Add ("UIKit.framework:weak");

		// files
		List<string> files = new List<string> ();
		#if !UNITY_5_AND_LATER
		AddFrameworkFile (ref files, "Plugins/Lobi/iOS/LobiCore.framework");
		AddFrameworkFile (ref files, "Plugins/Lobi/iOS/LobiCore.bundle");
		if (settings.chatEnabled == true) {
			AddFrameworkFile (ref files, "Plugins/Lobi/iOS/LobiChat.framework");
			AddFrameworkFile (ref files, "Plugins/Lobi/iOS/LobiChat.bundle");
		}
		if (settings.recEnabled == true) {
			AddFrameworkFile (ref files, "Plugins/Lobi/iOS/LobiRec.framework");
			AddFrameworkFile (ref files, "Plugins/Lobi/iOS/LobiRec.bundle");
		}
		if (settings.rankingEnabled == true) {
			AddFrameworkFile (ref files, "Plugins/Lobi/iOS/LobiRanking.framework");
			AddFrameworkFile (ref files, "Plugins/Lobi/iOS/LobiRanking.bundle");
		}
		#endif
		
		// headerpaths
		List<string> headerpaths = new List<string> ();
		#if !UNITY_5_AND_LATER
		headerpaths.Add ("Plugins/Lobi/iOS");
		#endif

		// ビルド設定
		Dictionary<string,List<string>> buildSettings = new Dictionary<string,List<string>> ();
		List<string> otherLinkerFlags = new List<string> ();
		otherLinkerFlags.Add ("-ObjC");
		buildSettings.Add ("OTHER_LDFLAGS", otherLinkerFlags);
		
		mod.Add ("group", "Lobi");
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
		string jsonMod = LobiMiniJSON.Json.Serialize (mod);
		
		string modPath = System.IO.Path.Combine (path, "Lobi");
		string file = System.IO.Path.Combine (modPath, "LobiXCode.projmods");
		
		if (!Directory.Exists (modPath)) {
			Directory.CreateDirectory (modPath);
		}
		if (File.Exists (file)) {
			File.Delete (file);
		}
		
		using (StreamWriter streamWriter = File.CreateText (file)) {
			streamWriter.Write (jsonMod);
		}
	}

	private static void AddFrameworkFile (ref List<string> files, String filePath)
	{
		string fileFullPath = System.IO.Path.Combine (Application.dataPath, filePath);
		if (Directory.Exists (fileFullPath)) {
			files.Add (filePath);
		} else {
			Debug.LogWarning ("Lobi SDK: file not found \"" + filePath + "\"");
		}
	}

	/// <summary>
	/// projmods ファイルの設定値を XCode プロジェクト設定へ反映する
	/// </summary>
	/// <param name="path">出力先のパス（ビルド時に指定するパス）</param>
	private static void ProcessXCodeProject (string path)
	{
		XCProject project = new XCProject (path);
		
		string modsPath = System.IO.Path.Combine (path, "Lobi");
		string[] files = System.IO.Directory.GetFiles (modsPath, "*.projmods", System.IO.SearchOption.AllDirectories);
		
		foreach (string file in files) {
			project.ApplyMod (file);
		}

		#if UNITY_5
		// To need to add a `-fno-objc-arc` flag if building in Unity5.
		string librariesPath  = System.IO.Path.Combine (path, "Libraries/Plugins/iOS");
		string[] filePathList = System.IO.Directory.GetFiles(librariesPath);
		
		PBXDictionary<PBXBuildFile> buildFiles = project.buildFiles;
		foreach (KeyValuePair<string, PBXBuildFile> kvp in buildFiles) {
			string fileName = kvp.Key;
			foreach (string targetPath in filePathList) {
				string extention = System.IO.Path.GetExtension(targetPath);
				if (!(extention == ".m" || extention == ".mm")) {
					continue;
				}

				string targetName = System.IO.Path.GetFileName(targetPath);
				if (!fileName.Contains(targetName)) {
					continue;
				}

				PBXBuildFile file = kvp.Value;
				file.AddCompilerFlag("\"-fno-objc-arc\"");
			}
		}
		#endif
		
		project.Save ();
	}

	// Info.plist を書き換える
	private static void ReWriteInfoPList (string path, string clientId)
	{
		try {
			string file = System.IO.Path.Combine (path, "Info.plist");
			
			if (!File.Exists (file)) {
				return;
			}
			
			XmlDocument xmlDocument = new XmlDocument ();
			
			xmlDocument.Load (file);
			
			XmlNode dict = xmlDocument.SelectSingleNode ("plist/dict");
			
			if (dict != null) {
				
				// Add url schemes				
				PListItem bundleUrlTypes = GetPlistItem (dict, "CFBundleURLTypes");
				
				if (bundleUrlTypes == null) {
					XmlElement key = xmlDocument.CreateElement ("key");
					key.InnerText = "CFBundleURLTypes";
					
					XmlElement array = xmlDocument.CreateElement ("array");
					
					bundleUrlTypes = new PListItem (dict.AppendChild (key), dict.AppendChild (array));
				}
				
				AddUrlScheme (xmlDocument, bundleUrlTypes.itemValueNode, URL_SCHEME_PREFIX_LOBI + clientId);

				// Add LSApplicationQueriesSchemes
				PListItem applicationQueriesSchemes = GetPlistItem (dict, "LSApplicationQueriesSchemes");

				if (applicationQueriesSchemes == null) {
					XmlElement key = xmlDocument.CreateElement ("key");
					key.InnerText = "LSApplicationQueriesSchemes";
					XmlElement array = xmlDocument.CreateElement ("array");
					applicationQueriesSchemes = new PListItem (dict.AppendChild (key), dict.AppendChild (array));
				}

				string[] queriesSchemes = new string[]{ URL_SCHEME_PREFIX_LOBI + clientId, "nakamap-sso", "nakamap", "lobi", "line" };
                AddApplicationQueriesSchemes (xmlDocument, applicationQueriesSchemes.itemValueNode, queriesSchemes);

				xmlDocument.Save (file);
				
				// Remove extra gargabe added by the XmlDocument save
				UpdateStringInFile (file, "dtd\"[]>", "dtd\">");
			} else {
				Debug.Log ("Info.plist is not valid");
			}
		} catch (Exception e) {
			Debug.Log ("Unable to update Info.plist: " + e);
		}
	}

	/// <summary>
	/// Info.plist に URL Scheme を設定する
	/// </summary>
	/// <param name="xmlDocument">Xml document.</param>
	/// <param name="dictContainer">Dict container.</param>
	/// <param name="urlScheme">URL scheme.</param>
	private static void AddUrlScheme (XmlDocument xmlDocument, XmlNode dictContainer, string urlScheme)
	{
		if (!CheckIfUrlSchemeExists (dictContainer, urlScheme)) {
			XmlElement dict = xmlDocument.CreateElement ("dict");
			
			XmlElement str = xmlDocument.CreateElement ("string");
			str.InnerText = urlScheme;
			
			XmlElement key = xmlDocument.CreateElement ("key");
			key.InnerText = "CFBundleURLSchemes";
			
			XmlElement array = xmlDocument.CreateElement ("array");
			array.AppendChild (str);
			
			dict.AppendChild (key);
			dict.AppendChild (array);
			
			dictContainer.AppendChild (dict);
		}
	}

	/// <summary>
	/// URL Scheme が設定済みか判定する
	/// </summary>
	/// <returns><c>true</c>, if if URL scheme exists was checked, <c>false</c> otherwise.</returns>
	/// <param name="dictContainer">Dict container.</param>
	/// <param name="urlScheme">URL scheme.</param>
	private static bool CheckIfUrlSchemeExists (XmlNode dictContainer, string urlScheme)
	{
		foreach (XmlNode dict in dictContainer.ChildNodes) {
			if (dict.Name.ToLower ().Equals ("dict")) {
				PListItem bundleUrlSchemes = GetPlistItem (dict, "CFBundleURLSchemes");
				
				if (bundleUrlSchemes != null) {
					if (bundleUrlSchemes.itemValueNode.Name.Equals ("array")) {
						foreach (XmlNode str in bundleUrlSchemes.itemValueNode.ChildNodes) {
							if (str.Name.Equals ("string")) {
								if (str.InnerText.Equals (urlScheme)) {
									return true;
								}
							} else {
								Debug.Log ("CFBundleURLSchemes array contains illegal elements.");
							}
						}
					} else {
						Debug.Log ("CFBundleURLSchemes contains illegal elements.");
					}
				}
			} else {
				Debug.Log ("CFBundleURLTypes contains illegal elements.");
			}
		}
		
		return false;
	}

	/// <summary>
	/// Info.plist に ApplicationQueriesSchemes を設定する
	/// </summary>
	/// <param name="xmlDocument">Xml document.</param>
    /// <param name="stringContainer">String container.</param>
    /// <param name="queriesSchemes">queriesSchemes.</param>
    private static void AddApplicationQueriesSchemes (XmlDocument xmlDocument, XmlNode stringContainer, string[] queriesSchemes)
	{
        foreach (string queriesScheme in queriesSchemes) {
            if (!CheckIfQueriesSchemeExists(stringContainer, queriesScheme)) {
                XmlElement str = xmlDocument.CreateElement ("string");
                str.InnerText = queriesScheme;

                stringContainer.AppendChild(str);
            }
        }
	}

    private static bool CheckIfQueriesSchemeExists (XmlNode stringContainer, string queriesScheme)
    {
        foreach (XmlNode str in stringContainer.ChildNodes) {
            if (str.Name.ToLower().Equals("string")) {                
                if (str.InnerText.Equals(queriesScheme)) {
                    return true;
                }
            } else {
                Debug.Log ("LSApplicationQueriesSchemes array contains illegal elements.");
            }
        }

        return false;
    }

	private class PListItem
	{
		public XmlNode itemKeyNode;
		public XmlNode itemValueNode;

		public PListItem (XmlNode keyNode, XmlNode valueNode)
		{
			itemKeyNode = keyNode;
			itemValueNode = valueNode;
		}
	}

	private static PListItem GetPlistItem (XmlNode dict, string name)
	{
		for (int i = 0; i < dict.ChildNodes.Count - 1; i++) {
			XmlNode node = dict.ChildNodes.Item (i);
			
			if (node.Name.ToLower ().Equals ("key") && node.InnerText.ToLower ().Equals (name.Trim ().ToLower ())) {
				XmlNode valueNode = dict.ChildNodes.Item (i + 1);
				
				if (!valueNode.Name.ToLower ().Equals ("key")) {
					return new PListItem (node, valueNode);
				} else {
					Debug.Log ("Value for key missing in Info.plist");
				}
			}
		}
		
		return null;
	}

	private static bool HasStringInFile (string filePath, string targetString)
	{
		try {
			if (!File.Exists(filePath)) {
				return false;
			}

			using (StreamReader sr = new StreamReader(filePath)) {
				while (sr.Peek() >= 0) {
					string line = sr.ReadLine();
					if (line.Contains(targetString)) {
						return true;
					}
				}
			}
		}
		catch (Exception e) {
			Debug.Log("Unable to check the file: " + e);
		}

		return false;
	}

	private static void UpdateStringInFile (string file, string subject, string replacement)
	{
		try {
			if (!File.Exists (file)) {
				return;
			}
			
			string processedContents = "";
			
			using (StreamReader sr = new StreamReader (file)) {
				while (sr.Peek () >= 0) {
					string line = sr.ReadLine ();
					processedContents += line.Replace (subject, replacement) + "\n";
				}
			}
			
			File.Delete (file);
			
			using (StreamWriter streamWriter = File.CreateText (file)) {
				streamWriter.Write (processedContents);
			}
		} catch (Exception e) {
			Debug.Log ("Unable to update string in file: " + e);
		}
	}

}
