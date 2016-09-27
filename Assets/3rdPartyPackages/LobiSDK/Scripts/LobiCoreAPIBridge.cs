using System;
using System.Collections;

using System.Runtime.InteropServices;
using UnityEngine;

namespace Kayac.Lobi.SDK
{
	public class LobiCoreAPIBridge : object {

		public static void SignupWithBaseName(string gameObjectName,
		                                      string callbackMethodName,
		                                      string baseName){
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaClass lobiClass = new AndroidJavaClass("com.kayac.lobi.sdk.unity.LobiCoreBridge");
			lobiClass.CallStatic("signupWithBaseName", gameObjectName, callbackMethodName, "id", baseName);
			#endif
			#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
			byte[] cGameObjectName     = System.Text.Encoding.UTF8.GetBytes(gameObjectName);
			byte[] cCallbackMethodName = System.Text.Encoding.UTF8.GetBytes(callbackMethodName);
			byte[] cBaseName           = System.Text.Encoding.UTF8.GetBytes(baseName);
			LobiAPI_signup_with_base_name_(cGameObjectName, cGameObjectName.Length,
			                               cCallbackMethodName, cCallbackMethodName.Length,
			                               cBaseName, cBaseName.Length);
			#endif
		}

		public static void SignupWithBaseName(string gameObjectName,
		                                      string callbackMethodName,
		                                      string baseName,
		                                      string encryptedExternalId,
		                                      string encryptIv){
			#if UNITY_ANDROID
			AndroidJavaClass lobiClass = new AndroidJavaClass("com.kayac.lobi.sdk.unity.LobiCoreBridge");
			lobiClass.CallStatic("signupWithBaseName", gameObjectName, callbackMethodName, "id", baseName, encryptedExternalId, encryptIv);
			#endif
			#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
			byte[] cGameObjectName      = System.Text.Encoding.UTF8.GetBytes(gameObjectName);
			byte[] cCallbackMethodName  = System.Text.Encoding.UTF8.GetBytes(callbackMethodName);
			byte[] cBaseName            = System.Text.Encoding.UTF8.GetBytes(baseName);
			byte[] cEncryptedExternalId = System.Text.Encoding.UTF8.GetBytes(encryptedExternalId);
			byte[] cEncryptIv           = System.Text.Encoding.UTF8.GetBytes(encryptIv);
			LobiAPI_signup_with_base_name_encrypted_external_id_encrypt_iv_(
				cGameObjectName, cGameObjectName.Length,
				cCallbackMethodName, cCallbackMethodName.Length,
				cBaseName, cBaseName.Length,
				cEncryptedExternalId, cEncryptedExternalId.Length,
				cEncryptIv, cEncryptIv.Length);
			#endif
		}

		public static void UpdateUserName(string gameObjectName,
		                                  string callbackMethodName,
		                                  string userName){
			#if UNITY_ANDROID
			AndroidJavaClass lobiClass = new AndroidJavaClass("com.kayac.lobi.sdk.unity.LobiCoreBridge");
			lobiClass.CallStatic("updateUserName", gameObjectName, callbackMethodName, "id", userName);
			#endif
			#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
			byte[] cGameObjectName      = System.Text.Encoding.UTF8.GetBytes(gameObjectName);
			byte[] cCallbackMethodName  = System.Text.Encoding.UTF8.GetBytes(callbackMethodName);
			byte[] cUserName            = System.Text.Encoding.UTF8.GetBytes(userName);
			LobiAPI_update_user_name_(
				cGameObjectName, cGameObjectName.Length,
				cCallbackMethodName, cCallbackMethodName.Length,
				cUserName, cUserName.Length);
			#endif
		}

		public static void UpdateUserIcon(string gameObjectName,
		                                  string callbackMethodName,
		                                  string filePath){
			#if UNITY_ANDROID
			AndroidJavaClass lobiClass = new AndroidJavaClass("com.kayac.lobi.sdk.unity.LobiCoreBridge");
			lobiClass.CallStatic("updateUserIcon", gameObjectName, callbackMethodName, "id", filePath);
			#endif
			#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
			byte[] cGameObjectName      = System.Text.Encoding.UTF8.GetBytes(gameObjectName);
			byte[] cCallbackMethodName  = System.Text.Encoding.UTF8.GetBytes(callbackMethodName);
			byte[] cfilePath            = System.Text.Encoding.UTF8.GetBytes(filePath);
			LobiAPI_update_user_icon_(
				cGameObjectName, cGameObjectName.Length,
				cCallbackMethodName, cCallbackMethodName.Length,
				cfilePath, cfilePath.Length);
			#endif
		}

