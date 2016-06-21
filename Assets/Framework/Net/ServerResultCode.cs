// TODO : エクセルなんなりで自動出力にしたい.

using System;
using System.Collections;

namespace Net 
{

	/// <summary>
	/// サーバ結果コード
	/// </summary>
	public enum ServerResultCode
	{
		SUCESS, 
		FAILURE,
		ERR_FATAL,
		// TODO : サーバー側で考えられる通信結果の列挙を記載していく.
	}

	/// <summary>
	/// ServerError ヘルパ
	/// </summary>
	public static partial class ServerResultCodeHelper
	{
		private static readonly uint[] HEX_CODE_TBL = {
			0x1000000,
			0xf000000,
			// TOOD : サーバー側で考えられるエラーコードを記載していく.
		};

		// TODO : 他、各エラー時の挙動(コンティニュー、リブートなど), ローカライズキーなどの列挙をを記載していく.
	}
}
