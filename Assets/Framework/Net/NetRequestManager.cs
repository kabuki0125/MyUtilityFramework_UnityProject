using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Net;

/// <summary>
/// 通信コマンド　ゲームサーバへの通信要求管理.
/// </summary>
public class NetRequestManager : MonoBehaviour 
{

	/// <summary>
	/// 共通インスタンス
	/// </summary>
	public static NetRequestManager SharedInstance { get ; private set ; }

	void Awake()
	{
		if(SharedInstance != null){
			Debug.LogError("Error!! NetRequestManager Awake : SharedInstance already exist!");
			return;
		}
		SharedInstance = this ;
	}
	void OnDestroy()
	{
		SharedInstance = null ;
	}

	void Update()
	{
		// １つづつ通信コマンドを実行していく.
		if( m_bRequesting ){
			return;
		}
		if( m_requestQueue.Count <= 0 ){
			return;
		}
		
		// TODO : View側のローティング演出やボタンロックなどがあればこのタイミングでロックしとくのが無難？
		m_bRequesting = true;
		var info = m_requestQueue[0];
		if( info.IsDisplayLoading ){
			// TODO : ローディング表示があれば開始
		}
		this.Connect(info);
    }
    
    /// <summary>
    /// リクエストをキューに貯める
    /// </summary>
    public void RequestToServer(ReqApiBase req)
    {
		m_requestQueue.Add(new NetRequestInfo(req, m_nowOrderID++, req.IsDisplayLoading)) ;
	}

	private void Connect(NetRequestInfo info)
	{
		Debug.Log("[NetRequestManager] Request : " + info.Request.Command.ToString() + ", orderID = " + info.OrderID);
		
		ReqApiBase api = info.Request;
		api.player_id  = 0;		// TODO : どこかに保持しておき代入.
		api.debug = 0;          // TODO : 必要ならば特定条件で 1 に設定するようにする.
		
		if( api.IsNotReady ){
			throw new NotImplementedException();// 未実装
		}
		string serverURL = "google.co.jp";		// TODO : API呼び出し用サーバーURL. 
		if(string.IsNullOrEmpty(serverURL)){
			Debug.LogError("[NetRequestManager] Request Error!! : Server URL is null or empty.");
			return;
		}
		var co = NetConnectManager.SharedInstance.StartConnectPost(NET_TAG,
		                                                    	   DownloadDataType.NetCommand.GetNetPriority(),
		                                                    	   serverURL + api.URN,
		                                                    	   System.Text.Encoding.UTF8.GetBytes(api.ToPostDataWithHash()),
		                                                    	   DownloadDataType.NetCommand.ToString());
		co.DidLoadDelegate = ConnectFinished(info);
	}

	// 通信終了時処理
	private Action<NetConnector> ConnectFinished(NetRequestInfo info)
	{
        return delegate(NetConnector con) {
			m_requestQueue.RemoveAll(i => i.OrderID == info.OrderID);
			// TODO : View側のローティング演出やボタンロックなどがあればこのタイミングで解除しとくのが無難？

			var response = NetResponse.Create(info.Request.Command, con);
			if( response.IsConnectError ){
				// TODO : 通信エラー処理.
				return;
			}
			// TODO : 通信成功処理.他にもキャッシュ更新などを実装する.
			if(response.IsConnectError && !response.IsErrorResultCode){
				info.Request.DidLoad(response);
			}
			m_bRequesting = false;
        };
    }

	private bool m_bRequesting = false;
	private int m_nowOrderID = 0;
	private List<NetRequestInfo> m_requestQueue = new List<NetRequestInfo>();

	private static readonly string NET_TAG = "NetRequestManager";	// TODO : 本実装時はサーバー担当者とすり合わせる.

	/// <summary>
	/// 内部クラス：リクエスト情報
	/// </summary>
	class NetRequestInfo
	{
		public ReqApiBase       Request          { get ; private set ; }
		/// <summary>リクエストされた順番</summary>
		public int              OrderID          { get ; private set ; }
		/// <summary>ローディング中表示をするか？</summary>
		public bool             IsDisplayLoading { get ; private set ; }
		
		public NetRequestInfo(ReqApiBase req, int order, bool bDisplayLoading)
        {
            Request = req ;
            OrderID = order ;
            IsDisplayLoading = bDisplayLoading ;
		}
	}
}
