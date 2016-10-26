using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using MyLibrary.Unity;

/// <summary>
/// クラス：アセットバンドルのローカルキャッシュクラス.
///        メモリ、ストレージの順にロードする.
///        キーはstringで固定.型は使用するアセットに合わせて設定する.
/// </summary>
// TODO : コルーチンを回すようにMonoBehaviourを継承.外部にコルーチン用のロジックがあるようであればそっちに任せる.
public class AssetBundleCache<T> : MonoBehaviour, IDisposable 
where T : UnityEngine.Object
{
    /// <summary>
    /// コンストラクタ.メモリキャッシュ容量とストレージに保存する際のディレクトリを指定.
    /// </summary>
    public AssetBundleCache(string saveDir, uint capacity)
    {
        m_storageDir = saveDir;
        m_cacheCapacity = capacity;
        m_cache = new AssetBundleLRUCache(m_cacheCapacity);
        
        if( !Directory.Exists(m_storageDir) ){
            Directory.CreateDirectory(m_storageDir);
        }
    }
    // デストラクタ.
    ~AssetBundleCache()
    {
        this.Dispose();
    }
    
    /// <summary>
    /// 外部から解放できるようにDisposeパターンで実装している.
    /// </summary>
    public void Dispose()
    {
        this.ClearMemory();
    }
    
    /// <summary>
    /// メモリクリア.ストレージには干渉しない.
    /// </summary>
    public void ClearMemory()
    {
        m_cache.Dispose();
    }
    
    /// <summary>
    /// 読み込み.
    /// </summary>
    public void Load(string key, Action<T> didLoad)
    {
        Action<T> didLoadEx = (t) => {
            if(didLoad != null){
                didLoad(t);
            }
            m_bLoadStorage = false;   // ストレージから読み込んでいた場合読み込みフラグが立っているので下げる.
        };
        
        // リクエストキューに追加してロード待ち.
        m_loadQueue.Add( new LoadInfo{ FileName = key, DidLoadProc = didLoadEx } );
    }
    
# region internal proc.
    
    // 毎フレームキューの中身を確認してロードしていく.
    void Update()
    {
        if(m_bLoadStorage || m_loadQueue.Count <= 0){
            return;
        }
        
        // キューなので上から取り出すイメージ.
        var info = m_loadQueue[0];
        m_loadQueue.RemoveAt(0);    
        
        // メモリ上にあればメモリから.なければストレージから読み込む.
        if( m_cache.IsExsits(info.FileName) ){
            info.DidLoadProc(m_cache[info.FileName] as T);
            return;
        }
        m_bLoadStorage = true;
        this.StartCoroutine("LoadFromStorage", info);
    }
    private IEnumerator LoadFromStorage(LoadInfo info)
    {
        if( !File.Exists(this.GetCachePath(info.FileName)) ){
            Debug.LogError("[AssetBundleCache] LoadFromStorage Error!! : file not found. file="+info.FileName);
            yield break;
        }
        
        using( var reader = new WWW("file://" + this.GetCachePath(info.FileName)) ){
            while( !reader.isDone && reader.error == null ){
                yield return null;
            }
            if( reader.error != null ){
                reader.Dispose();
                info.DidLoadProc(default(T));
                yield break;
            }
            // 読み込み成功.
            m_cache[info.FileName] = reader.assetBundle;
            info.DidLoadProc(reader.assetBundle as T);
        }
    }
    
    // ローカルストレージに配置されているファイルパス.
    private string GetCachePath(string fileName)
    {
        return m_storageDir + "/" + fileName + ".dat"; // 拡張子はdat想定.
    }
    
    private List<LoadInfo> m_loadQueue = new List<LoadInfo>();  // ロード用のキューリスト.
    private AssetBundleLRUCache m_cache;    // キャッシュデータ.
    private string m_storageDir;    // TODO : とりあえずここで設定しているがキャッシュ用ディレクトリが共通ならDefineをどこかで設定しておいた方が良い.
    private uint m_cacheCapacity;
    private bool m_bLoadStorage = false;

#endregion    
    
#region internal class.
    
    // プライベートクラス：アセットロード用情報.
    private sealed class LoadInfo
    {
        public string       FileName;
        public Action<T>    DidLoadProc;
    }
    
    // プライベートクラス：アセットバンドル用LRUキャッシュ.
	private sealed class AssetBundleLRUCache : LRUCache<string, AssetBundle>
    {
        public AssetBundleLRUCache(uint capacity) : base(capacity){}
        
        /// <summary>指定キーのアイテムが存在する？</summary>
        public override bool IsExsits(string key)
        {
            return this[key] != null;
        }
        // アセットバンドルなので解放=Unload.
        protected override void DisposeItem(KeyValuePair<string,AssetBundle> item)
        {
            item.Value.Unload(false);
        }
    }
    
#endregion
}
