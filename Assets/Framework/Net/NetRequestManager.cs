
namespace Net
{

	using System;
	using UnityEngine;
	using System.Collections;

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
			co.DidLoadDelegate = delegate(NetConnector obj) {};	// TODO : レスポンス周りの機能を実装したらちゃんと実装.
		}

		private static readonly string NET_TAG = "NetRequestManager";	// TODO : 本実装時はサーバー担当者とすり合わせる.
	}

	/// <summary>
	/// リクエスト情報
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
