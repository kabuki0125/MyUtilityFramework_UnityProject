using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using MyLibrary.Unity;


/// <summary>
/// スクリーン切り替えロジック
/// </summary>
public class ScreenChanger : MonoBehaviour
{
	/// シーン切り替え前にやりたい処理
    public static event Action<string/*nextSceneName*/> WillChangeScene;
	/// シーン切り替え後にやりたい処理
	public static event Action<string/*nextSceneName*/> DidEndChangeScene;

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
		ScreenChanger.SharedInstance.Exec("title", ctrl, didProcEnd);
	}
    
    public void Reboot()
    {
        if(m_currentCtrl != null){
            m_currentCtrl.Dispose();
            m_currentCtrl = null;
        }
        SceneManager.LoadScene("boot");
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
			WillChangeScene(nextSceneName);
		}

        SceneManager.LoadScene(nextSceneName);
        
		yield return Resources.UnloadUnusedAssets();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

		if(DidEndChangeScene != null){
            DidEndChangeScene(nextSceneName);
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
