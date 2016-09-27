using UnityEngine;
using System;
using System.Collections;


// マスターデータに対する処理を行いたい場合はここに記述する。

namespace JsonDataParser {

	public partial class MasterDataParser {

		/// <summary>データ復号化の為のKeyを得る</summary>
		public static byte[] GetDecryptKey(){ return HexStringToByteArray( MasterDataParser.Decrypt3DES_Key ); } 
		
		/// <summary>データ復号化の為のIVを得る</summary>
		public static byte[] GetDecryptIV(){ return HexStringToByteArray( MasterDataParser.Decrypt3DES_IV ); }

		/// <summary>暗号化判定用文字列</summary>
		private static readonly string		CryptedSign = "43525950544544";




		public static string ConvertString( byte[] data ){
			int signLength = CryptedSign.Length;
			string signHex = FileUtility.ConvertUTF8String( data, 0, signLength );

			// 暗号化されているデータか？
			if( signHex == CryptedSign ){
				byte[] encryptData = MasterDataParser.HexStringToByteArray( FileUtility.ConvertUTF8String( data, signLength ) );
				return FileUtility.Decrypt3DESToString( GetDecryptKey(), GetDecryptIV(), encryptData );
			} else{
				return FileUtility.ConvertUTF8String( data );
			}
		}


		// １６進数値を表す文字列をバイト列に変換する
		public static byte[] HexStringToByteArray( string strHex ){
			var	r = new byte[strHex.Length / 2];
			for( int i = 0; i <= strHex.Length - 1; i += 2 ){
				r[i / 2] = Convert.ToByte( Convert.ToInt32( strHex.Substring( i, 2 ), 16 ) );
			}
			return r;
		}


 		#region サンプル メソッド (不要なら削除可)

		/// <summary>
		/// 指定ワールド内のステージリストを得る(リストはID昇順にソートされる)
		/// </summary>
		public Json_Stage[] GetStagesInWorld( int worldID ){
			var list = Array.FindAll<Json_Stage>( Stage, i => i.worldId == worldID );
			Array.Sort<Json_Stage>( list, ( a, b ) => a.id - b.id );
			return list;
		}

		#endregion

	}

}

