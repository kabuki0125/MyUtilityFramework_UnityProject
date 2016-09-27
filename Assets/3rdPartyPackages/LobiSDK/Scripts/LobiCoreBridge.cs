using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Kayac.Lobi.SDK
{
	public class LobiCoreBridge : object {
		public enum PopOverArrowDirection {
			Up = 0,
			Left,
			Right,
		};
		
		[System.Obsolete("use IsSignedIn")]
		public static bool IsReady(){
			bool isReady = false;
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaClass lobiClass = new AndroidJavaClass("com.kayac.lobi.sdk.unity.LobiCoreBridge");
			isReady = (lobiClass.CallStatic<int>("isReady") == 1);
			#endif
			#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
			isReady = LobiCore_is_signed_in_() == 1;
			#endif
			return isReady;
		}
		
		public static bool IsSignedIn(){
			bool isReady = false;
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaClass lobiClass = new AndroidJavaClass("com.kayac.lobi.sdk.unity.LobiCoreBridge");
			isReady = (lobiClass.CallStatic<int>("isReady") == 1);
			#endif
			#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
			isReady = LobiCore_is_signed_in_() == 1;
			#endif
			return isReady;
		}

		public static void SetAccountBaseName(string baseName){
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaClass lobiClass = new AndroidJavaClass("com.kayac.lobi.sdk.unity.LobiCoreBridge");
			lobiClass.CallStatic("setAccountBaseName", baseName);
			#endif
			#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
			byte[] cBaseName = System.Text.Encoding.UTF8.GetBytes(baseName);		
			LobiCore_set_account_base_name_(cBaseName, cBaseName.Length);
			#endif
		}

		public static void PresentProfile(){
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaClass lobiClass = new AndroidJavaClass("com.kayac.lobi.sdk.unity.LobiCoreBridge");
			lobiClass.CallStatic("presentProfile");
			#endif
			#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
			LobiCore_present_profile_();
			#endif
		}

		public static void PresentAdWall(){
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaClass lobiClass = new AndroidJavaClass("com.kayac.lobi.sdk.unity.LobiCoreBridge");
			lobiClass.CallStatic("presentAdWall");
			#endif
			#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
			LobiCore_present_ad_wall_();
			#endif
		}

		public static void SetupPopOverController(int x, int y, LobiCoreBridge.PopOverArrowDirection direction){
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaClass lobiClass = new AndroidJavaClass("com.kayac.lobi.sdk.unity.LobiCoreBridge");
			lobiClass.CallStatic("setupPopOverController", x, y, (int)direction);
			#endif
			#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
			LobiCore_setup_pop_over_controller_(x, y, (int)direction);
			#endif
		}

		public static void SetNavigationBarCustomColor(float r, float g, float b){
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaClass lobiClass = new AndroidJavaClass("com.kayac.lobi.sdk.unity.LobiCoreBridge");
			lobiClass.CallStatic("setNavigationBarCustomColor", r, g, b);
			#endif
			#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
			LobiCore_set_navigation_bar_custom_color_(r, g, b);
			#endif
		}

		public static void PrepareExternalId(string encryptedExternalId,
											 string encryptIv,
											 string baseName)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaClass lobiClass = new AndroidJavaClass("com.kayac.lobi.sdk.unity.LobiCoreBridge");
			lobiClass.CallStatic("prepareExternalId", encryptedExternalId, encryptIv, baseName);
			#endif
			#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
			byte[] cEncryptedExternalId = System.Text.Encoding.UTF8.GetBytes(encryptedExternalId);
			byte[] cEncryptIv           = System.Text.Encoding.UTF8.GetBytes(encryptIv);
			byte[] cBaseName            = System.Text.Encoding.UTF8.GetBytes(baseName);

			LobiCore_prepare_external_id_initialize_vector_account_base_name_(
				cEncryptedExternalId, cEncryptedExternalId.Length,
				cEncryptIv, cEncryptIv.Length,
				cBaseName, cBaseName.Length);
			#endif
		}

		public static void SetUseStrictExId(bool enable){
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaClass lobiClass = new AndroidJavaClass("com.kayac.lobi.sdk.unity.LobiCoreBridge");
			if (enable) {
				lobiClass.CallStatic("enableStrictExid");
			}
			#endif
			#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
			LobiCore_set_use_strict_ex_id_(enable ? 1 : 0);
			#endif
		}

		public static bool GetUseStrictExId(){
			bool enable = false;
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaClass lobiClass = new AndroidJavaClass("com.kayac.lobi.sdk.unity.LobiCoreBridge");
			enable = (lobiClass.CallStatic<int>("getUseStrictExid") == 1);
			#endif
			#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
			enable = LobiCore_get_use_strict_ex_id_() == 1;
			#endif
			return enable;
		}

		public static bool HasExIdUser(){
			bool enable = false;
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaClass lobiClass = new AndroidJavaClass("com.kayac.lobi.sdk.unity.LobiCoreBridge");
			enable = (lobiClass.CallStatic<int>("hasExIdUser") == 1);
			#endif
			#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
			enable = LobiCore_has_exid_user_() == 1;
			#endif
			return enable;
		}

		public static void BindToLobiAccount(){
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaClass lobiClass = new AndroidJavaClass("com.kayac.lobi.sdk.unity.LobiCoreBridge");
			lobiClass.CallStatic("bindToLobiAccount");
			#endif
			#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
			LobiCore_bind_to_lobi_account_();
			#endif
		}

		#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
		[DllImport("__Internal")]
		private static extern int LobiCore_is_signed_in_();
		
		[DllImport("__Internal")]
		private static extern void LobiCore_set_account_base_name_(byte[] base_name, int base_name_len);

		[DllImport("__Internal")]
		private static extern void LobiCore_present_profile_();

		[DllImport("__Internal")]
		private static extern void LobiCore_present_ad_wall_();

		[DllImport("__Internal")]
		private static extern void LobiCore_setup_pop_over_controller_(int x, int y, int direction);
		
		[DllImport("__Internal")]
		private static extern void LobiCore_set_navigation_bar_custom_color_(float r, float g, float b);

		[DllImport("__Internal")]
		private static extern void LobiCore_prepare_external_id_initialize_vector_account_base_name_(
			byte[] encrypted_external_id, int encrypted_external_id_len,
			byte[] encrypt_iv, int encrypt_iv_len,
			byte[] base_name, int base_name_len);

		[DllImport("__Internal")]
		private static extern void LobiCore_set_use_strict_ex_id_(int enable);

		[DllImport("__Internal")]
		private static extern int LobiCore_get_use_strict_ex_id_();

		[DllImport("__Internal")]
		private static extern int LobiCore_has_exid_user_();

		[DllImport("__Internal")]
		private static extern void LobiCore_bind_to_lobi_account_();
		#endif
	}
}
