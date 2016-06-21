using UnityEngine;
using System.Collections;

/// <summary>
/// スクリーンコントローラ：ゲームメイン
/// </summary>
public class GameMainSController : ScreenControllerBase
{
	/// <summary>
	/// タイトル周りのオブジェクト
	/// </summary>
	public static ObjectGenerator ObjGenerator { get; private set; }
	
	/// <summary>
	/// アンマネージド系のリソースを使うことを考慮
	/// </summary>
	public override void Dispose()
	{
		Debug.Log ("Call Dispose - GameMainSController");
		base.Dispose();
	}
	
	/// <summary>
	/// 初期化.スクリーン展開前の通信処理がある場合はここで.
	/// </summary>
	/// <param name="didConnectEnd">通信終了時の処理</param>
	public override void Init(System.Action<bool> didConnectEnd)
	{
		var go = GameObjectEx.LoadAndCreateObject("GameMainObjects");
		ObjGenerator = go.GetComponent<ObjectGenerator>();
		base.Init(didConnectEnd);
	}
	
	/// <summary>
	/// コントローラが最初に管理するスクリーンの生成
	/// </summary>
	public override void CreateBootScreen()
	{
		var go = ObjGenerator.InstantiatePrefab(this.gameObject, "ScreenGameMain");
		go.GetComponent<ScreenGameMain>().Init();
	}	
}
