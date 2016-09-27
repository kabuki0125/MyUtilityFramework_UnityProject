using UnityEngine;
using System.Collections;
using MyLibrary.Unity;

/// <summary>
/// クラス：アプリ共通定義
/// </summary>
public static class ClientDefine
{
    
#region Directory getters.
    
    /// <summary>ディレクトリパス：マスターデータ.</summary>
    public static string MasterDataDirectoryPath { get { return GameSystem.CachesDirectoryPath+"/0000"; } }
    
#endregion

    /// <summary>GoogleDriveファイルアクセス用辞書のファイルID</summary>
	public static readonly string GoogleDriveFileDictionaryFileID = "hogehogepiyopiyo";

    /// <summary>固定設定しているアスペクト比.widthとheigthのサイズ.</summary>
    public static readonly Vector2 GAME_SCREEN_SIZE = new Vector2(720f, 1280f);

	#region サーバ関連
	/// <summary>接続先サーバ種別</summary>
	public enum ServerType { Develop, Staging, Product }

	/// <summary>接続先サーバ</summary>
 #if SERVER_DEVELOP
	public static readonly ServerType	AccessServer = ServerType.Develop;
 #elif SERVER_STAGING
	public static readonly ServerType	AccessServer = ServerType.Staging;
 #elif SERVER_PRODUCT
	public static readonly ServerType	AccessServer = ServerType.Product;
 #else
	public static readonly ServerType	AccessServer = ServerType.Develop;
 #endif
	#endregion

	/// <summary>デバッグモードか？</summary>
#if DEBUG_MODE
	public static readonly bool		IsDebugMode = true;
#else
	public static readonly bool		IsDebugMode = false;
#endif


}
