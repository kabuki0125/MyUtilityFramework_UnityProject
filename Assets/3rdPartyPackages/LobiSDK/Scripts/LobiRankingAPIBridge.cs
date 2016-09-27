using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Kayac.Lobi.SDK
{
	public class LobiRankingAPIBridge : object {
		public enum RankingRange {
			Today = 0,
			Week,
			All,
			LastWeek,
		}
		
		public enum RankingCursorOrigin {
			Top = 0,
			Self,
		}

		public static void SendRanking(string gameObjectName,
		                               string callbackMethodName,
		                               string rankingId,
		                               System.Int64 score){
			#if UNITY_ANDROID
			AndroidJavaClass lobiClass = new AndroidJavaClass("com.kayac.lobi.sdk.ranking.unity.LobiRankingBridge");
			lobiClass.CallStatic("sendRanking", gameObjectName, callbackMethodName, "id", rankingId, score);
			#endif
			
			#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
			byte[] cGameObjectName     = System.Text.Encoding.UTF8.GetBytes(gameObjectName);
			byte[] cCallbackMethodName = System.Text.Encoding.UTF8.GetBytes(callbackMethodName);
			byte[] cRankingId          = System.Text.Encoding.UTF8.GetBytes(rankingId);
			LobiRanking_send_ranking_(cGameObjectName, cGameObjectName.Length,
			                          cCallbackMethodName, cCallbackMethodName.Length,
			                          cRankingId, cRankingId.Length,
			                          score);
			#endif
		}

		public static void GetRanking(string gameObjectName,
		                              string callbackMethodName,
		                              string rankingId,
		                              RankingRange type,
		                              RankingCursorOrigin origin,
		                              int cursor,
		                              int limit){
			#if UNITY_ANDROID
			AndroidJavaClass nakamapClass = new AndroidJavaClass("com.kayac.lobi.sdk.ranking.unity.LobiRankingBridge");  
			nakamapClass.CallStatic("getRanking", gameObjectName, callbackMethodName, "id", rankingId, (int)type, (int)origin, cursor, limit);
			#endif
			
			#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
			byte[] cGameObjectName     = System.Text.Encoding.UTF8.GetBytes(gameObjectName);
			byte[] cCallbackMethodName = System.Text.Encoding.UTF8.GetBytes(callbackMethodName);
			byte[] cRankingId          = System.Text.Encoding.UTF8.GetBytes(rankingId);
			LobiRanking_get_ranking_(cGameObjectName, cGameObjectName.Length,
			                         cCallbackMethodName, cCallbackMethodName.Length,
			                         cRankingId, cRankingId.Length,
			                         (int)type,
			                         (int)origin,
			                         cursor,
			                         limit);
			#endif
		}

		public static void GetRankingList(string gameObjectName,
		                                  string callbackMethodName,
		                                  RankingRange type){
			#if UNITY_ANDROID
			AndroidJavaClass nakamapClass = new AndroidJavaClass("com.kayac.lobi.sdk.ranking.unity.LobiRankingBridge");  
			nakamapClass.CallStatic("getRankingList", gameObjectName, callbackMethodName, "id", (int)type);
			#endif
			
			#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
			byte[] cGameObjectName     = System.Text.Encoding.UTF8.GetBytes(gameObjectName);
			byte[] cCallbackMethodName = System.Text.Encoding.UTF8.GetBytes(callbackMethodName);
			LobiRanking_get_ranking_list_(cGameObjectName, cGameObjectName.Length,
			                              cCallbackMethodName, cCallbackMethodName.Length,
			                              (int)type);
			#endif
		}

		public static void GetRankingList(string gameObjectName,
		                                  string callbackMethodName,
		                                  RankingRange type,
		                                  string uid){
			#if UNITY_ANDROID
			AndroidJavaClass nakamapClass = new AndroidJavaClass("com.kayac.lobi.sdk.ranking.unity.LobiRankingBridge");  
			nakamapClass.CallStatic("getRankingListWithUid", gameObjectName, callbackMethodName, "id", (int)type, uid);
			#endif
			
			#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
			byte[] cGameObjectName     = System.Text.Encoding.UTF8.GetBytes(gameObjectName);
			byte[] cCallbackMethodName = System.Text.Encoding.UTF8.GetBytes(callbackMethodName);
			byte[] cUid                = System.Text.Encoding.UTF8.GetBytes(uid);
			LobiRanking_get_user_ranking_list_(cGameObjectName, cGameObjectName.Length,
			                                   cCallbackMethodName, cCallbackMethodName.Length,
			                                   (int)type,
			                                   cUid, cUid.Length);
			#endif
		}

		#if ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
		[DllImport("__Internal")]
		private static extern void LobiRanking_send_ranking_(
			byte[] game_object_name, int game_object_name_len,
			byte[] callback_method_name, int callback_method_name_len,
			byte[] ranking_id, int ranking_id_len,
			System.Int64 score);

		[DllImport("__Internal")]
		private static extern void LobiRanking_get_ranking_(
			byte[] game_object_name, int game_object_name_len,
		    byte[] callback_method_name, int callback_method_name_len,
		    byte[] ranking_id, int ranking_id_len,
		    int type,
		    int origin,
		    int cursor,
			int limit);

		[DllImport("__Internal")]
		private static extern void LobiRanking_get_ranking_list_(
			byte[] game_object_name, int game_object_name_len,
			byte[] callback_method_name, int callback_method_name_len,
			int type);

		[DllImport("__Internal")]
		private static extern void LobiRanking_get_user_ranking_list_(
			byte[] game_object_name, int game_object_name_len,
			byte[] callback_method_name, int callback_method_name_len,
			int type,
			byte[] uid, int uid_len);
		#endif
	}
}
