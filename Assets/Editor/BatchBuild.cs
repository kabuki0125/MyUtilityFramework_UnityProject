using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

/// <summary>
/// バッチモードによるビルド用クラス.
/// </summary>
public class BatchBuild
{

	static BatchBuildUtility.Settings[]	BatchBuildSettingTbl ={
										  // 開発用
											new BatchBuildUtility.Settings( "DEBUG_MODE", "SERVER_DEVELOP" ),									
										  // ステージング用
											new BatchBuildUtility.Settings( "SERVER_STAGING" ),									
										  // 本番用
											new BatchBuildUtility.Settings( "SERVER_PRODUCT" ),									
										};



	[MenuItem("Build/定数設定/Dev用")]
	public static void SetSymbolForDev(){
		BatchBuildUtility.ApplySetting( BatchBuildSettingTbl[0] );
	}
	public static void SetSymbolForDev( BuildTargetGroup BTG ){
		BatchBuildUtility.ApplySetting( BTG, BatchBuildSettingTbl[0] );
	}

	[MenuItem("Build/定数設定/Stg用")]
	public static void SetSymbolForStg(){
		BatchBuildUtility.ApplySetting( BatchBuildSettingTbl[1] );
	}
	public static void SetSymbolForStg( BuildTargetGroup BTG ){
		BatchBuildUtility.ApplySetting( BTG, BatchBuildSettingTbl[1] );
	}

	[MenuItem("Build/定数設定/Prd用")]
	public static void SetSymbolForPrd(){
		BatchBuildUtility.ApplySetting( BatchBuildSettingTbl[2] );
	}
	public static void SetSymbolForPrd( BuildTargetGroup BTG ){
		BatchBuildUtility.ApplySetting( BTG, BatchBuildSettingTbl[2] );
	}


    /// <summary>
    /// iOSビルド. 開発用.
    /// </summary>
    [MenuItem("Build/ROM iOS_Dev")]
    public static void iOSDev()
    {
		SetSymbolForDev( BuildTargetGroup.iOS );
        Build(BuildTarget.iOS, BuildOptions.Development);
    }
    /// <summary>
    /// iOSビルド. リリース用.
    /// </summary>
    [MenuItem("Build/ROM iOS_Release")]
    public static void iOSRelease()
    {
        SetSymbolForStg( BuildTargetGroup.iOS );
        Build(BuildTarget.iOS, BuildOptions.SymlinkLibraries);
    }
    
    /// <summary>
    /// Androidビルド. 開発用.
    /// </summary>
    [MenuItem("Build/ROM Android_Dev")]
    public static void AndroidDev()
    {
		SetSymbolForDev( BuildTargetGroup.Android );
        Build(BuildTarget.Android, BuildOptions.Development);
    }
    /// <summary>
    /// Androidビルド. リリース用.
    /// </summary>
    [MenuItem("Build/ROM Android_Release")]
    public static void AndroidRelease()
    {
        SetSymbolForStg( BuildTargetGroup.Android );
        Build(BuildTarget.Android, BuildOptions.None);
    }
    
    // ビルド処理.
    private static void Build(BuildTarget target, BuildOptions option)
    {
        // BuildTargetをtargetに合わせる.
        if(EditorUserBuildSettings.activeBuildTarget != target){
            EditorUserBuildSettings.SwitchActiveBuildTarget(target);
        }
        
        // TODO : 事前に色々設定.事前にどこかでここの処理をまとめておいたほうがいいかも.
        PlayerSettings.companyName = "OriginCompany";
        PlayerSettings.productName = "ORIGIN";
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;    // TODO : 縦持ち？
        PlayerSettings.bundleIdentifier = "com.origin.org";
        PlayerSettings.bundleVersion = "1.0";   // TODO : バージョン管理も必要になったタイミングで.
        
        // シーン
        var scenes = from i in EditorBuildSettings.scenes
                     select i.path;
        
        // 出力先
        var destName = target == BuildTarget.Android ? "Android.apk": "xcode";
        
        // 実行
        var eMsg = BuildPipeline.BuildPlayer(scenes.ToArray(), destName, target, option);
        Debug.Log( string.IsNullOrEmpty(eMsg) ? "Scuccess" : "Error!! : "+eMsg );
    }
}
