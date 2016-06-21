// TODO : エクセルなんなりで自動出力にしたい.
namespace Net  
{

	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// クラス：通信リクエスト プレイヤーのログイン.
	/// </summary>
	public partial class ReqApiLoginPlayer  : ReqApiBase 
	{

		public override string ApiName 
		{ 
			get {
				return "login_player";
			}
		}
		
		public override bool IsNotReady 
		{ 
			get {
				return false;
			}
		}
		
		public string user_id { get; set; }
		public string language { get; set; }
		public string device { get; set; }
		
		public override bool IsValid() { 
			if(!base.IsValid()){ return false; }
			if(!IsValid_string(user_id)){ return false; }
			if(!IsValid_string(language)){ return false; }
			if(!IsValid_string(device)){ return false; }
			return true;
		} 
		
		public override string ToPostData() { 
			StringBuilder builder = new StringBuilder();
			builder.Append(base.ToPostData());
			if(builder.Length > 0){
				builder.Append("&");
			}
			builder.Append("user_id=" + user_id);
			builder.Append("&language=" + language);
			builder.Append("&device=" + device);
			return builder.ToString();
		} 
		
		public override object[] Pack() { 
			List<object> obj = new List<object>();
			obj.AddRange(base.Pack());
			obj.Add(user_id);
			obj.Add(language);
			obj.Add(device);
			return obj.ToArray();
		} 
		
		public override void Unpack(object[] obj, ref int count) { 
			base.Unpack(obj, ref count);
			user_id = Encoding.UTF8.GetString(obj[count ++] as byte[]);
			language = Encoding.UTF8.GetString(obj[count ++] as byte[]);
			device = Encoding.UTF8.GetString(obj[count ++] as byte[]);
		} 
		
	}
	
}

