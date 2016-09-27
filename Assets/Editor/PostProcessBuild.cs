using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.XCodeEditor;
using UnityEditor.iOS.Xcode;
using UnityEngine;
using System.IO;


/// <summary>
/// クラス：ビルド事後処理.
/// </summary>
public class PostprocessBuild
{
    /// <summary>
    /// ポストプロセス時のコールバック.
    /// </summary>
    /// <param name="target">ビルドターゲット.</param>
    /// <param name="path">出力先のパス（ビルド時に指定するパス）</param>
    [PostProcessBuild]
    public static void OnPostProcessBuild (BuildTarget target, string path)
    {
        if (target != BuildTarget.iOS) {
            return;
        }
        ProcessXCodeProject(path);
    }
    
    /// <summary>
    /// projmods ファイルの設定値を XCode プロジェクト設定へ反映する.
    /// </summary>
    static void ProcessXCodeProject (string path)
    {
        XCProject project = new XCProject(path);
        
        string[] files = Directory.GetFiles(path, "*.projmods", SearchOption.AllDirectories);
        
        foreach(string file in files){
            project.ApplyMod(Path.Combine(Application.dataPath, file));
        }
        // Xcode7.0から実装されたBitCode対応しているかどうかの設定.デフォルトでYESになっている.
        project.overwriteBuildSetting("ENABLE_BITCODE", "NO");
        // XCode6.1 のアップデートでいろいろ動かなくなった所の一つ対処用.
        project.overwriteBuildSetting("CODE_SIGN_RESOURCE_RULES_PATH", "$(SDKROOT)/ResourceRules.plist");
        
        project.Save();
    }
}
