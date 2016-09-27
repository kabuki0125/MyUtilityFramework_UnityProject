using UnityEditor;
using UnityEngine;
using System.Collections;


public class BatchBuildUtility {

	/// <summary>
	/// BatchBuildで行うビルドの設定を定義する為のクラス
	/// </summary>
	public class Settings {

		public readonly string[]	Defines;

		public Settings( params string[] defines ){
			Defines = new string[defines.Length];
			for( int i = 0; i < Defines.Length; i++ ){
				Defines[i] = defines[i];
			}
		}
	}


	/// <summary>
	/// 設定用クラスの内容をUnityエディタの環境に反映する
	/// </summary>
	public static void ApplySetting( Settings setting ){
		ApplySetting( EditorUserBuildSettings.selectedBuildTargetGroup, setting );
	}
	public static void ApplySetting( BuildTargetGroup targetGroup, Settings setting ){
		SetDefineSymbols( targetGroup, setting.Defines );
	}


	/// <summary>
	/// 指定ビルドターゲットのコンパイル用シンボルを設定する
	/// </summary>
	public static void SetDefineSymbols( BuildTargetGroup targetGroup, params string[] defiles ){

        PlayerSettings.SetScriptingDefineSymbolsForGroup( targetGroup, "" );

		string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup( targetGroup );
		foreach( string def in defiles ){
			symbols += ";" + def;
		}
		PlayerSettings.SetScriptingDefineSymbolsForGroup( targetGroup, symbols );

		Debug.Log( "### Scripting Define Symbols: " + symbols );
	}


	/// <summary>
	/// Unityエディタ起動時に一度だけ呼ばれるイベント
	/// </summary>
	public static void OnBootEditor(){
	}


	[InitializeOnLoadMethod]
	private static void OnLoad(){
		if( System.IO.File.Exists( InitUnityEditorMarkerFilePath ) ) return;
		System.IO.File.Create( InitUnityEditorMarkerFilePath );
		OnBootEditor();
	}

	private static readonly string		InitUnityEditorMarkerFilePath = "Temp/xxxxxxxxxx";


}
