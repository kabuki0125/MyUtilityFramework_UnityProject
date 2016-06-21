// TODO : エクセルなんなりで自動出力にしたい.

using System;

namespace Net
{
	public enum ServerClassCode 	{
		RocVersion = 1,	/// バージョン情報の取
		// TODO : サーバーに通信結果用のデータクラス(Roc〜)を作ってもらったらここに追加.
	}

	/// <summary>
	/// ServerClassCode ヘルパー
	/// </summary>
	public static class ServerClassCodeHelper
	{
		/// <summary>
		/// 対応する ResultObject インスタンスを生成する
		/// </summary>
		public static IMsgPack CreateInstance(this ServerClassCode code)
		{
			switch(code){
			case ServerClassCode.RocVersion:
				return new RocResultCode();		// TODO :→ new RocVersion
			default:
				return null;
			}
		}

		/// <summary>
		/// 数値から変換
		/// </summary>
		public static ServerClassCode Parse(int code)
		{
			return (ServerClassCode)Enum.Parse(typeof(ServerClassCode), code.ToString());
		}
	}
}