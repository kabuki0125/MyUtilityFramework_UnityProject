using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Viewベースクラス
/// </summary>
public class ViewBase : MonoBehaviour, IDisposable
{

	/// <summary>
	/// プロパティ：ボタン有効設定
	/// </summary>
	public bool IsEnableButton
	{
		set {
			foreach(var b in this.GetComponentsInChildren<Button>(true)){
                if(!b.interactable){
                    continue;   // interactable値は演出上の見かけのトリガーとして使われることもあるのでここでは設定しない.そもそも設定されているものに関しては無視する.
                }
				b.enabled = value;
			}
		}
	}

	/// <summary>
	/// プロパティ：Destroy された？
	/// </summary>
	public bool IsDestroyed { get; private set; }


	/// <summary>
	/// 使用後は必ず呼び出すこと！内部でGameObjectも破棄してる.アンマネージド系のリソースを使うことを考慮.
	/// </summary>
	public virtual void Dispose()
	{
        if(!this.IsDestroyed){
		    GameObject.Destroy(this.gameObject);
        }
	}
	private void OnDestroy()
	{
		this.IsDestroyed = true;
	}

	/// <summary>
	/// ボタンを押した時の処理設定. ※for UGUI
	/// </summary>
	public void SetButtonMsg(string btnName, UnityEngine.Events.UnityAction func)
	{
		var btn = this.GetScript<Button>(btnName);
        btn.onClick.AddListener(func);
	}

	/// <summary>
	/// 指定タイプのコンポーネントを子階層から全て取得する
	/// </summary>
	public Dictionary<string, object> GetScriptList(Type type)
	{
		var	tbl	= new Dictionary<string, object>();
		
		foreach(var i in this.GetComponentsInChildren(type, true)){
			tbl[i.name]	= i;
			
			if( i.transform.parent != null ){
				tbl[i.transform.parent.name + "/" + i.name]	= i;
			}
		}
		return tbl;
	}

	/// <summary>
	/// 子階層にあるすべてのスクリプトから該当名のものを取得する.
	/// </summary>
	public T GetScript<T>(string key) where T : Component
	{
		if(!this.Scripts.ContainsKey(typeof(T)) ){
			this.UpdateScriptList<T>();
		}
		var	tbl	= this.Scripts[typeof(T)];
		return tbl[key] as T;
	}
	private void UpdateScriptList<T>() where T : Component
	{
		if( this.Scripts.ContainsKey(typeof(T)) ){
			this.Scripts[typeof(T)].Clear();
		}
		this.Scripts[typeof(T)]	= this.GetScriptList(typeof(T));
	}

	private Dictionary<Type, Dictionary<string, object>>	Scripts
	{
		get{
			if( this.m_scripts == null ){
				this.m_scripts	= new Dictionary<Type, Dictionary<string, object>>();
			}
			return m_scripts;
		}
	}
	private Dictionary<Type, Dictionary<string, object>> m_scripts;

}
