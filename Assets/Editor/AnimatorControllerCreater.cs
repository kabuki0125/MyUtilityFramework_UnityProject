using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;


/// <summary>
/// クラス：エディタメニューからAnimatorControllerを生成するクラス.
/// </summary>
public class AnimatorControllerCreater
{

#region GameProc.
    
    /// <summary>
    /// BlendTreeを利用するアニメーションコントローラを生成.このサンプルでは歩行モーションを想定.
    /// </summary>
    [MenuItem("Tools/CreateAnimatorController/Output BlendTree")]
	public static void OutputBlendTreeController()
    {
        var bOK = EditorUtility.DisplayDialog("8方向歩行アニメーションサンプル作成", 
                                              PATH_ROOT+"以下に設定されている" +
                                              "ディレクトリ構成に倣ってAnimatorControllerを作成します。\n" +
                                              "各AnimationClipが格納されているwalk_nディレクトリ単位でAnimatorControllerを生成します。\n" +
                                              "成果物は"+PATH_OUTPUT+"に出力されます。\n" +
                                              "よろしければOKを押してください。",
                                              "OK", "Cancel");
        if(bOK){
            if(!Directory.Exists(PATH_ROOT)){
                EditorUtility.DisplayDialog("生成失敗！", "ルートディレクトリ："+PATH_ROOT+"が見つかりませんでした。\nディレクトリ構成を確認してください。", "閉じる");
                return;
            }
            
            Debug.Log("-------------------->>\nBlendTreeAnimatorController作成開始....\n:RootDirectory="+PATH_ROOT+"  Output="+PATH_OUTPUT);
            if(!Directory.Exists(PATH_OUTPUT)){
                Directory.CreateDirectory(PATH_OUTPUT);
            }
            
            // 配置されているディレクトリの数分コントローラを作成する.
            var aList = new List<RuntimeAnimatorController>();
            var dirs = Directory.GetDirectories(PATH_ROOT);
            foreach(var dir in dirs){
                if(dir.IndexOf("walk") < 0){
                    continue;   // TODO : walkと名のつくファイルに入っている想定.実際に使う場合はアニメーションの種別に応じて命名規則を設定すること.
                }
                var resPath = PATH_OUTPUT+dir.Remove(0,PATH_ROOT.Length-1)+".controller";
                var blendTree = CreateWalkBlendTree(dir);    // 生成されるアニメーションはBlendTreeを利用したもの.
                aList.Add( CreateWalkController(blendTree, resPath) );    // 定義クラスを作成.BlendTreeを追加.作成した定義クラスを元にAnimatorControllerを作成する.
            }
            
            // TODO : ゲームとステージ選択のObjectGeneratorに追加してみる.
            AddControllerToObjectGenerator(aList, "GameObjects", "StageSelectObjects");
        }
    }
    // 歩行モーションを想定したBlendTree作成.
    private static BlendTree CreateWalkBlendTree(string directory)
    {
        Debug.Log(" >Load Directory="+directory);
        
        var blendTree = new BlendTree();
        blendTree.name = "Blend Tree";
        blendTree.useAutomaticThresholds = false;
        blendTree.blendParameter = "Degree";
        
        // 指定ディレクトリに格納されているアニメーションクリップを全て読み込み.BlendTreeを作成.
        AnimationClip degree0Anim = null;   // ディクリー角0度のアニメーション.
        var animPaths = Directory.GetFiles(directory);
        var threshold = 270f;
        foreach(var animPath in animPaths){
            if(animPath.IndexOf(".meta") > 0){
                continue;   // メタファイル無視.
            }
            var anim = AssetDatabase.LoadAssetAtPath<AnimationClip>(animPath);
            blendTree.AddChild(anim, threshold);
            
            // ディクリー角0度のアニメーションは360度の時にも使用するものになるので予め保持しておき一通り設定し終わった後に設定する.
            if(threshold == 0){
                degree0Anim = anim;
            }
            // 0〜360度の角度で切り替わるBlendTreeアニメーションを形成する.
            threshold -= 45f;
            if(threshold < 0){
                threshold += 360f;
            }
            
            Debug.Log("  >Load AnimationClip="+anim);
        }
        // 360度の時にもアニメーションを適用.
        blendTree.AddChild(degree0Anim, 360f);
        
        // BlendTreeもアセットとして作成し保存しておかないとシーン再生のタイミングで消える.
        var name = "WalkBlendTree_"+directory.Remove(0,PATH_ROOT.Length);
        AssetDatabase.CreateAsset(blendTree, PATH_OUTPUT+"/BlendTreeTemp/"+name);       // なんでもいいので一度Assetとして保存しておかないとEditor上の出来事になってしまいシーン再生時にBlendTreeが消える.
        AssetDatabase.SaveAssets();
//        AssetDatabase.DeleteAsset("Assets/"+blendTree.name);            // なので一度作成して、セーブした後不要な筈のBlendTree仮Assetデータを削除する.これで消えないようになる.....が、ここでこれを消すと別のPCでビルドする際などBlendTreeの見えない何かがコミットされずに上手くいかない.
        
        return blendTree;
    }
    // AnimatorController作成.
    private static RuntimeAnimatorController CreateWalkController(BlendTree blendTree, string resPath)
    {
        var definition = new AnimatorControllerDefinition();
        definition.MotionList.Add(blendTree);
        definition.LayerName = "Base Layer";
        definition.ResulutPath = resPath;
        definition.ParameterList.Add(new AnimatorControllerParameter{
            type = AnimatorControllerParameterType.Float,
            name = "Degree",
            defaultFloat = 90f,
        });
        Debug.Log(" >Create AnimatorContoller Success!!\n  State="+definition.MotionList[0].name+"\n  LayerName="+definition.LayerName+"\n  ResulutPath="+definition.ResulutPath);
        return CreateController(definition);
    }
    
