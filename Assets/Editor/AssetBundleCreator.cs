using UnityEditor;
using UnityEngine;
using System.IO;

/// <summary>
/// アセットバンドル生成.
/// </summary>
public class AssetBundleCreator
{

    /// <summary>
    /// 全てビルド.
    /// </summary>
    [MenuItem("Tools/Create/CreateAssetBundle_All")]
	public static void BuildAll()
    {
        Build_Android();
        Build_iOS();
    }
    
    /// <summary>
    /// Android.
    /// </summary>
    [MenuItem("Tools/Create/CreateAssetBundle_Android")]
    public static void Build_Android()
    {
        var path = UserSettings.AssetBundleDestPath_Android;
        if(!Directory.Exists(path)){
            Directory.CreateDirectory(path);
        }
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.Android);
    }
    
    /// <summary>
    /// iOS.
    /// </summary>
    [MenuItem("Tools/Create/CreateAssetBundle_iOS")]
    public static void Build_iOS()
    {
        var path = UserSettings.AssetBundleDestPath_iOS;
        if(!Directory.Exists(path)){
            Directory.CreateDirectory(path);
        }
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.iOS);
    }
}
