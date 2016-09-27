using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.IO;
using System.Xml;

/// <summary>
/// Lobi の設定（クライアントIDなど）をするための画面をMenuやInspectorパネルに表示する
/// </summary>
[CustomEditor(typeof(LobiSettings))]
public class LobiSettingsEditor : Editor {
	
	public const string settingsFile = "LobiSettings";
	public const string settingsFileExtension = ".asset";

	private LobiSettings _currentSettings = null;

	// Inspector パネルに表示するコントロール
	private static GUIContent _labelClientId = new GUIContent("Client ID");
	private static GUIContent _labelBaseName = new GUIContent("Base Name");
	private static GUIContent _labelIOsSupport = new GUIContent("iOS enabled");
	private static GUIContent _labelTestButtons = new GUIContent ("Enable test buttons");

	/// <summary>
	/// メニューの [Edit] に [Lobi Settings] を追加
	/// </summary>
	[MenuItem("Edit/Lobi Settings")]
	public static void ShowSettings()
	{
		LobiSettings settingsInstance = (LobiSettings)Resources.Load(settingsFile);
		
		if(settingsInstance == null) {
			settingsInstance = CreateLobiSettings();
		}
		
		if(settingsInstance != null) {
			String dirPath = Application.dataPath;

			string[] files = System.IO.Directory.GetFiles (dirPath, "*LobiChat*", System.IO.SearchOption.AllDirectories);
			settingsInstance.chatEnabled = files.Length > 0;
			files = System.IO.Directory.GetFiles (dirPath, "*LobiRec*", System.IO.SearchOption.AllDirectories);
			settingsInstance.recEnabled = files.Length > 0;
			files = System.IO.Directory.GetFiles (dirPath, "*LobiRanking*", System.IO.SearchOption.AllDirectories);
			settingsInstance.rankingEnabled = files.Length > 0;

			// 設定ファイルを選択状態にして、Inspectorパネルに表示する
			Selection.activeObject = settingsInstance;
		}
	}

	public override void OnInspectorGUI()
	{
		try {
			// Might be null when this gui is open and this file is being reimported
			if(target == null) {
				Selection.activeObject = null;
				return;
			}

			_currentSettings = (LobiSettings)target;

			if(_currentSettings == null) {
				return;
			}

			EditorGUILayout.HelpBox("1) Enter your game credentials", MessageType.None);
			EditorGUILayout.HelpBox("You can check your game credentials at https://developer.lobi.co/", MessageType.Info);

			// Client ID
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(_labelClientId, GUILayout.Width(80), GUILayout.Height(18));
			_currentSettings.clientId = TrimmedText(EditorGUILayout.TextField(_currentSettings.clientId));
			EditorGUILayout.EndHorizontal();

			// Base Name
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(_labelBaseName, GUILayout.Width(80), GUILayout.Height(18));
			_currentSettings.baseName = TrimmedText(EditorGUILayout.TextField(_currentSettings.baseName));
			EditorGUILayout.EndHorizontal();

			// プラットフォームのON/OFF設定
			EditorGUILayout.HelpBox("2) Enable auto build settings", MessageType.None);
			EditorGUILayout.BeginVertical();
			_currentSettings.iosSupportEnabled = EditorGUILayout.Toggle(_labelIOsSupport, _currentSettings.iosSupportEnabled);

			if(_currentSettings.iosSupportEnabled == true){
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Chat",GUILayout.Width(112));
				EditorGUILayout.LabelField(GetOnOffString(_currentSettings.chatEnabled));
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Rec",GUILayout.Width(112));
				EditorGUILayout.LabelField(GetOnOffString(_currentSettings.recEnabled));
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Ranking",GUILayout.Width(112));
				EditorGUILayout.LabelField(GetOnOffString(_currentSettings.rankingEnabled));
				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.EndVertical();

			// テストボタンのON/OFF設定
			EditorGUILayout.HelpBox("3) Enable test buttons", MessageType.None);
			EditorGUILayout.BeginVertical();
			_currentSettings.testButtonsEnabled = EditorGUILayout.Toggle(_labelTestButtons, _currentSettings.testButtonsEnabled);
			if(_currentSettings.testButtonsEnabled == true) {
				EditorGUILayout.HelpBox("Add \"Assets/LobiSDK/Scripts/LobiTestButton(prefab)\" to scene", MessageType.Info);
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.HelpBox("If you use REC SDK,\nadd \"Assets/LobiSDK/Scripts/LobiRec(prefab)\" to scene", MessageType.Info);
			EditorGUILayout.EndVertical();
		}
		catch(Exception e) {
			if(e != null) {
			}
		}
	}

	private static string GetOnOffString(bool i_boolValue){
		return i_boolValue ? "ON" : "OFF";
	}

	// 設定ファイルの生成
	private static LobiSettings CreateLobiSettings()
	{
		LobiSettings lobiSettings = (LobiSettings)ScriptableObject.CreateInstance(typeof(LobiSettings));
		
		if(lobiSettings != null) {
			if(!Directory.Exists(System.IO.Path.Combine(Application.dataPath, "Plugins/Lobi/Resources"))) {
				AssetDatabase.CreateFolder("Assets/Plugins/Lobi", "Resources");
			}

			// 初期値設定
			lobiSettings.iosSupportEnabled = true;
			
			AssetDatabase.CreateAsset(lobiSettings, "Assets/Plugins/Lobi/Resources/" + settingsFile + settingsFileExtension);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			
			return lobiSettings;
		}
		
		return null;
	}

	private static string TrimmedText(string txt)
	{
		if(txt != null) {
			return txt.Trim();
		}
		return "";
	}

	// Inspectorパネルが閉じられたときに呼ばれる（ビルド実行時にも呼ばれる）
	void OnDisable()
	{
		if(_currentSettings != null) {
			EditorUtility.SetDirty(_currentSettings);
			_currentSettings = null;
		}
	}
}
