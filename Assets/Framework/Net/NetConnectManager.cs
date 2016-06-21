using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Net;
using Extensions;


/// <summary>
/// クラス：通信管理クラス
/// </summary>
public class NetConnectManager : MonoBehaviour
{
	/// <summary>接続前イベント</summary>
	public event Action<NetConnectInfo> WillStart;
	/// <summary>接続後イベント</summary>
	public event DidEndDelegate DidEnd;
	/// <summary>
	/// 通信終了時に呼ばれるデリゲート.
	/// </summary>
	/// <param name="con">通信終了した通信</param>
	/// <param name="didEndDelegate">内部で処理が完了したら呼び出すこと. （つまりは、複数のフレームにまたがって処理をすることも可能.）</param>
	public delegate void DidEndDelegate(NetConnector con,Action didEndDelegate);

	/// <summary>共通インスタンス</summary>
	public static NetConnectManager SharedInstance { get; private set; }

	/// <param>
	/// 通信共通で使われる HTTP ヘッダー
	/// </param>
	public readonly Dictionary<string, string> HttpHeaders = new Dictionary<string, string>();

	/// <summary>
	/// 最大同時接続数
	/// </summary>
	public int MAX_CONNET_COUNT = 3;

	/// <summary>
	/// 通信を開始する. (POST)
	/// </summary>
	/// <param name="tag">管理用タグ名. 削除時に指定する.</param>
	/// <param name="priority">通信優先度. 数字が大きいほど優先度が上.</param>
	/// <param name="url">通信先 URL</param>
	/// <param name="data">POST データ</param>
	/// <param name="dataType">ダウンロードデータ種類</param>
	public NetConnectInfo StartConnectPost(string tag,int priority, string url,byte[] data,string dataType)
	{
		var info = new NetConnectInfo(url,data){ Tag = tag, Priority = priority, DataType = dataType };
		this.EnqueueConnectQueue(info);
		return info;
	}
	
	/// <summary>
	/// 通信を開始する. (GET)
	///     現状, HTTP ヘッダーの追加なしで通信します。
	/// </summary>
	/// <param name="tag">管理用タグ名. 削除時に指定する.</param>
	/// <param name="priority">通信優先度. 数字が大きいほど優先度が上.</param>
	/// <param name="url">通信先 URL</param>
	/// <param name="dataType">ダウンロードデータ種類</param>
	/// <param name="didLoad">通信終了時に呼ばれるメソッド</param>
	public NetConnectInfo StartConnectGet(string tag,int priority, string url,string dataType)
	{
		var info = new NetConnectInfo(url){ Tag = tag, Priority = priority, DataType = dataType };
		this.EnqueueConnectQueue(info);
		return info;
	}
	
	/// <summary>
	/// 通信を開始する.  MCNetConnectInfo 継承クラスをつかうときはこいつを使うべし.
	/// </summary>
	/// <param name="info">通信情報</param>
	public void StartConnect(NetConnectInfo info)
	{
		this.EnqueueConnectQueue(info);
	}

	// 通信接続リクエストキューに実行優先順にソートして追加
	private void EnqueueConnectQueue(NetConnectInfo info)
	{
		m_connectQueue.Enqueue(info);
		var list = from i in m_connectQueue.ToArray()
			orderby i.Priority descending
			select i;
		m_connectQueue.Clear();
		foreach(var i in list){
			m_connectQueue.Add(i);
		}
	}

	// 更新処理：通信待ちキュー
	private void UpdateConnectQueue()
	{
		if( m_connectQueue.Count <= 0 ){
			return;
		}
		
		if( m_canConnectCnt > 0 ){
			this.Connect(m_connectQueue.Dequeue());
			m_canConnectCnt--;
		}
	}

	// 通信接続処理
	private void Connect(NetConnectInfo info)
	{
		var go = new GameObject("connection");
		go.transform.parent = this.gameObject.transform;
		
		var con = go.AddComponent<NetConnector>();
		con.DidLoadEvent += this.DidLoad;
		
		// 通信開始
		if( this.WillStart != null ){
			this.WillStart(info);
		}
		if( info.IsPost ){
			con.StartConnectPost(info,this.HttpHeaders);
		}else{
			con.StartConnectGet(info);
		}
		if( info.DidStartDelegate != null ){
			info.DidStartDelegate(con);
		}
	}

