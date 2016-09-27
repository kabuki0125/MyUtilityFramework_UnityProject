using UnityEngine;
using System.Collections;
using MyLibrary.Unity;
using System.Runtime.InteropServices;

namespace MyLibrary.Unity
{
    
    /// <summary>
    /// ローカル通知のスケジューラ.
    /// </summary>
    public class LocalNotificationScheduler
    {
        
#if !UNITY_EDITOR
#if UNITY_ANDROID
        private static AndroidJavaObject _androidObject;
        private static AndroidJavaObject AndroidObj
        {
            get{
                if( _androidObject == null ){
                    _androidObject = new AndroidJavaObject("com.original.localnotificationplugin.LocalNotificationPlugin");
                    _androidObject.Call("Init");
                }
                return _androidObject;
            }
        }
#elif UNITY_IPHONE
        [DllImport("__Internal")]
        private static extern void LocalNotificationPlugin_ClearBadge();
#endif
#endif
    
        /// <summary>
        /// スケジュールされた通知を削除
        /// </summary>
    	public static void ClearLocalNotification()
        {
#if !UNITY_EDITOR
#if UNITY_ANDROID
            AndroidObj.Call("ClearNotification");
#elif UNITY_IPHONE
            UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
#endif
#endif
        }
        
        /// <summary>
        /// スケジュールされた通知を削除(既にステータスバーに通知された情報も削除)
        /// </summary>
        public static void CancelAllLocalNotifications()
        {
#if !UNITY_EDITOR
#if UNITY_ANDROID
            AndroidObj.Call("ClearNotification");
#elif UNITY_IPHONE
            UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
#endif
#endif
        }
        
        /// <summary>
        /// 通知を設定
        /// </summary>
        /// <param name="id">通知ID(Androidのみ。同IDは上書きされる)</param>
        /// <param name="tickerText">通知時のテキスト(Androidのみ)</param>
        /// <param name="contentTitle">通知バーに表示されるタイトル</param>
        /// <param name="contentText">通知バーに表示されるテキスト</param>
        /// <param name="fireTime">通知する時間(Unix Time)</param>
        public static void Schedule(int id, string tickerText, string contentTitle, string contentText, ulong fireTime, int badgeNumber = 0)
        {
#if !UNITY_EDITOR
#if UNITY_ANDROID
            AndroidObj.Call("ScheduleNotification", id, tickerText, contentTitle, contentText, (long)fireTime);
#elif UNITY_IPHONE
            var no = new UnityEngine.iOS.LocalNotification();
            no.alertAction = contentTitle;
            no.alertBody = contentText;
            no.fireDate = CSSystem.UnixEpoch.AddSeconds(fireTime);
            no.soundName = UnityEngine.iOS.LocalNotification.defaultSoundName;
            no.applicationIconBadgeNumber = badgeNumber;
            UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(no);
#endif
#endif
        }
        
        // <summary>
        /// バッジ削除(iOSのみ)
        /// </summary>
        public static void ClearBadge()
        {
#if !UNITY_EDITOR && UNITY_IPHONE
            LocalNotificationPlugin_ClearBadge() ;
#endif
        }
    }
}