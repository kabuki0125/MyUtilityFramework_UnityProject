// データを暗号化してローカルに保存する
#define ENCRYPT_LOCAL_FILE

using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Security.Cryptography;

using Ionic.Zlib;
using MyLibrary.Unity.IO;


	
public static class FileUtility {

	private const string	EncryptKeyFor3DES = "cN5hfmgup8smWVtdPhmTTJbRxG2WsRwJuBgtxmHXnh5FrJQ5WtZ5ViGSGnQz7UG3";



	/// <summary>ディレクトリが存在しなければ作成する</summary>
	public static void CreateDirectory( string path ){
        if( !System.IO.Directory.Exists( path ) ) System.IO.Directory.CreateDirectory( path );
	}


	/// <summary>ファイルが存在するか？</summary>
	public static bool Exists( string filePath ){
		return System.IO.File.Exists( filePath );
	}


	/// <summary>
	/// ファイルの最終更新日が指定の時刻かどうかを調べる。
	/// </summary>
	public static bool IsSameLastUpdateTime( string filePath, DateTime date ){
		DateTime lastUpdateTime = System.IO.File.GetLastWriteTime( filePath );
		return lastUpdateTime.CompareTo( date ) == 0;
	}


	/// <summary>
	/// 作成日時と更新日時を指定日時に変更する。
	/// </summary>
	public static void Touch( string filePath, DateTime time ){
		File.SetCreationTime( filePath, time );
		File.SetLastWriteTime( filePath, time );
	}


	/// <summary>
	/// ファイルを削除する。
	/// </summary>
	public static void Delete( string filePath ){
		if( Exists( filePath ) ){
			File.Delete( filePath );
		}
	}


	/// <summary>データをファイルに書き込む</summary>
	public static void WriteToFile( byte[] data, string filePath ){
		System.IO.File.WriteAllBytes( filePath, data );
	}


	/// <summary>
	/// データをTripleDESで暗号化してファイルに書き込む
	/// </summary>
	/// <param name="isPack">Zip圧縮するか？</param>
	public static void WriteToFileWith3DES( byte[] data, string filePath, bool isPack = false ){
#if ENCRYPT_LOCAL_FILE
		if( isPack ) data = PackByZip( data );
		EncryptHelper.WriteToFile( data, EncryptKeyFor3DES, filePath );
#else
		System.IO.File.WriteAllBytes( filePath, data );
#endif
	}


	/// <summary>
	/// JSONデータをTripleDESで暗号化してファイルに書き込む
	/// </summary>
	/// <param name="isPack">Zip圧縮するか？</param>
	public static void WriteToJsonFileWith3DES( Char[] jsonData, string filePath, bool isPack = false ){
		WriteToFileWith3DES( Encoding.UTF8.GetBytes( jsonData ), filePath, isPack );
	}
	public static void WriteToJsonFileWith3DES( Char[] jsonData, int offset, int length, string filePath, bool isPack = false ){
		WriteToFileWith3DES( Encoding.UTF8.GetBytes( jsonData, offset, length ), filePath, isPack );
	}


	/// <summary>ファイルからデータを読み込む</summary>
	public static byte[] ReadFromFile( string filePath ){
		if( System.IO.File.Exists( filePath ) ) return System.IO.File.ReadAllBytes( filePath );
		return null;
	}


	/// <summary>
	/// TripleDESで暗号化されたファイルを読み込んで、復号化してから返す
	/// </summary>
	/// <param name="isPacked">Zip圧縮されたファイルか？</param>
	public static byte[] ReadFromFileWith3DES( string filePath, bool isPacked = false ){
#if ENCRYPT_LOCAL_FILE
		if( isPacked ){
			byte[] packedData = EncryptHelper.ReadFromFile( EncryptKeyFor3DES, filePath );
            if (packedData != null) {
                return UnPackByZip(packedData);
            } else {
                return null;
            }
		} else{
			return EncryptHelper.ReadFromFile( EncryptKeyFor3DES, filePath );
		}
#else
		if( System.IO.File.Exists( filePath ) ) return System.IO.File.ReadAllBytes( filePath );
		return null;
#endif
	}


	/// <summary>
	/// TripleDESで暗号化されたJSONファイルを読み込んで、復号化してから返す
	/// </summary>
	/// <param name="isPacked">Zip圧縮されたファイルか？</param>
	public static Char[] ReadFromJsonFileWith3DES( string filePath, bool isPacked = false ){
		byte[] d = ReadFromFileWith3DES( filePath, isPacked );
		if( d == null ) return null;
		return Encoding.UTF8.GetChars( d );
	}


	/// <summary>
	/// TripleDESで暗号化されたバイト列を文字列として復号化する
	/// </summary>
	public static string Decrypt3DESToString( byte[] key, byte[] iv, byte[] source ){
		byte[] data = null;

		using( var memStream = new MemoryStream( source ) ){
			using( var tDes = new TripleDESCryptoServiceProvider() ){
				tDes.Key = key;
				tDes.IV = iv;

				using( var cStream = new CryptoStream( memStream, tDes.CreateDecryptor(), CryptoStreamMode.Read ) ){
					using( var reader = new BinaryReader( cStream ) ){
						data = reader.ReadBytes( (int)memStream.Length );
					}
				}
			}
        }

		return Encoding.UTF8.GetString( data );
	}


	/// <summary>
	/// 指定データをzip圧縮して返す
	/// </summary>
	public static byte[] PackByZip( byte[] data ){
		return ZlibStream.CompressBuffer( data );
	}
	public static byte[] PackByZip( string text ){
		return PackByZip( Encoding.UTF8.GetBytes( text ) );
	}


	/// <summary>
	/// zip圧縮されたデータを展開して返す
	/// </summary>
	public static byte[] UnPackByZip( byte[] packedData ){
		return ZlibStream.UncompressBuffer( packedData );
	}
	
	public static string UnPackStringByZip( byte[] packedData ){
		return Encoding.UTF8.GetString( UnPackByZip( packedData ) );
	}


	public static string ConvertUTF8String( byte[] data, int index = 0, int count = -1 ){
		if( count < 0 ) count = data.Length - index;
		return Encoding.UTF8.GetString( data, index, count );
	}


}


