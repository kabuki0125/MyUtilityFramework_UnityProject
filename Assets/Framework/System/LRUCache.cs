
namespace MyLibrary.Unity
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    
    /// <summary>
    /// クラス：LRU形式のメモリキャッシュ.(LRU=最も参照されていないアイテムから順次削除していくアルゴリズム.)
    /// </summary>
    public class LRUCache<K, V> : IDisposable
    {
        
        /// <summary>
        /// プロパティ：キャッシュ数.
        /// </summary>
        public int Count
        {
            get {
                return this.lruList.Count;
            }
        }
        
        /// <summary>
        /// キャッシュ操作用インデクサ.
        /// </summary>
        public V this[K key]
        {
            get {
                return this.Get(key);
            }
            set {
                this.Add(key, value);
            }
        }
        // 指定キーのアイテムを取得.
        private V Get(K key)
        {
            KeyValuePair<K, V> cacheItem;
            if(this.cacheMap.TryGetValue(key, out cacheItem)){
                this.lruList.Remove(cacheItem);
                this.lruList.Add(cacheItem);    // Getが呼ばれた=使用されたとみなし保存状態を更新.先頭に移動.
                return cacheItem.Value;
            }
            return default(V);
        }
        // 指定キー&値のセットでキャッシュ追加.
        private void Add(K key, V value)
        {
            this.Remove(key);   // 同一キーを追加する場合、入れ直すことでLRUを体現.
            if(this.lruList.Count >= capacity){
                var item = this.lruList[0];
                this.Remove(item.Key, true);    // キャパが上限に達していたら一番古いデータ=先頭アイテムから削除.
            }
            
            var cacheItem = new KeyValuePair<K, V>(key, value);
            this.lruList.Add(cacheItem);
            this.cacheMap.Add(key, cacheItem);
        }
        
        
        /// <summary>
        /// コンストラクタ.必ず容量を指定する.
        /// </summary>
        public LRUCache(uint capacity)
        {
            this.capacity = capacity;
        }
        protected LRUCache(){}
        
        // デストラクタ.
        ~LRUCache()
        {
            this.Dispose();
        }
        
        /// <summary>
        /// 解放処理.外部から解放できるようにDisposeパターンで作ってる.
        /// </summary>
        public void Dispose()
        {
            this.Clear();
        }
        
        // キャッシュクリア.
        private void Clear()
        {
            foreach(var i in this.lruList){
                this.DisposeItem(i);
            }
            this.lruList.Clear();
            this.cacheMap.Clear();
            GC.Collect();
        }
        
        // 指定キーのアイテムを削除.
        private void Remove(K key, bool bDispose = false)
        {
            if(!this.cacheMap.ContainsKey(key)){
                return;
            }
            if(bDispose){
                this.DisposeItem(this.cacheMap[key]);
            }
            this.lruList.Remove(this.cacheMap[key]);
            this.cacheMap.Remove(key);
            GC.Collect();
        }
        
        /// <summary>
        /// 指定キーのアイテムが存在する？
        /// </summary>
        public virtual bool IsExsits(K key)
        {
            return this[key].Equals(default(K));
        }
        
        // 解放処理があれば継承してここに処理を記載する.
        protected virtual void DisposeItem(KeyValuePair<K, V> cacheItem){}
        
        
        private readonly uint capacity;    // 容量.
        private Dictionary<K, KeyValuePair<K, V>> cacheMap = new Dictionary<K, KeyValuePair<K, V>>(); // 探査用キャッシュマップ.キャッシュの登録管理はこのDictionaryを見て行う.
        private List<KeyValuePair<K, V>> lruList = new List<KeyValuePair<K, V>>();    // lruキャッシュ本体.
    }
}