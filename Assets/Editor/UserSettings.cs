using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor操作を行う上での各種ユーザー設定.
/// </summary>
public class UserSettings : EditorWindow
{
    /// <summary>アセットバンドル出力先ディレクトリ Android.</summary>
    public static string AssetBundleDestPath_Android 
    { 
        get {
            var rtn = PlayerPrefs.GetString(PREFS_KEY_ANDROID);
            if(string.IsNullOrEmpty(rtn)){
                rtn = "Assets/AssetBundle/Android";
                PlayerPrefs.SetString(PREFS_KEY_ANDROID, rtn);
            }
            return rtn;
        }
        private set {
            PlayerPrefs.SetString(PREFS_KEY_ANDROID, value);
            PlayerPrefs.Save();
        }
    }
    
    /// <summary>アセットバンドル出力先ディレクトリ iOS.</summary>
    public static string AssetBundleDestPath_iOS 
    { 
        get {
            var rtn = PlayerPrefs.GetString(PREFS_KEY_iOS);
            if(string.IsNullOrEmpty(rtn)){
                rtn = "Assets/AssetBundle/iOS";
                PlayerPrefs.SetString(PREFS_KEY_iOS, rtn);
            }
            return rtn;
        }
        private set {
            PlayerPrefs.SetString(PREFS_KEY_iOS, value);
            PlayerPrefs.Save();
        }
    }
    
    
    /// <summary>
    /// メニュー展開.
    /// </summary>
    [MenuItem("Tools/Setting/UserSettings")]
	static void Open()
    {
        GetWindow<UserSettings>();
    }
    
    // メニュー一覧.自動表示.
    void OnGUI()
    {
        EditorGUILayout.HelpBox("ここではEditor操作上の各種設定を変更することが出来ます。", MessageType.Info);

        // --- AssetBundle出力先変更 ---
        EditorGUILayout.LabelField("AssetBundle出力先ディレクトリ");
        temp_AndroidPath = string.IsNullOrEmpty(temp_AndroidPath) ? AssetBundleDestPath_Android : temp_AndroidPath;
        temp_iOSPath = string.IsNullOrEmpty(temp_iOSPath) ? AssetBundleDestPath_iOS : temp_iOSPath;
        temp_AndroidPath = EditorGUILayout.TextField("Android", temp_AndroidPath);
        temp_iOSPath = EditorGUILayout.TextField("iOS", temp_iOSPath);
        
        
        // 変更があった場合上書き保存.
        if(GUILayout.Button("Save")){
            if(temp_AndroidPath != AssetBundleDestPath_Android){
                AssetBundleDestPath_Android = temp_AndroidPath;
            }
            if(temp_iOSPath != AssetBundleDestPath_iOS){
                AssetBundleDestPath_iOS = temp_iOSPath;
            }
        }
    }
    private string temp_AndroidPath = null;
    private string temp_iOSPath = null;
    
    
    private static readonly string PREFS_KEY_ANDROID = "ABDestPath_Android";
    private static readonly string PREFS_KEY_iOS = "ABDestPath_iOS";
}
