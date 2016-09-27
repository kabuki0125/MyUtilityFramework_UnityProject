using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System;
using System.Collections;
using System.IO;

/// <summary>
/// クラス：だいこんの8方向アニメーションに対応するanimator controllerを生成するクラス.
/// </summary>
public class CreateRadishAnimController
{

    /// <summary>
    /// 8方向アニメーションを元にAnimatorのコントローラを生成.
    /// </summary>
    [MenuItem("Tools/Create/Create Radish AnimatorController")]
	public static void Exec()
    {
        var bOK = EditorUtility.DisplayDialog("注意", 
                                              "Assets/Radish/Anime/RadishAnimations以下に設定されている" +
                                              "ディレクトリ構成に倣ってAnimatorControllerを作成します。\n" +
                                              "AnimatorControllerの名前はAnimationClipが格納されているディレクトリ名になります。\n" +
                                              "各ディレクトリに必ず8方向のAnimationのみが格納されている状態で実行してください。\n" +
                                              "よろしければOKを押してください。",
                                              "OK", "Cancel");
        if(bOK){
            var rootPath = "Assets/Radish/Anime/RadishAnimations/";
//            var ac = AssetDatabase.LoadAssetAtPath<AnimatorController>(rootPath+"daikon_dir.controller");   // テンプレート用のAnimatorControllerを元に種類分作成していく.
            var dirs = Directory.GetDirectories(rootPath);  // 配置されているディレクトリの数分コントローラを作成する.
            
            foreach(var dir in dirs){
                Debug.Log(rootPath+rootPath+dir.Remove(0, rootPath.Length));
//                var newCtrl = AnimatorController.CreateAnimatorControllerAtPath(rootPath+dir.Remove(0, rootPath.Length));
                
                /*
                var newCtrl = UnityEngine.Object.Instantiate(ac) as AnimatorController;
                newCtrl.name = dir;
                
                var allPath = Directory.GetFiles(dir);
                foreach(var a in newCtrl.animationClips){
                    AnimationClip animClip;
                    Array.Find(allPath, p => {
                        if(p.IndexOf(".meta") > 0){
                            return false;
                        }
                        animClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(p);
                        Debug.Log(animClip);
                        if(animClip == null){
                            return false;
                        }
                        return a.name.Contains(animClip.name);
                    });
                    // TODO : animationClipsはリードオンリー.stateMacineからいくとBlendTreeが見えない.どっから設定したらいいんだ？
                }
                */
                
//                EditorUtility.SetDirty(newCtrl);
//                AssetDatabase.SaveAssets(); // アセットを最新に更新
            }
        }
    }
}
