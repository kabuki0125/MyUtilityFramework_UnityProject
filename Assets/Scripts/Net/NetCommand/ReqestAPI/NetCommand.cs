// TODO : エクセルなんなりで自動出力にしたい.
namespace Net
{

	using System;
	using UnityEngine;
	using System.Collections;

	/// <summary>
	/// enum：通信コマンド一覧.
	/// </summary>
	public enum NetCommand
	{
		REQ_API_LOGIN_PLAYER,
	}

	/// <summary>
	/// クラス：通信コマンド.
	/// </summary>
	public static class NetCommandExtension
	{
		/// <summary>
		/// 対応するリクエストオブジェクトを返す.
		/// </summary>
		public static ReqApiBase ToRequest(this NetCommand cmd) 
		{
			ReqApiBase req = (ReqApiBase)Activator.CreateInstance(API_LIST[(int)cmd]) ;
			req.SetCommand(cmd) ;
			return req ;
		}
		private static readonly Type[] API_LIST = new Type[] {
			typeof(ReqApiLoginPlayer),
		};
	}
	
}