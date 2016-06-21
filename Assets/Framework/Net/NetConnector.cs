using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Net;
using UniRx;

/// <summary>
/// クラス：通信一つあたりの実行単位クラス.
/// </summary>
public class NetConnector : MonoBehaviour, IDisposable
{
	/// <param>通信終了時に呼ばれるイベント</param>
	public event Action<NetConnector> DidLoadEvent;

	/// <param>アクセス情報</param>
	public NetConnectInfo Info { get; protected set; }

	/// <param>通信タイムアウト時間 [s]</param>
	public int TimeoutDuration { get; set; }

	public WWW WWW { get; protected set; }

	/// <param>タイムアウトした？</param>
	public bool IsTimeout { get; private set; }

	/// <param>接続中？</param>
	public bool IsConnecting { get; private set; }

	/// <param>エラー？</param>
	public bool IsError { get; private set; }

	/// <summary>通信取得データ</summary>
	public byte[] ResponseData 
	{ 
		get {
			if(this.WWW == null){
				return null;
			}
			return this.WWW.bytes;
		}
	}

	/// <summary>
	/// 解放処理.このクラスを使用するなら使用後に必ず呼び出すこと！
	/// </summary>
	public void Dispose()
	{
		if(this.WWW != null){
			this.WWW.Dispose();
			this.WWW = null;
		}
		this.DidLoadEvent = null;
		GameObject.Destroy(this.gameObject);
	}

	/// <summary>
	/// 通信を開始する. (GET)
	/// </summary>
	/// <param name="info">通信情報</param>
	public void StartConnectGet(NetConnectInfo info)
	{
		Debug.Log("[NetConnector] Start Connect GET : " + info.Url);
		if(info == null){
			Debug.LogError("[NetConnector] Error!! Start Connect GET : NetConnectInfo is null.");
			return;
		}
		if(info.Url == null){
			Debug.LogError("[NetConnector] Error!! Start Connect GET : NetConnectInfo URL is null.");
			return;
		}
		this.StopAllCoroutines();
		this.Info = info;
		this.TimeoutDuration = info.TimeoutDuration;
		this.gameObject.name = info.Url;
		this.IsTimeout = false;
		
		if( this.WWW != null ){
			this.WWW.Dispose();
		}
		this.WWW = new WWW(this.Info.Url);
		this.IsConnecting = true;
		this.StartCoroutine("ProcDownload");
	}

	/// <summary>
	/// 通信を開始する. (HTTP ヘッダー付き POST)
	/// </summary>
	/// <param name="info">通信情報</param>
	/// <param name="form">Http ヘッダー情報</param>
	public void StartConnectPost(NetConnectInfo info,Dictionary<string, string> headers)
	{
		Debug.Log("[NetConnector] Start Connect POST : " + info.Url);
		if(info == null){
			Debug.LogError("[NetConnector] Error!! Start Connect POST : NetConnectInfo is null.");
			return;
		}
		if(info.Url == null || !info.IsPost){
			Debug.LogError("[NetConnector] Error!! Start Connect POST : URL is null. Or this connect is not post.");
			return;
		}
		this.StopAllCoroutines();
		this.Info = info;
		this.TimeoutDuration = info.TimeoutDuration;
		this.gameObject.name = info.Url;
		this.IsTimeout = false;
		
		if(this.WWW != null){
			this.WWW.Dispose();
		}
		if(headers != null && headers.Count > 0){
			this.WWW = new WWW(info.Url,info.PostData, headers);
		}else{
			this.WWW = new WWW(info.Url,info.PostData);
		}
		m_prevProgress = 0f;
		this.IsConnecting = true;
		this.StartCoroutine("ProcDownload");
	}

	/// <summary>
	/// 通信を開始する. (POST)
	/// </summary>
	/// <param name="info">通信情報</param>
	public void StartConnectPost(NetConnectInfo info)
	{
		this.StartConnectPost(info,null);
	}

	// ダウンロード処理
	private IEnumerator ProcDownload()
	{
		if(!this.IsConnecting){
			Debug.LogError("[NetConnector] Error!! ProcDownload : Connecting not ready. IsConnecting flag is false.");
			yield break;
		}
		while( !this.WWW.isDone && this.WWW.error == null ){
			this.CheckTimeOutTimer(this.WWW.progress);
			yield return null;
		}
		this.CancelInvoke("ProcTimeOut");
		
		Debug.Log("[NetConnector] ProcDownload : IsError = " + this.IsError.ToString() + (this.IsError ? "" : ", data size = " + this.WWW.bytes.Length.ToString()));
#if UNITY_EDITOR
		StringBuilder headerStr = new StringBuilder(">------- RESPONSE HEADER ----------\n");
		foreach(var pair in this.WWW.responseHeaders){
			headerStr.Append(pair.Key + " : " + pair.Value + "\n");
		}
		Debug.Log(headerStr.ToString());
		if( !this.IsError ){
			// TODO : デバッグ用：ファイルに残しとく
		}
#endif
		this.IsConnecting = false;
		if( this.DidLoadEvent != null ){
			this.DidLoadEvent(this);
		}
    }
    
    /// <summary>
	/// 通信を停止する.
	/// </summary>
	public void StopConnect()
	{
		if( this.IsConnecting ){
			if( this.DidLoadEvent != null ){
				this.DidLoadEvent(this);
            }
        }
		this.Dispose();
		this.IsConnecting = false;
    }
    
    // タイムアウトチェック
    private void CheckTimeOutTimer(float progress)
	{
		if( progress >= 1f ){
			this.CancelInvoke("ProcTimeOut");
			m_prevProgress = 1f;
			return;
		}
		
		if( progress != m_prevProgress ){
			this.CancelInvoke("ProcTimeOut");
			this.Invoke("ProcTimeOut", this.TimeoutDuration);
		}
		m_prevProgress = progress;
	}
	private void ProcTimeOut()
	{
		Debug.Log("[ProcDownload] Time out!! : " + (this.Info != null ? this.Info.Url : ""));
		this.IsTimeout = true;
		this.StopCoroutine("ProcDownload");
		this.StopConnect();
	}

	private float m_prevProgress = 0f;
}