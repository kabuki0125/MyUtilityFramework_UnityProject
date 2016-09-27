using UnityEngine;
using System.Collections;
using System;
using System.IO;
using MyLibrary.Unity.IO;


public class WWWDownloader 
{

	public delegate string URLBuilder( string relativeURL );

	public class Params {

		/// <summary>予め設定されているサーバURLからの相対URLを表す文字列</summary>
		public string		RelativeURL;
		/// <summary>ローカルキャッシュ時の格納先</summary>
		public string		LocalStorePath;
		/// <summary>ローカルキャッシュの最終更新日</summary>
		public DateTime		LastUpdateTime;

		/// <summary>進捗状況を引数として、毎フレーム呼び出される</summary>
		public Action<float>								OnProgress;
		/// <summary>ダウンロード処理が終わった際に呼び出される。キャッシュからロードされた場合は、第一引数がtrueになる</summary>
		public Action<bool /*isLoadFromCache*/, byte[]>		OnLoad;
		/// <summary>ダウンロード処理がエラー終了した際に呼び出される</summary>
		public Action<string>								OnError;
	}


	private delegate string GetHashDelegate( string s );
	private delegate IEnumerator DownloadDelegate( DownloadSetting p );


	private class DownloadSetting : Params {
		public DownloadDelegate		DownloadProc;


		public DownloadSetting( string relativeURL, string localStorePath, DateTime date, DownloadDelegate dlDelegate ){
			this.RelativeURL = relativeURL;
			LocalStorePath = localStorePath;
			LastUpdateTime = date;
			DownloadProc = dlDelegate;
		}

		public string UrlHash {
			get {
				return this.RelativeURL.ToHashKey();
			}
		}

		public string CacheRelativePath {
			get {
				return LocalStorePath + "/" + UrlHash;
			}
		}
	}


	private URLBuilder		urlBuilder;
	private string			localCachePath;




	/// <summary>
	/// ダウンロード処理クラスを生成する
	/// </summary>
	public static WWWDownloader Create( URLBuilder urlBuilder, string localCachePath ){
		return new WWWDownloader( urlBuilder, localCachePath );
	}


	private WWWDownloader(){}

	private WWWDownloader( URLBuilder urlBuilder, string localCachePath ){
		this.urlBuilder = urlBuilder;
		this.localCachePath = localCachePath;
	}


	/// <summary>
	/// ダウンロードのみ行う通信を行う為のデータクラスを生成する
	/// </summary>
	public Params CreateParamsAsDownloadOnly( string relativeURL ){
		return new DownloadSetting( relativeURL, null, DateTime.Now, DownloadOnly );
	}


	/// <summary>
	/// ダウンロード＆ローカル保存を行う通信を行う為のデータクラスを生成する
	/// </summary>
	public Params CreateParamsAsDownloadAndCache( string relativeURL, string localStorePath, DateTime date ){
		return new DownloadSetting( relativeURL, localStorePath, date, DownloadAndCache );
	}


	/// <summary>
	/// Paramsの設定に応じた通信処理を行う。
	/// MonoBehaviour.StartCoroutineメソッドを使って呼び出す事。
	/// </summary>
	public IEnumerator Download( Params p ){
		var info = p as DownloadSetting;

		if( info == null ){
			Debug.LogError( "### Download params is invalid!!! ###" );
			yield break;
		}

		Debug.Log( "Download start: " + p.RelativeURL );
		yield return info.DownloadProc( info );
	}


	private IEnumerator DownloadOnly( DownloadSetting setting ){
		yield return DownloadProcess( setting, false );
	}


	private IEnumerator DownloadAndCache( DownloadSetting setting ){
		yield return DownloadProcess( setting, true );
	}


	private IEnumerator DownloadProcess( DownloadSetting setting, bool useCache ){
		string	localFilePath = localCachePath + "/" + setting.CacheRelativePath;

		if( useCache ){
			//ローカルにファイルがあるか？
			if( FileUtility.Exists( localFilePath ) ){
				// キャッシュファイルと時刻比較して同じならそのファイルを読み込む
				if( FileUtility.IsSameLastUpdateTime( localFilePath, setting.LastUpdateTime ) ){
					var assets = FileUtility.ReadFromFileWith3DES( localFilePath );
					if( assets != null ){
						if( setting.OnProgress != null ) setting.OnProgress( 1.0f );
						if( setting.OnLoad != null ) setting.OnLoad( true, assets );
						yield break;
					}
				} else{
					FileUtility.Delete( localFilePath );
				}
			}
		}

		WWW www = new WWW( urlBuilder( setting.RelativeURL ) );
		while( !www.isDone ){
			if( setting.OnProgress != null ) setting.OnProgress( www.progress );
			yield return null;
		}

		//失敗
		if( ( www == null ) || ( www.error != null ) ){
			if( setting.OnError != null ) setting.OnError( ( www == null ) ? "" : www.error );

		//成功
		} else{
			if( setting.OnProgress != null ) setting.OnProgress( 1.0f );
			if( setting.OnLoad != null ) setting.OnLoad( false, www.bytes );

			if( useCache ){
				//キャッシュ保存
				FileUtility.WriteToFileWith3DES( www.bytes, localFilePath );
				FileUtility.Touch( localFilePath, setting.LastUpdateTime );
			}
		}

		if( www != null ) www.Dispose();
	}

}
