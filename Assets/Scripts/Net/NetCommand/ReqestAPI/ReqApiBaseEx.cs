
namespace Net
{

	using UnityEngine;
	using System.Collections;

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
	}
}
