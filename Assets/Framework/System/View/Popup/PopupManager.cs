using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using MyLibrary.Unity;

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
    private GameObject[] prefabs;
    
	/// <summary>
	/// 共通インスタンス
	/// </summary>
	public static PopupManager SharedInstance { get; private set; }
    

	public T Create<T>(string name) where T : MonoBehaviour
	{
		GameObject obj = this.InstantiatePrefab(this.gameObject, name);
        return obj.GetOrAddComponent<T>();
	}

	void Awake()
	{
		if(SharedInstance == null){
			SharedInstance = this;
		}
	}
    
#region internal proc.
    
    // 名前からプレハブをインスタンス化
    public GameObject InstantiatePrefab(GameObject parent, string name)
    {
        var prefab = this.GetPrefab(name) as GameObject;
        var obj = GameObject.Instantiate(prefab) as GameObject;
        if(parent != null){
            parent.AddInChild(obj);
        }
        return obj;
    }
    // リストからプレハブを取得する
    private UnityEngine.Object GetPrefab(string name)
    {
        foreach(var i in prefabs){
            if( i.name != name ){
                continue;
            }
            return i;
        }
        Debug.LogError("GetPrefab Error:"+name);
        return null;
    }
    
#endregion
}