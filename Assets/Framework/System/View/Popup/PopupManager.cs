using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ポップアップ生成クラス.
/// </summary>
public class PopupManager : MonoBehaviour
{
	/*
	/// OKポップアップ使用例
		PopupManager.SharedInstance.Create<PopupOK>("PopupOK").Init(
		"ポップアップ文言",
		//Okボタンがタップされた時の処理
		delegate(){
		});
		
	/// YNポップアップ使用例
		PopupManager.SharedInstance.Create<PopupYN>("PopupYN").Init(
		"ポップアップ文言",
		//Yesボタンがタップされた時の処理
		delegate(){
		},
		//Noボタンがタップされた時の処理
		delegate(){
		});
	*/
	[SerializeField]
	private ObjectGenerator generator;

	/// <summary>
	/// 共通インスタンス
	/// </summary>
	public static PopupManager SharedInstance { get; private set; }

	public T Create<T>(string name) where T : Component
	{
		if( generator == null ){
			return null;
		}

		GameObject obj = generator.InstantiatePrefab(this.gameObject, "Popup/"+name);
		if( obj.GetComponent<T>() == null ){
			obj.AddComponent<T>();
		}
		return obj.GetComponent<T>();
	}

	void Awake()
	{
		if(SharedInstance == null){
			SharedInstance = this;
		}
	}
}