	/// 通信処理終了時に呼ばれるメソッド
	private void DidLoad(NetConnector con)
	{
		// 通信終了イベント後を１つの通信終了とする.
		if( this.DidEnd == null ){
			this.DidLoadEndProc(con);
			con.Dispose();
		}else{
			// 通信終了イベントが設定されている場合、ながく終了デリゲートが呼ばれない場合がある。終了デリゲートが返って来るまでは次以降に来る結果を遅延しとく.
			m_endEventWaitList.Enqueue(con);
		}
	}
	private void DidLoadEndProc(NetConnector con)
	{
		if(con == null){
			Debug.LogError("[NetConnectManager] Error!! DidLoadEndProc : NetConnector is null.");
			return;
		}
		if( con.Info.DidLoadDelegate != null ){
			con.Info.DidLoadDelegate(con);
		}
		m_canConnectCnt++;
		if( m_canConnectCnt > this.MAX_CONNET_COUNT ){
			m_canConnectCnt = this.MAX_CONNET_COUNT;
		}
	}

	/// 更新処理：通信終了イベント待ちキュー
	private void UpdateEndEventWaitList()
	{
		if( m_bNowEndEventWait
		   || m_endEventWaitList.Count <= 0 ){
			return;
		}
		
		m_bNowEndEventWait = true;
		NetConnector con = m_endEventWaitList.Dequeue();
		this.DidEnd(con,delegate{
			this.DidLoadEndProc(con);
			con.Dispose();
			m_bNowEndEventWait = false;
		});
	}

	void LateUpdate()
	{
		this.UpdateEndEventWaitList();
		this.UpdateConnectQueue();
	}

	void Awake()
	{
		SharedInstance = this;
		m_canConnectCnt = this.MAX_CONNET_COUNT;
	}
	void OnDestroy()
	{
		SharedInstance = null;
	}


	private int m_canConnectCnt;	// 一度に通信できる数.0なら通信不可.
	private bool m_bNowEndEventWait = false;

	private readonly List<NetConnectInfo> m_connectQueue = new List<NetConnectInfo>();  // 通信待ちキュー
	private readonly List<NetConnector> m_endEventWaitList = new List<NetConnector>(); // 通信終了待ちキュー
}

namespace Net
{
	/// <summary>
	/// クラス：接続情報
	/// </summary>
	public class NetConnectInfo
	{
		/// <summary>管理用タグ</summary>
		public string Tag { get; set; }

		/// <summary>通信優先度. 数字が大きいほど優先度が上</summary>
		public int Priority { get; set; }

		/// <summary>アクセス先 URL</summary>
		public string Url { get; protected set; }

		/// <summary>データ種類</summary>
		public string DataType { get; set; }

		/// <summary>通信開始時に呼ばれるデリゲート</summary>
		public Action<NetConnector> DidStartDelegate { get; set; }

		/// <summary>通信完了時に呼ばれるデリゲート</summary>
		public Action<NetConnector> DidLoadDelegate  { get; set; }

		/// <summary>POST データ.  GET 送信なら null</summary>
		public byte[] PostData { get; private set; }

		/// <summary>通信タイムアウト時間 [s]</summary>
		public int TimeoutDuration = 30;

		/// <summary>
		/// POST 通信？
		/// </summary>
		public virtual bool IsPost
		{
			get { return this.PostData != null; }
		}
		
		/// <summary>
		/// URL がファイルアクセス？
		/// </summary>
		public bool IsFileAccess
		{
			get { return this.Url.IndexOf("file://") >= 0; }
		}


		/// <summary>
		/// コンストラクタ : GET 通信として
		/// </summary>
		public NetConnectInfo(string url) : this()
		{
			this.Url = url;
			this.PostData = null;
		}
		/// <summary>
		/// コンストラクタ : POST 通信として
		/// </summary>
		public NetConnectInfo(string url, byte[] postData) : this()
		{
			this.Url = url;
			this.PostData = postData;
		}
		protected NetConnectInfo()
		{
			this.Tag        = "default";
			this.Priority   = 0;
			this.DataType   = "";
		}
	}
}
