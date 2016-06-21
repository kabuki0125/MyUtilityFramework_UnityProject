using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ゲームオブジェクト拡張クラス.
/// </summary>
public static class GameObjectEx
{
	/// <summary>
	/// ResourcesLoadからゲームオブジェクトを生成.
	/// </summary>
	public static GameObject LoadAndCreateObject(string name, GameObject parent = null)
	{
		var o = Resources.Load(name) as GameObject;
		var go = GameObject.Instantiate(o) as GameObject;
		if(parent != null){
			parent.AddChild(go);
		}
		return go;
	}

	/// <summary>
	/// 子オブジェクトをすべて取得
	/// </summary>
	public static GameObject[] GetChildren(this GameObject self)
	{
		var rtn = new List<GameObject>();
		foreach(Transform t in self.transform){
			rtn.Add(t.gameObject);
		}
		return rtn.ToArray();
	}
	
	/// <summary>
	/// 子オブジェクトを追加.
	/// </summary>
	public static void AddChild(this GameObject self, GameObject child)
	{
		var p	= child.transform.localPosition;
		var r	= child.transform.localRotation;
		var s	= child.transform.localScale;

		child.transform.parent = self.transform;
		
		child.transform.localPosition	= p;
		child.transform.localRotation	= r;
		child.transform.localScale	= s;
	}
	
	/// <summary>
	/// 子オブジェクトをすべて破棄.
	/// </summary>
	public static void DestroyChildren(this GameObject self)
	{
		foreach(Transform t in self.transform){
			if(t != null && t.gameObject != null){
				GameObject.Destroy(t.gameObject);
			}
		}
	}

	/// <summary>
	/// 必要であればAddComponentするGetComponent.
	/// </summary>
	public static T GetOrAddComponent<T>(this GameObject self) where T : Component
	{
		return self.GetComponent<T>() ?? self.AddComponent<T>();
	}
}
