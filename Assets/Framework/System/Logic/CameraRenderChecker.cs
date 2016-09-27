using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// カメラに写っているか調べるスクリプト.何かしらのレンダラーコンポーネントが同じオブジェクトについていないと機能しない点に注意！
/// </summary>
public class CameraRenderChecker : MonoBehaviour
{
    
    // OnWillRenderの前に呼ばれる.
    void Update()
    {
        // 毎回全てのキーをオフにする.
        var keyList = new List<string>(m_enableCameraNameDict.Keys);
        foreach(var key in keyList){
            m_enableCameraNameDict[key] = false;
        }
    }
    // カメラに写っていたらそのカメラから呼ばれる.
	void OnWillRenderObject()
    {    
        // 描画されているカメラをキーとしてオンオフ状況を登録.
        if(!m_enableCameraNameDict.ContainsKey(Camera.current.name)){
            m_enableCameraNameDict.Add(Camera.current.name, true);
        }
        m_enableCameraNameDict[Camera.current.name] = true;
    }
    
    // 再表示時も全てのフラグを下ろす.
    protected virtual void OnEnable()
    {
        var keyList = new List<string>(m_enableCameraNameDict.Keys);
        foreach(var key in keyList){
            m_enableCameraNameDict[key] = false;
        }
    }
    
    protected Dictionary<string, bool> m_enableCameraNameDict = new Dictionary<string, bool>();
}
