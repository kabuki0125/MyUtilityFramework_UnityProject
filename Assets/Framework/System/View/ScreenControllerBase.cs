using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 各スクリーンコントローラのベースクラス
/// </summary>
public abstract class ScreenControllerBase : MonoBehaviour, IDisposable
{
    /// <summary>
    /// スクリーン生成
    /// </summary>
	public static T Create<T>() 
        where T: ScreenControllerBase
	{
        var go = new GameObject("ScreenController");
		// TODO : UIRootつける？
        return go.AddComponent<T>();
	}

	/// <summary>
	/// アンマネージド系のリソースを使うことを考慮
	/// </summary>
	public virtual void Dispose()
	{
		Debug.Log(this.name + ": call dispose.");
		GameObject.Destroy(this.gameObject);
	}

	/// <summary>
	/// 初期化.スクリーン展開前の通信処理がある場合はここで.
	/// </summary>
	/// <param name="didConnectEnd">通信終了時の処理</param>
	public virtual void Init(Action<bool/*bSuccess*/> didConnectEnd)
	{
		didConnectEnd(true);
	}

	/// <summary>
	/// コントローラが最初に管理するスクリーンの生成
	/// </summary>
	public abstract void CreateBootScreen();

	void Awake()
	{
		DontDestroyOnLoad(this.gameObject);
	}
}

