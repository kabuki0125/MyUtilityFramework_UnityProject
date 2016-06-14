using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Viewベースクラス
/// </summary>
public abstract class ViewBase : MonoBehaviour, IDisposable
{
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

	/// <summary>
	/// アンマネージド系のリソースを使うことを考慮
	/// </summary>
	public virtual void Dispose()
	{
		Debug.Log(this.name + ": call dispose.");
		GameObject.Destroy(this.gameObject);
	}

	/// <summary>
	/// ボタンを押した時の処理設定. ※NGUI ver3未満対応
	/// </summary>
	public void SetButtonMsg(string btnName, string funcName)
	{
		this.GetScript<UIButtonMessage>(btnName).target = this.gameObject;
		this.GetScript<UIButtonMessage>(btnName).functionName = funcName;
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
		
#if DEBUG
		if(!tbl.ContainsKey(key) ){
			Debug.LogError("GetComponent KeyError:"+key);
		}
#endif
		return tbl[key] as T;
	}
	private void UpdateScriptList<T>() where T : Component
	{
		if( this.Scripts.ContainsKey(typeof(T)) ){
			this.Scripts[typeof(T)].Clear();
		}
		this.Scripts[typeof(T)]	= this.GetScriptList(typeof(T));
	}
	private Dictionary<string, object> GetScriptList(Type type)
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

}
