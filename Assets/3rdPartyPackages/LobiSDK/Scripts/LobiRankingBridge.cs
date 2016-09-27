using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Kayac.Lobi.SDK
{
	public class LobiRankingBridge : object {
		public static void PresentRanking(){
			#if UNITY_ANDROID
			AndroidJavaClass lobiClass = new AndroidJavaClass("com.kayac.lobi.sdk.ranking.unity.LobiRankingBridge");
			lobiClass.CallStatic("presentRanking");
			#endif
			#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
			LobiRanking_present_ranking_();
			#endif
		}

		#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
		[DllImport("__Internal")]
		private static extern void LobiRanking_present_ranking_();
		#endif
	}
}
