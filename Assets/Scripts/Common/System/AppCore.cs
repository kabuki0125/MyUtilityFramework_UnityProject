using UnityEngine;
using System.Collections;

/// <summary>
/// アプリケーションのコアをとりまとめたコンポーネント
/// </summary>
public class AppCore : MonoBehaviour
{
	/// <summary>
	/// 初期化が終わっているかどうか
	/// </summary>
	public static bool IsInit { get; private set; }

	void Awake()
	{
		IsInit = false;
		DontDestroyOnLoad(this.gameObject);
	}
	IEnumerator Start()
	{
		while( PopupManager.SharedInstance == null){
			yield return null;
		}
		while( ScreenChanger.SharedInstance == null){
			yield return null;
		}
		IsInit = true;
	}
}
