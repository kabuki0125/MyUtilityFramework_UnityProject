using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// スクリーンコントローラ：タイトル
/// </summary>
public class TitleSController : ScreenControllerBase
{

	/// <summary>
	/// アンマネージド系のリソースを使うことを考慮
	/// </summary>
	public override void Dispose()
	{
		Debug.Log ("Call Dispose - TitleSController");
		base.Dispose();
	}

	/// <summary>
	/// 初期化.スクリーン展開前の通信処理がある場合はここで.
	/// </summary>
	/// <param name="didConnectEnd">通信終了時の処理</param>
	public override void Init(Action<bool> didConnectEnd)
	{
		base.Init(didConnectEnd);
	}

	/// <summary>
	/// コントローラが最初に管理するスクリーンの生成
	/// </summary>
	public override void CreateBootScreen()
	{
		var go = ObjectGenerator.SharedInstance.InstantiatePrefab(this.gameObject, "ScreenTitle");
		go.GetComponent<ScreenTitle>().Init();
	}	
}
