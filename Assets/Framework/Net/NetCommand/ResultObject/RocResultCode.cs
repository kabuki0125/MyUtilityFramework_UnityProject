// TODO : エクセルなんなりで自動出力にしたい.

using System;
using System.Collections;

namespace Net
{
	public class RocResultCode : IMsgPack
	{
		public int	ResultCode { get; protected set; }	/// 結果コード
		public int	PlayerId { get; protected set; }	/// 送信したPlayerID
		public int	TargetPlayerId { get; protected set; }	/// 対象のPlayerID
		public int	NewSessionId { get; protected set; }	/// セッションID
		public long	ServerTime { get; protected set; }	/// サーバー時間


		public void Parse(object[] obj)
		{
			this.ResultCode = Convert.ToInt32(obj[0]);
			this.PlayerId = Convert.ToInt32(obj[1]);
			this.TargetPlayerId = Convert.ToInt32(obj[2]);
			this.NewSessionId = Convert.ToInt32(obj[3]);
			this.ServerTime = Convert.ToInt64(obj[4]);
		}
	}
}
