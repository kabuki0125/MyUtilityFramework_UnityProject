using UnityEngine;
using System;
using System.Collections;

namespace Net
{
	/// <summary>
	/// クラス：ServerErrorCode 拡張
	/// </summary>
	public partial class ServerResultCodeHelper
	{

		/// <summary>
		/// ユニークな値から Enum 値を返す.
		/// </summary>
		public static ServerResultCode ToResult(int res)
		{
			for ( int i = 0, len = HEX_CODE_TBL.Length ; i < len ; i ++ ) {
				if ( HEX_CODE_TBL[i] == (uint)res ) {
					return (ServerResultCode)Enum.Parse(typeof(ServerResultCode), i.ToString()) ;
				}
			}
			return ServerResultCode.ERR_FATAL ;
		}

		/// <summary>
		/// 対応する値を返す
		/// </summary>
		public static uint ToHexCode(this ServerResultCode e)
		{
			return HEX_CODE_TBL[(int)e] ;
		}

		// TODO : ServerResultCode.csを自動出力にして、partial分けしたこのヘルパーでゲッターなどを調整していく.必要に応じて処理を追加.
	}
}