		public static void UpdateUserCover(string gameObjectName,
		                                  string callbackMethodName,
		                                  string filePath){
			#if UNITY_ANDROID
			AndroidJavaClass lobiClass = new AndroidJavaClass("com.kayac.lobi.sdk.unity.LobiCoreBridge");
			lobiClass.CallStatic("updateUserCover", gameObjectName, callbackMethodName, "id", filePath);
			#endif
			#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)

			#endif
		}

		public static void UpdateEncryptedExternalId(string gameObjectName,
													 string callbackMethodName,
													 string encryptedExternalId,
													 string encryptIv){
			#if UNITY_ANDROID
			AndroidJavaClass lobiClass = new AndroidJavaClass("com.kayac.lobi.sdk.unity.LobiCoreBridge");
			lobiClass.CallStatic("updateEncryptedExternalId", gameObjectName, callbackMethodName, "id", encryptedExternalId, encryptIv);
			#endif
			#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
			byte[] cGameObjectName      = System.Text.Encoding.UTF8.GetBytes(gameObjectName);
			byte[] cCallbackMethodName  = System.Text.Encoding.UTF8.GetBytes(callbackMethodName);
			byte[] cEncryptedExternalId = System.Text.Encoding.UTF8.GetBytes(encryptedExternalId);
			byte[] cEncryptIv           = System.Text.Encoding.UTF8.GetBytes(encryptIv);
			LobiAPI_update_encrypted_external_id_iv_(
				cGameObjectName, cGameObjectName.Length,
				cCallbackMethodName, cCallbackMethodName.Length,
				cEncryptedExternalId, cEncryptedExternalId.Length,
				cEncryptIv, cEncryptIv.Length);
			#endif
		}

		public static void IsBoundWithLobiAccount(){
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaClass lobiClass = new AndroidJavaClass("com.kayac.lobi.sdk.unity.LobiCoreBridge");
			lobiClass.CallStatic("isBoundWithLobiAccount");
			#endif
			#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
			LobiAPI_is_bound_with_lobi_account_();
			#endif
		}

		#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
		[DllImport("__Internal")]
		private static extern void LobiAPI_signup_with_base_name_(
			byte[] game_object_name, int game_object_name_len,
			byte[] callback_method_name, int callback_method_name_len,
			byte[] base_name, int base_name_len);

		[DllImport("__Internal")]
		private static extern void LobiAPI_signup_with_base_name_encrypted_external_id_encrypt_iv_(
			byte[] game_object_name, int  game_object_name_len,
			byte[] callback_method_name, int callback_method_name_len,
			byte[] base_name, int base_name_len,
			byte[] encrypted_external_id, int encrypted_external_id_len,
			byte[] encrypt_iv, int encrypt_iv_len);

		[DllImport("__Internal")]
		private static extern void LobiAPI_update_user_name_(
			byte[] game_object_name, int  game_object_name_len,
			byte[] callback_method_name, int callback_method_name_len,
			byte[] user_name, int user_name_len);

		[DllImport("__Internal")]
		private static extern void LobiAPI_update_user_icon_(
			byte[] game_object_name, int  game_object_name_len,
			byte[] callback_method_name, int callback_method_name_len,
			byte[] file_path, int file_path_len);

		[DllImport("__Internal")]
		private static extern void LobiAPI_update_encrypted_external_id_iv_(
			byte[] game_object_name, int  game_object_name_len,
			byte[] callback_method_name, int callback_method_name_len,
			byte[] encrypted_external_id, int encrypted_external_id_len,
			byte[] iv, int iv_len);

		[DllImport("__Internal")]
		private static extern void LobiAPI_is_bound_with_lobi_account_();
		#endif
	}
}
