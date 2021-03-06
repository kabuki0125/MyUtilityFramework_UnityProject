/**
 * @file    CSEncryptHelper.cs
 * @brief
 *
 * @author  $Author$
 * @date    $Date$
 * @version $Rev$
 */
namespace MyLibrary.Unity.IO
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.IO;
    using System.Text;
    using System.Security.Cryptography;

    /// <summary>
    /// ヘルパークラス：暗号化
    /// </summary>
    public static class EncryptHelper
    {
        /// <summary>
        /// ハッシュ文字列取得
        /// </summary>
        public static string ToHashKey(this byte[] data)
        {
            byte[] hash = null;
            using(var md5 = new MD5CryptoServiceProvider()){
                hash = md5.ComputeHash(data);
            }
            return BitConverter.ToString(hash).ToLower().Replace("-","") ;
        }
        /// <summary>
        /// UTF-16 文字列からハッシュ文字列取得
        /// </summary>
        public static string ToHashKey(this string str)
        {
            return ToHashKey(Encoding.Unicode.GetBytes(str));
        }


        /// <summary>
        /// 指定パスのファイルに、指定データを暗号化して書きこむ.
        /// </summary>
        public static void WriteToFile(byte[] data, string key,string filePath)
        {
            using(var tDes = new TripleDESCryptoServiceProvider()){
                tDes.Key = GenerateKey(key, tDes.Key.Length);
                tDes.IV = GenerateKey(key, tDes.IV.Length);

                using(var fStream = File.Create(filePath)){
                    using(var cStream = new CryptoStream(fStream, tDes.CreateEncryptor(), CryptoStreamMode.Write) ){
                        cStream.Write(data, 0,data.Length);
                    }
                }
            }
            //CSDebug.Log("[MCEncryptHelper] Write To File : " + filePath + ", length = " + data.Length);
        }
        /// <summary>
        /// 指定キーでデータを暗号化して返す.
        /// </summary>
        public static byte[] EncryptData(string key,byte[] sources)
        {
            byte[] data = null;
            using(var tDes = new TripleDESCryptoServiceProvider()){
                tDes.Key = GenerateKey(key, tDes.Key.Length);
                tDes.IV = GenerateKey(key, tDes.IV.Length);

                using(var memStream = new MemoryStream()){
                    using(var cStream = new CryptoStream(memStream, tDes.CreateEncryptor(), CryptoStreamMode.Write) ){
                        cStream.Write(sources, 0,sources.Length);
                    }
                    data = memStream.ToArray();
                }
            }
            return data;
        }


        /// <summary>
        /// 指定パスのファイルを読み込んで、指定キーで復号化して返す.
        /// </summary>
        public static byte[] ReadFromFile(string key,string filePath)
        {
            if( !File.Exists(filePath)){
                return null;
            }

            byte[] data = null;
            using(var stream = new FileStream(filePath,FileMode.Open)){
                data = DecryptData(key,stream);
            }
            return data;
        }
        /// <summary>
        /// 指定キーでデータを復号化して返す.
        /// </summary>
        public static byte[] DecryptData(string key,byte[] sources)
        {
            byte[] data = null;
            using(var stream = new MemoryStream(sources)){
                data = DecryptData(key,stream);
            }
            return data;
        }
        public static byte[] DecryptData(string key,Stream sourceStream)
        {
            byte[] data = null;
            using(var tDes = new TripleDESCryptoServiceProvider()){
                tDes.Key = GenerateKey(key, tDes.Key.Length);
                tDes.IV = GenerateKey(key, tDes.IV.Length);

                using(var cStream = new CryptoStream(sourceStream, tDes.CreateDecryptor(), CryptoStreamMode.Read) ){
                    using(var reader = new BinaryReader(cStream)){
                        data = reader.ReadBytes((int)sourceStream.Length);
                    }
                }
            }
            //MCDebug.Log("[Encrypthelper] Read From File : " + filePath + ", length = " + data.Length);
            return data;
        }

        /// <summary>
        /// 指定キー文字列から、暗号化バイト列を返す
        /// </summary>
        public static byte[] GenerateKey(string key,int length)
        {
            var res = new byte[length];
            for(int i = 0; i < length; i++){
                res[i] = (byte)((key[i % key.Length] + i) & 0xff);
            }
            return res;
        }
    }
}