    /// <summary>
    /// AnimatorControllerを生成サンプル.
    /// </summary>
    [MenuItem("Tools/CreateAnimatorController/Output GeneralController")]
    public static void OutputSampleController()
    {
        var bOK = EditorUtility.DisplayDialog("サンプルアニメーション作成", 
                                              PATH_ROOT+"以下に設定されている" +
                                              "ディレクトリ構成に倣ってAnimatorControllerを作成します。\n" +
                                              "各AnimationClipが格納されているname_define_nディレクトリ単位でAnimatorControllerを生成します。\n" +
                                              "成果物は"+PATH_OUTPUT+"に出力されます。\n" +
                                              "よろしければOKを押してください。",
                                              "OK", "Cancel");
        if(bOK){
            if(!Directory.Exists(PATH_ROOT)){
                EditorUtility.DisplayDialog("生成失敗！", "ルートディレクトリ："+PATH_ROOT+"が見つかりませんでした。\nディレクトリ構成を確認してください。", "閉じる");
                return;
            }
            
            Debug.Log("-------------------->>\nAnimatorController作成開始....\n:RootDirectory="+PATH_ROOT+"  Output="+PATH_OUTPUT);
            if(!Directory.Exists(PATH_OUTPUT)){
                Directory.CreateDirectory(PATH_OUTPUT);
            }
            
            // 配置されているディレクトリの数分コントローラを作成する.
            var aList = new List<RuntimeAnimatorController>();
            var dirs = Directory.GetDirectories(PATH_ROOT);
            foreach(var dir in dirs){
                if(dir.IndexOf("name_define") < 0){
                    continue;   // TODO : name_defineと名のつくファイルに入っている想定.実際に使う場合はアニメーションの種別に応じて命名規則を設定すること.
                }
                var resPath = PATH_OUTPUT+dir.Remove(0,PATH_ROOT.Length-1)+".controller";
                var animClip = LoadAnimationClip(dir);    // 生成されるアニメーションはBlendTreeを利用したもの.
                aList.Add( CreateController(animClip, resPath) );    // 定義クラスを作成.BlendTreeを追加.作成した定義クラスを元にAnimatorControllerを作成する.
            }
            // TODO : ObjectGeneratorに追加.
            AddControllerToObjectGenerator(aList, "GameObjects");
        }
    }
    // AnimationClip読み込み
    private static AnimationClip LoadAnimationClip(string directory)
    {
        var animPaths = Directory.GetFiles(directory);
        return AssetDatabase.LoadAssetAtPath<AnimationClip>(animPaths[0]);  // TODO : １種類だけ返すサンプル.
    }
    // AnimatorController作成.
    private static RuntimeAnimatorController CreateController(AnimationClip animClip, string resPath)
    {
        var definition = new AnimatorControllerDefinition();
        definition.MotionList.Add(animClip);
        definition.LayerName = "Base Layer";
        definition.ResulutPath = resPath;
        Debug.Log(" >Create AnimatorContoller Success!!\n  State="+definition.MotionList[0].name+"\n  LayerName="+definition.LayerName+"\n  ResulutPath="+definition.ResulutPath);
        return CreateController(definition);
    }
    
    // ObjectGeneratorを用いた場合サンプル : 指定のコントローラをObjectGeneratorに追加する.
    private static void AddControllerToObjectGenerator(List<RuntimeAnimatorController> aList, params string[] destDirectoryNames)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Project/Prefabs/AppCore.prefab");
        var core = prefab.GetComponent<AppCore>();
        
        // 指定名のObjectGeneratorにだけ追加. 
        foreach(var path in destDirectoryNames){
            var gene = core.GetScript<ObjectGenerator>(path);
            if(gene == null){
                continue;
            }
            gene.AddPrefab(aList.ToArray());
        }
        
        AssetDatabase.SaveAssets();
        
        // シーン配置されているインスタンスが変更したprefabの内容を反映できていないのでRevertをかけてやることで反映.
        var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach(var obj in rootObjects){
            PrefabUtility.RevertPrefabInstance(obj);
        }
    }
    
    // TODO : ゲーム内容に応じて読み込み・出力先ディレクトリを変える.
    private static readonly string PATH_ROOT = "Assets/SampleProject/Animations/PlayerAnimations/";
    private static readonly string PATH_OUTPUT = "Assets/SampleProject/Animations/PlayerAnimations/output";   
    
#endregion
    
    // AnimatorController作成.
    private static RuntimeAnimatorController CreateController(AnimatorControllerDefinition definition)
    {
        AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath(definition.ResulutPath);
        
        // ステート追加. 
        AnimatorStateMachine stateMachine = animatorController.layers[0].stateMachine;
        foreach(Motion motion in definition.MotionList){
            AnimatorState state = stateMachine.AddState(motion.name);
            state.motion = motion;
        }
        // パラメータ追加.
        foreach(var p in definition.ParameterList){
            animatorController.AddParameter(p);
        }
        
        // 保存.
        EditorUtility.SetDirty(animatorController);
        AssetDatabase.SaveAssets(); // 念のためAssetの更新をかけておく.
        
        return animatorController;
    }
    
    // AnimatorController作成用定義クラス.
    private class AnimatorControllerDefinition
    {
        public List<Motion> MotionList { get; set; }
        public string LayerName { get; set; }        
        public string ResulutPath { get; set; }
        public List<AnimatorControllerParameter> ParameterList { get; set; }
        
        public AnimatorControllerDefinition()
        {
            MotionList = new List<Motion>();
            ParameterList = new List<AnimatorControllerParameter>();
        }
    }
}
