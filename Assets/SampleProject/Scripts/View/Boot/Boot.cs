using UnityEngine;
using System.Collections;

/// <summary>
/// アプリブート用スクリプト.
/// </summary>
public class Boot : MonoBehaviour
{
	IEnumerator Start()
	{
		while(!AppCore.IsInit){
			yield return null;	// AppCoreの準備を待つ
		}
		ScreenChanger.SharedInstance.GoToTitle();
	}
}
