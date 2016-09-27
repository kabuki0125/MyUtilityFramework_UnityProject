using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



public class DownloadManager {


	public enum DownloadType {
		DownloadOnly, DownloadAndLocalCache
	}

	/// <summary>GoogleDriveからファイルをDLする為のURL。後にファイルIDを付けて使う。</summary>
	public static string	GoogleDriveFileDownloadPath {
								get{
									if( instance == null ) return "";
									return instance.baseURL;
								}
							}

	/// <summary>
	/// ダウンロード処理を受け付ける準備ができているか？
	/// </summary>
	public static bool		IsReady { get; private set; }


	private static DownloadManager			instance;

	private WWWDownloader					downloader;
	private Queue<WWWDownloader.Params>		downloadQueue;

	private bool							useGoogleDrive;		// Googleドライブ上のデータを扱う？
	private string							baseURL;
	private CoroutineAgent.CoroutineInfo	currentDownload;

	private Dictionary<string,string[]>		fileDataDictionary;





	/// <summary>
	/// 初期化処理
	/// baseURLに http://drive.google.com が指定された場合、サーバにGoogleDriveを使用する。
	/// </summary>
	public static void Init( string baseURL, string localCachePath, string fileDictFileID = null ){

		if( instance == null ) instance = new DownloadManager();

		instance.downloader = WWWDownloader.Create( URLBuilder, localCachePath );
		instance.downloadQueue = new Queue<WWWDownloader.Params>();

		instance.useGoogleDrive = ( baseURL == "http://drive.google.com" );

		if( instance.useGoogleDrive ){
			instance.baseURL = baseURL + "/uc?export=view&id=";
		} else{
			instance.baseURL = baseURL;
		}

		CoroutineAgent.Execute( instance.SetFilePathDictionary( fileDictFileID ) )
					  .Next( instance.Process() );
	}


	private static string URLBuilder( string relativeURL ){
		if( instance.useGoogleDrive ){
			string fileID = GetFileID( relativeURL );
			if( fileID == null ){
				Debug.LogError( "GoogleDrive FileID not found!  path:" + relativeURL );
				return null;
			}
			return instance.baseURL + fileID;
		} else{
			return instance.baseURL + "/" + relativeURL;
		}
	}


	/// <summary>
	/// ダウンロード通信の設定用データクラスを生成する
	/// </summary>
	public static WWWDownloader.Params CreateParams( string relativeURL, string localStorePath = null ){
		if( instance == null ) return null;
		if( localStorePath == null ) return instance.downloader.CreateParamsAsDownloadOnly( relativeURL );
		return instance.downloader.CreateParamsAsDownloadAndCache( relativeURL, localStorePath, GetFileLastUpdateTime( relativeURL ) );
	}


	/// <summary>
	/// 指定パラメータでのダウンロード通信を実行待ちキューに登録する
	/// </summary>
	public static void Register( WWWDownloader.Params p ){
		if( instance == null ) return;

		instance.downloadQueue.Enqueue( p );
	}


	/// <summary>
	/// 全てのダウンロードを取り止める
	/// </summary>
	public static void AbortAll(){
		if( instance == null ) return;

		instance.downloadQueue.Clear();
		if( instance.currentDownload != null ) CoroutineAgent.Stop( instance.currentDownload );
	}


	/// <summary>
	/// ダウンロードされたデータを文字列データに変換する。
	/// 暗号化データ対応。
	/// </summary>
	public static string ConvertDonwloadDataToString( byte[] data ){
		return JsonDataParser.MasterDataParser.ConvertString( data );
	}


	/// <summary>ファイルのIDを得る</summary>
	public static string GetFileID( string filePath ){
		if( instance.fileDataDictionary.ContainsKey( filePath ) ) return instance.fileDataDictionary[filePath][0];
		return null;
	}


	/// <summary>ファイルの最終更新日を得る</summary>
	public static DateTime GetFileLastUpdateTime( string filePath ){
		return ServerTime.GetTime( long.Parse( instance.fileDataDictionary[filePath][1] ) );
	}


	private IEnumerator Process(){

		// サーバ時刻を取得する
		yield return ServerTime.SetFromTimeServer();
		CoroutineAgent.Execute( TickTime() );

		IsReady = true;

		while( true ){
			if( downloadQueue.Count > 0 ){
				currentDownload = CoroutineAgent.Execute( downloader.Download( downloadQueue.Dequeue() ) );
				yield return currentDownload;
				currentDownload = null;
			}
			yield return new WaitForEndOfFrame();
		}
	}


	private IEnumerator TickTime(){
		while( true ){
			ServerTime.Update( Time.deltaTime );
			yield return new WaitForEndOfFrame();
		}
	}


	// ファイルパスからGoogleドライブIDや最終更新日などの情報を引けるDictionaryを作成する
	private IEnumerator SetFilePathDictionary( string fileDictFileID ){
		WWW	www;

		fileDataDictionary = new Dictionary<string,string[]>();

		if( useGoogleDrive ){
			www = new WWW( GoogleDriveFileDownloadPath + fileDictFileID );
		} else{
			www = new WWW( baseURL + "/FileList.dat" );
		}

		while( !www.isDone ){
			yield return null;
		}

		if( ( www == null ) || ( www.error != null ) ){
			Debug.LogError( "### FileDictionary download error! ###" );

		} else{
			string text = DownloadManager.ConvertDonwloadDataToString( www.bytes );
			foreach( var s in text.Split( ',' ) ){
				string[] v = s.Split( ':' );
				if( !useGoogleDrive ) v[1] = "";
				fileDataDictionary[v[0]] = new string[]{ v[1], v[2] };
				Debug.Log( v[0] + ": " + v[1] + ", " + v[2] );
			}
		}
	}


}


