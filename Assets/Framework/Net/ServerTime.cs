using UnityEngine;
using System;
using System.Collections;

using System.Text.RegularExpressions;


public class ServerTime {

	public static DateTime				Now { get { return new DateTime( ticks ); } }

	/// <summary>Unix時間の開始時刻</summary>
	public static readonly DateTime		UnixOrigin = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
	/// <summary>日本のタイムゾーン</summary>
	public static readonly TimeSpan		TimeZoneJST = new TimeSpan( 9, 0, 0 );
	/// <summary>JSTでのUnix時間の開始時刻</summary>
	public static readonly DateTime		UnixOriginJST = UnixOrigin + TimeZoneJST;

	private static long			ticks = 0;
	private static float		timer;
	private static float		counter;
	private static Action		OnTimerEvent;



	/// <summary>
	/// NTPサーバから時刻を取得する。
	/// 　yield return SetFromTimeServer(); で呼び出す事。
	/// </summary>
	public static IEnumerator SetFromTimeServer(){

		WWW www = new WWW( "https://ntp-a1.nict.go.jp/cgi-bin/jst" );
		while( !www.isDone ){
			yield return null;
		}

		Match match = Regex.Match( www.text, "<body>(.*)</body>", RegexOptions.IgnoreCase | RegexOptions.Singleline );

		float t;	
		if( float.TryParse( match.Groups[1].Value, out t ) ){
			ticks = UnixOriginJST.Ticks + ConvertDateTimeTicks( t );
		} else{
			ticks = DateTime.Now.Ticks;
		}
	}


	/// <summary>
	/// 更新処理。
	/// </summary>
	public static void Update( float deltaTime ){
		ticks += ConvertDateTimeTicks( deltaTime );

		if( timer > 0 ){
			counter += deltaTime;
			if( timer <= counter ){
				if( OnTimerEvent != null ) OnTimerEvent();
				counter = 0;
			}
		}
	}


	/// <summary>
	/// タイマーを開始する。
	/// </summary>
	/// <param name="timeCount">計測時間(秒)</param>
	/// <param name="onTimerEvent">計測完了時に呼ぶデリゲート</param>
	public static void StartTimer( float timeCount, Action onTimerEvent ){
		timer = timeCount;
		counter = 0;
		OnTimerEvent = onTimerEvent;
	}


	/// <summary>
	/// タイマーを停止する
	/// </summary>
	public static void StopTimer(){
		timer = 0;
		OnTimerEvent = null;
	}


	/// <summary>
	/// Unix時間(ミリ秒)からDateTimeを得る
	/// </summary>
	/// <param name="unixTime"></param>
	/// <returns></returns>
	public static DateTime GetTime( long unixTimeMS ){
		return new DateTime( UnixOriginJST.Ticks + ConvertDateTimeTicks( unixTimeMS / 1000.0f ) );
	}


	private static long ConvertDateTimeTicks( float seconds ){
		return (long)( seconds * 10000000 );		// DateTimeのTicksは100ナノ秒単位( 10000000 = 1000 * 1000 * 10 )
	}


}
