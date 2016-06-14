using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// スクリーン切り替えロジック
/// </summary>
public class ScreenChanger : MonoBehaviour
{
	/// シーン切り替え前にやりたい処理
	public static event Action WillChangeScene;
	/// シーン切り替え後にやりたい処理
	public static event Action DidEndChangeScene;

	/// <summary>
	/// 共通インスタンス
	/// </summary>
	public static ScreenChanger SharedInstance { get; private set; }

	#region 各種シーン移動処理.

	/// <summary>
	/// タイトルシーンに移動
	/// </summary>
	public void GoToTitle(Action didProcEnd = null)
	{
		var ctrl = ScreenControllerBase.Create<TitleSController>();
		ScreenChanger.SharedInstance.Exec("SampleScene1", ctrl, didProcEnd);
	}
	/// <summary>
	/// ゲームメインシーンに移動
	/// </summary>
	public void GoToGameMain(Action didProcEnd = null)
	{
		var ctrl = ScreenControllerBase.Create<GameMainSController>();
		ScreenChanger.SharedInstance.Exec("SampleScene2", ctrl, didProcEnd);
	}

	#endregion

	#region internal proc.

	// シーン切り替え実行
	private void Exec(string nextSceneName, ScreenControllerBase ctrl, Action didProcEnd = null)
	{
		if(m_currentCtrl != null){
			m_currentCtrl.Dispose();
			m_currentCtrl = null;
		}
		m_currentCtrl = ctrl;

		this.ChangeScene(nextSceneName, delegate() {
			m_currentCtrl.Init(delegate(bool bSuccess) {
				// 通信エラー時はスクリーンを展開せずにそのまま
				if(!bSuccess){
					Debug.LogWarning("[ScreenChanger - Exec] Connection error.");
					return;
				}
				m_currentCtrl.CreateBootScreen();
				if(didProcEnd != null){
					didProcEnd();
				}
			});
		});
	}
	private void ChangeScene(string nextSceneName, Action didProcEnd)
	{
		this.StartCoroutine(this.ChangeSceneproc(nextSceneName,didProcEnd));
	}
	private IEnumerator ChangeSceneproc(string nextSceneName, Action didProcEnd)
	{
		if(WillChangeScene != null){
			WillChangeScene();
		}

		Application.LoadLevel(nextSceneName);

		yield return Resources.UnloadUnusedAssets();
		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();

		if(DidEndChangeScene != null){
			DidEndChangeScene();
		}
		if(didProcEnd != null){
			didProcEnd();
		}
	}

	void Awake()
	{
		if(SharedInstance == null){
			SharedInstance = this;
		}
	}

	private ScreenControllerBase m_currentCtrl;

	#endregion
}
