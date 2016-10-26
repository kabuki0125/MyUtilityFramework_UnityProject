using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// クラス：アセットバンドルロード用クラス.
/// </summary>
public class AssetBundleLoader<T> : MonoBehaviour 
where T : UnityEngine.Object
{
    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init(string saveDir, uint capacity)
    {
        m_cache = new AssetBundleCache<T>(saveDir, capacity);
    }
    
    /// <summary>
    /// ファイル名からロード.必要に応じて事前にサーバーからのダウンロード処理を行う.
    /// </summary>
    public void Load(string fileName, Action<T> didLoad, string serverUrl = null)
    {
        if( !string.IsNullOrEmpty(serverUrl) ){
            this.DownLoad( serverUrl, () => m_cache.Load(fileName, didLoad) );
        }else{
            m_cache.Load(fileName, didLoad);
        }
    }
    // サーバーからのダウンロード.
    public void DownLoad(string url, Action didDownload)
    {
        
    }
    
    /// <summary>
    /// メモリキャッシュの削除.
    /// </summary>
    public void ClearCacheMemory()
    {
        m_cache.ClearMemory();
    }

    private AssetBundleCache<T> m_cache;
}
