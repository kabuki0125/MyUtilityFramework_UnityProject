using UnityEngine;
using System.Collections;


namespace Net
{

	/// <summary>
	/// enum : サーバー名
	/// </summary>
	public enum ServerName
	{
		Develop,
		Staging,
		Master, 
		// TODO : 必要に応じて追加していく.
	}

	/// <summary>
	/// クラス：サーバー名補完・ヘルパークラス
	/// </summary>
	public static class ServerNameHelper
	{

		/// <summary>
		/// サーバ URL を返す.
		/// </summary>
		public static string ToURL(this ServerName s)
		{
			return URL_LIST[(int)s];
		}
		private static readonly string[] URL_LIST = {
			"http://www.google.co.jp",  // Develop
			"http://www.google.co.jp",  // Staging
			"http://www.google.co.jp",  // Master
			// TODO : 必要に応じて追加していく.
		};

	}

}
