using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Resources.Load撤廃用のクラス.
 * 
 * ※Resources.Load撤廃の理由：
 *      アプリサイズの削減が主たる目的.Resourcesフォルダ内のファイルはビルド時にアプリパッケージに含まれるため.
 *      またResources.Loadはロードの度にシーンとは関係なくキャッシュされていき、解放処理は動的に行わなければならない.
 *      極力シーンに必要なものだけをキャッシュし必要がなくなれば解放としたい.
 *      シーン毎にこのクラスを配置することでMonoBehaviourの解放タイミングにキャッシュクリアを任せる目的.
*/

/// <summary>
/// オブジェクト生成クラス.シーンにこいつを配置しておきロードしたいプレハブをInspectorで設定しておく.
/// </summary>
public class ObjectGenerator : MonoBehaviour 
{
    /// ロード用プレハブ
    public Object[] prefabs;
    
    /// <summary>
    /// 共通インスタンス.
    /// </summary>
    public static ObjectGenerator SharedInstance { get; private set; }
    
    
    /// <summary>
    /// 共通インスタンスをこのクラスに設定する.
    /// (シーンごとにObjectGeneratorを用意するがシーンロードのたびに表示してると所持するprefab量によっては読み込みに時間がかかってしまうので予めAppCoreに入れておく.複数の同一シングルトンクラスを使い回す事になるのでその対応用メソッド.)
    /// </summary>
    public void SetSharedInstance()
    {
        SharedInstance = this;
    }
    
    /// <summary>
    /// 名前からプレハブをインスタンス化
    /// </summary>
    public GameObject InstantiatePrefab(GameObject parent, string name, bool isDontDestroy = false)
    {
        return InstantiatePrefab(parent, this.GetPrefab(name) as GameObject, isDontDestroy);
    }
    /// <summary>
    /// リストからプレハブをインスタンス化
    /// </summary>
    public GameObject InstantiatePrefab(GameObject parent, GameObject prefab, bool isDontDestroy = false)
    {
        var obj = GameObject.Instantiate(prefab) as GameObject;
        if(parent != null){
            parent.AddInChild(obj);
        }
        if(isDontDestroy){
            DontDestroyOnLoad(obj);
            m_dontDestroyList.Add(name, obj);
        }
        return obj;
    }
    /// <summary>
    /// ゲームオブジェクト以外をインスタンス化したいときはこっちを呼ぶ
    /// </summary>
    public T InstantiatePrefab<T>(string name) where T : UnityEngine.Object
    {
        return Instantiate(this.GetPrefab(name)) as T;
    }
    
    /// <summary>
    /// isDontDestroy = true で生成したシーンをまたいでも破棄されないオブジェクトの破棄.
    /// ※isDontDestroy = true で生成したオブジェクトは必ずこのメソッドで削除すること！！
    /// </summary>
    public void DestroyObject(string name)
    {
        if(m_dontDestroyList == null){
            Debug.LogWarning("[ObjectGenerator] DestroyObject warning : m_dontDestroyList is null");
            return;
        }
        if(m_dontDestroyList.Count <= 0){
            Debug.LogWarning("[ObjectGenerator] DestroyObject warning : m_dontDestroyList is haven't object");
            return;
        }
        if(m_dontDestroyList.ContainsKey(name)){
            if(m_dontDestroyList[name] != null){
                Object.Destroy(m_dontDestroyList[name]);
            }
            m_dontDestroyList.Remove(name);
        }else{
            Debug.LogWarning("[ObjectGenerator] DestroyObject warning : object="+name+" is not found in scene.");
        }
    }
    
    /// <summary>
    /// リストからプレハブを取得する.
    /// </summary>
    public Object GetPrefab(string name)
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
    
    /// <summary>
    /// 可変長な数のobjectを追加する.
    /// </summary>
    public void AddPrefab(params Object[] objects)
    {
        var list = new List<Object>(prefabs);
        foreach(var obj in objects){
            list.Add(obj);
        }
        // リスト整形(nullになっている隙間を削除し、A~Z順に並べ替え)
        list.RemoveAll(o => o == null);
        list.Sort((x, y) => x.name.CompareTo(y.name));
        
        prefabs = list.ToArray();
    }
    
    private Dictionary<string, UnityEngine.Object> m_dontDestroyList = new Dictionary<string, UnityEngine.Object>();
}
