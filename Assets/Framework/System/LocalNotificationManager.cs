using UnityEngine;
using System;
using System.Collections;
using MyLibrary.Unity;

/// <summary>
/// Local notification manager.
/// </summary>
public class LocalNotificationManager : MonoBehaviour 
{
    public static LocalNotificationManager  SharedInstance  { get ; private set ; }
    
    void Awake()
    {
        if( SharedInstance != null ){
            GameObject.Destroy(this.gameObject);
            return;
        }
        
        SharedInstance  = this;
    }
    
    void OnApplicationPause(bool bPause)
    {
        if(!LocalDataManager.Data.IsNotificate){
            return;
        }
        
        LocalNotificationScheduler.CancelAllLocalNotifications();
        LocalNotificationScheduler.ClearLocalNotification();
        LocalNotificationScheduler.ClearBadge();
        
        if( bPause ){
			LocalNotificationSchedule();
        }
    }
    
    void OnApplicationQuit()
    {
        if(!LocalDataManager.Data.IsNotificate){
            return;
        }
        
		LocalNotificationSchedule();
    }

	void LocalNotificationSchedule()
	{
		// TODO : ローカル通知をスケジュール.
		LocalNotificationPush("3:00", "12:00", "test", "lunch time notification.", "this message is android only.");
	}
	// TODO : ローカル通知サンプル.インターバルを設けて直後の設定時刻に通知するロジック.
	void LocalNotificationPush(string interval_time, string schedule_time, string title, string message, string android_only_text)
	{
		// 現在時間
		var date		= new DateTime(1970, 1, 1, 9, 0, 0, DateTimeKind.Utc).AddSeconds(GameSystem.GetUnixTimeNow());
		var interval	= date + TimeSpan.Parse(interval_time);
		var schedule	= new DateTime(interval.Year, interval.Month, interval.Day) + TimeSpan.Parse(schedule_time);
		// 既に過ぎているので翌日にする。
		if( schedule.CompareTo(date) < 0 ){
			schedule	= schedule.AddDays(1);
		}
		var span		= schedule-date;
		
		var	time	= GameSystem.GetUnixTimeNow()+(ulong)span.TotalSeconds;
		LocalNotificationScheduler.Schedule(0, title, android_only_text, message, time, 1);
	}
}
