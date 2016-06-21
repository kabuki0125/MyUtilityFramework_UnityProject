using UnityEngine;
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

		public void SetCommand(NetCommand cmd)
		{
			Command = cmd ;
		}

		/// <summary>
		/// NetRequestManager 処理用： POST データをハッシュ値文字列で返す.
		/// </summary>
		public string ToPostDataWithHash()
		{
#if _NC_ENABLE_NET_DEBUG_
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
#if _NC_ENABLE_NET_DEBUG_
			if( NetCache.SharedInstance.LastResultBase.NewSessionId == 0 ){
				sid += lastEnabledSessionId;
			}else{
				sid += NetCache.SharedInstance.LastResultBase.NewSessionId ;
			}
#else
//			sid += NetCache.SharedInstance.LastResultBase.NewSessionId;
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
    }
}
