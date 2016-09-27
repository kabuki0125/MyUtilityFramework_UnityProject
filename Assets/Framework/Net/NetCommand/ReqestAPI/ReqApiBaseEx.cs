using UnityEngine;
using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace Net
{
	/// <summary>
	/// クラス：通信リクエストAPIの基底クラス.
	/// </summary>
	public abstract partial class ReqApiBase
	{

		/// <summary>
		/// 対応する通信コマンド
		/// </summary>
		public NetCommand   Command { get ; private set ; }
		
		/// <summary>
		/// TODO : アクセス先URL パス部分.サーバー担当者と相談して決める.
		/// </summary>
		public string URN
		{
			get { return "/game/api/" + ApiName + ".php" ; }
		}

		/// <summary>
		/// ローディング表示をするか？
		/// </summary>
		public bool IsDisplayLoading = true;

		/// <summary>
		/// 通信中か？
		/// </summary>
		public bool         IsConnecting { get ; private set ; }

		/// <summary>
		/// なにかしらのエラーが発生したか？
		/// </summary>
		public bool         IsError      { get ; private set ; }

		/// <summary>
		/// 通信結果情報
		/// </summary>
		public NetResponse  Response     { get ; private set ; }
        
        
        public void SetCommand(NetCommand cmd)
		{
			Command = cmd ;
		}

		/// <summary>
		/// ゲームサーバと通信を開始する.
		/// </summary>
		public void ConnectToServer(Action<NetResponse> didLoad = null)
		{
			m_didLoad    = didLoad;
			IsConnecting = true;
			IsError      = false;
//			this.SetVersions();
			NetRequestManager.SharedInstance.RequestToServer(this);
		}

		/// <summary>
		/// NetRequestManager 処理用： 通信終了時処理
		/// </summary>
		public void DidLoad(NetResponse response)
		{
			Response = response;
			IsError = Response.IsError;
#if _ENABLE_NET_DEBUG_
			if(Response.ResultBase != null && Response.ResultBase.NewSessionId != 0){
				m_lastEnabledSessionId = Response.ResultBase.NewSessionId;
			}
#endif
			
			IsConnecting = false;
			if(this.m_didLoad != null){
				this.m_didLoad(Response);
            }
        }
        
        /// <summary>
        /// NetRequestManager 処理用： POST データをハッシュ値文字列で返す.
        /// </summary>
        public string ToPostDataWithHash()
        {
#if _ENABLE_NET_DEBUG_
			if ( this.debug != 0 ) {
				return this.ToPostData() + "&secret_key=1d050924e71c34b689879a1cc39dcd02" ;// ダミーリクエスト時は、チェック処理通らないシークレットキーで。
			}
#endif
			string post = this.ToPostData();
			string data = Regex.Replace(post, @"(&session_id=\w+)&", @"&");
			string sid  = GetSessionIDQuery();
			Debug.Log(data/*.ToLower()*/ + "&" + sid);
			
			string secretKey = "&secret_key=" + ToMD5HashString(data/*.ToLower()*/ + "&" + sid + "DE18AE7DFE417D51791066DCE16FACBF");
			
			return data + secretKey;
		}
		private static string GetSessionIDQuery()
		{
			// 前回の通信コマンド実行時に取得したセッションIDを使う.
			string sid = "session_id=";
			// TODO : セッションIDの更新処理
#if _ENABLE_NET_DEBUG_
			if( 今回の通信セッションID == 0 ){
				sid += m_lastEnabledSessionId;
			}else{
				sid += NetCache.SharedInstance.LastResultBase.NewSessionId ;
			}
#else
			sid += m_lastEnabledSessionId;
#endif
			return sid;
		}
		private static string ToMD5HashString(string source)
		{
			source = WWW.UnEscapeURL(source);
			
			byte[] hashKey = null;
			using(var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider()){
				hashKey = md5.ComputeHash(Encoding.UTF8.GetBytes(source));
			}
			
			var builder = new StringBuilder();
			for (int i = 0; i < hashKey.Length; ++i){
				builder.Append(hashKey[i].ToString("x2"));
            }
            return builder.ToString();
        }

		private Action<NetResponse> m_didLoad;

		private static int m_lastEnabledSessionId;    // 最後に受信した有効なセッションID
    }
}
