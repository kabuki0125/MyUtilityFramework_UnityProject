using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Ionic.Zlib;
using MsgPack;

namespace Net
{
	/// <summary>
	/// クラス：通信結果.
	/// </summary>
	public class NetResponse
	{

		/// <summary>
		/// 対応する通信コマンド
		/// </summary>
		public NetCommand Command { get ; private set ; }

		/// <summary>
		/// エラー？
		/// </summary>
		public bool IsError
		{
			get {
				return this.IsConnectError || this.IsServerError;
			}
		}
		/// <summary>
		/// 通信接続エラー？
		/// </summary>
		public bool IsConnectError { get; private set; }
		/// <summary>
		/// サーバ結果エラー？
		/// </summary>
		public bool IsServerError { get; private set; }

		/// <summary>
		/// プロパティ：結果ベース情報. IsConnectError = true のときは null
		/// </summary>
		public RocResultCode ResultBase  { get; private set; }

		/// <summary>
		/// サーバ結果コード
		/// </summary>
		public ServerResultCode ResultCode
		{
			get {
				if( this.ResultBase == null ){
					return ServerResultCode.FAILURE;
				}
				return ServerResultCodeHelper.ToResult(this.ResultBase.ResultCode);
			}
		}

		/// <summary>
		/// サーバ結果コードが失敗か？
		/// </summary>
		public bool IsErrorResultCode {
			get {
				if ( ResultBase == null ) { return true ; }
				uint mask = ServerResultCodeHelper.ToHexCode(ServerResultCode.FAILURE) ;
				return (this.ResultBase.ResultCode & mask) == mask ;
			}
		}

		/// <summary>
		/// プロパティ：サーバ結果 ResultObject. IsConnectError = true のときは null
		/// </summary>
		public List<KeyValuePair<ServerClassCode,IMsgPack>>   ResultObjects { get; private set; }

		/// <summary>
		/// プロパティ：サーバ結果 ResultObject のバイト列. IsError = false のときのみ取得できる。
		/// </summary>
		public byte[] ResultObjectBytes { get; private set; }


		/// <summary>
		/// インスタンス生成：通信情報から生成
		/// </summary>
		public static NetResponse Create(NetCommand cmd, NetConnector con)
		{
			var instance = new NetResponse();
			instance.Command = cmd;
			if( con == null ){
				instance.IsConnectError = true;
				return instance;
			}
			Debug.Log("[NetResponse] IsError = " + con.IsError.ToString() + (con.IsError ? "" : ", data size = " + con.WWW.bytes.Length.ToString()));
			if( con.IsError ){
				instance.IsConnectError = true;
				return instance;
			}
			
			var result = instance.UnpackObject(con.ResponseData);
			if( result == null ){
				Debug.LogError("[NetResponse] Parse Error!! : " + cmd.ToString());
				return instance;
			}
			
			instance.ParseResultBase(result);
			// TODO : 結果をDumpしたいなどあれば追記.
			instance.ResultObjects = ParseResultObjects(result);
			return instance;
		}

		// 通信結果解凍.サーバーからのレスポンスはzip圧縮されて返ってくる想定.
		private System.Object[] UnpackObject(byte[] data)
		{
			if( data == null || data.Length <= 0 ){
				this.IsServerError = true;
				return null;
			}
			for (int i = 0; i < data.Length; ++i){
				data[i] = (byte)(data[i] ^ 0x9A);
			}
			this.ResultObjectBytes = ZlibStream.UncompressBuffer(data);		// 解凍にはIonic.Zlib.dllを使用.
			
			var packer = new BoxingPacker();
			var result = packer.Unpack(this.ResultObjectBytes) as System.Object[];	// IMsgPackのデシリアライズ
			if( result == null ){
				this.IsServerError = true;
			}
			return result;
		}
		// 通信結果解析.通信結果として返ってきたクラス(Roc〜)を特定する.
		private static List<KeyValuePair<ServerClassCode, IMsgPack>> ParseResultObjects(System.Object[] result)
		{
			var resFullCache = new List<KeyValuePair<ServerClassCode, IMsgPack>>();
			if( result.Length <= 1 ){
				return resFullCache;// RocResultCode しかない場合
			}
			
			ServerClassCode classCode;
			System.Object[] objList;
			for (int i = 1; i < result.Length; i++){
				IMsgPack ro = null;
				try{
					objList = (System.Object[])result[i];
					classCode = ServerClassCodeHelper.Parse( Convert.ToInt32(objList[0]) );	// 通信結果クラスをenumで特定して、
					ro = classCode.CreateInstance(); 	// 特定したenumを基にクラスを生成.
				}catch{
					Debug.LogError("[NetResponse] Parse Error!! : class code");
					continue;
				}
				if( ro == null ){
					Debug.LogError("[NetResponse] Parse Error!! : source = " + Convert.ToInt32(objList[0]).ToString() + ", class code = " + classCode.ToString());
					continue;
				}
				
				try{
					ro.Parse(objList);	// ここで中身の解析.
				}catch{
					Debug.LogError("[NetResponse] Parse Error!! : " + classCode.ToString());
					continue;
				}
				
				var res = new KeyValuePair<ServerClassCode,IMsgPack>(classCode,ro);
				resFullCache.Add(res);
				Debug.Log("Parse RO : " + classCode.ToString());
			}
			
			return resFullCache;
		}

		private void ParseResultBase(System.Object[] result)
		{
			this.ResultBase = new RocResultCode();
			this.ResultBase.Parse((System.Object[])result[0]);
			this.IsServerError = this.IsErrorResultCode;
		}
	}
}
