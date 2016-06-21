// TODO : エクセルなんなりで自動出力にしたい.
namespace Net
{

	using UnityEngine;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Text ;

	/// <summary>
	/// クラス：通信リクエストAPIの基底クラス.
	/// </summary>
	public abstract partial class ReqApiBase
	{
		/// <summary>通信APIの名前.phpと同じ名前にしとく.</summary>
		public abstract string ApiName { get; }

		/// <summary>作成中のAPIはこのフラグが立っている.</summary>
		public abstract bool IsNotReady { get; }

#region 通信時、基底側で処理を行うものたち
		/// デバッグフラグ.1ならデバッグモードみたいにしておく.
		public int debug { get ; set ; }
		/// プレイヤーのID.
		public int player_id { get ; set ; }
		/// ローカル保存のマスターデータバージョン.
		public uint param_other_version_id { get ; set ; }
		/// アセットバンドルバージョン.
		public uint abundle_parts_version_id { get ; set ; }
		/// ローカライズ文言のバージョン.外部出力にするので.
		public uint localize_version_id { get ; set ; }
		/// クライアント側のバージョン.
		public string client_version_id { get ; set ; }
		/// 0ならスマフォ、１ならPCなどとしておく.
		public int os { get ; set ; }
#endregion

#region 型チェッカー
		public virtual bool IsValid() { 
			if(!IsValid_int(debug, false)){ return false; }
			if(!IsValid_int(player_id, true)){ return false; }
			if(!IsValid_uint(param_other_version_id, true)){ return false; }
			if(!IsValid_uint(abundle_parts_version_id, true)){ return false; }
			if(!IsValid_uint(localize_version_id, true)){ return false; }
			if(!IsValid_string(client_version_id)){ return false; }
			if(!IsValid_int(os, true, 1, 2)){ return false; }
			return true;
		} 

		protected bool IsValid_string(string str) { return String.IsNullOrEmpty(str) ? false : true ; }	
		protected bool IsValid_sbyte (sbyte  value, bool bRequired, sbyte  min = sbyte.MinValue,  sbyte  max = sbyte.MaxValue) { return value >= min && value <= max; }
		protected bool IsValid_byte  (byte value, bool bRequired, byte   min = byte.MinValue,   byte max = byte.MaxValue) { return value >= min && value <= max; }
		protected bool IsValid_char  (char value, bool bRequired, char   min = char.MinValue,   char max = char.MaxValue) { return value >= min && value <= max; }
		protected bool IsValid_short (short  value, bool bRequired, short  min = short.MinValue,  short  max = short.MaxValue) { return value >= min && value <= max; }
		protected bool IsValid_ushort(ushort value, bool bRequired, ushort min = ushort.MinValue, ushort max = ushort.MaxValue) { return value >= min && value <= max; }
		protected bool IsValid_int (int value, bool bRequired, int    min = int.MinValue,  int max = int.MaxValue) { return value >= min && value <= max; }
		protected bool IsValid_uint  (uint value, bool bRequired, uint   min = uint.MinValue,   uint max = uint.MaxValue){ return value >= min && value <= max; }
		protected bool IsValid_long  (long value, bool bRequired, long   min = long.MinValue,   long max = long.MaxValue) { return value >= min && value <= max; }
		protected bool IsValid_ulong (ulong  value, bool bRequired, ulong  min = ulong.MinValue,  ulong  max = ulong.MaxValue) { return value >= min && value <= max; }
		protected bool IsValid_float (float  value, bool bRequired, float  min = float.MinValue,  float  max = float.MaxValue) { return value >= min && value <= max; }
#endregion

		public virtual string ToPostData() 
		{ 
			StringBuilder builder = new StringBuilder() ;
			builder.Append("debug=" + debug);
			builder.Append("&player_id=" + player_id);
			builder.Append("&param_other_version_id=" + param_other_version_id);
			builder.Append("&abundle_parts_version_id=" + abundle_parts_version_id);
			builder.Append("&localize_version_id=" + localize_version_id);
			builder.Append("&client_version_id=" + client_version_id);
			builder.Append("&os=" + os);
			return builder.ToString();
		} 

		public virtual object[] Pack() 
		{ 
			List<object> obj = new List<object>();
			obj.Add(debug);
			obj.Add(player_id);
			obj.Add(param_other_version_id);
			obj.Add(abundle_parts_version_id);
			obj.Add(localize_version_id);
			obj.Add(client_version_id);
			obj.Add(os);
			return obj.ToArray();
		} 
		
		public virtual void Unpack(object[] obj, ref int count) 
		{ 
			debug = Convert.ToInt32(obj[count ++]);
			player_id = Convert.ToInt32(obj[count ++]);
			param_other_version_id = Convert.ToUInt32(obj[count ++]);
			abundle_parts_version_id = Convert.ToUInt32(obj[count ++]);
			localize_version_id = Convert.ToUInt32(obj[count ++]);
			client_version_id = Encoding.UTF8.GetString(obj[count ++] as byte[]);
			os = Convert.ToInt32(obj[count ++]);
		} 
	}

}