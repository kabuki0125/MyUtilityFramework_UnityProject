using UnityEngine;
using System.Collections;

/*
 * Resources.Load撤廃用のクラス.
 * 
 * ※Resources.Load撤廃の理由：
 * 		アプリサイズの削減が主たる目的.Resourcesフォルダ内のファイルはビルド時にアプリパッケージに含まれるため.
 * 		またResources.Loadはロードの度にシーンとは関係なくキャッシュされていき、解放処理は動的に行わなければならない.
 * 		極力シーンに必要なものだけをキャッシュし必要がなくなれば解放としたい.
 * 		シーン毎にこのクラスを配置することでMonoBehaviourの解放タイミングにキャッシュクリアを任せる目的.
*/

/// <summary>
/// オブジェクト生成クラス.シーンにこいつを配置しておきロードしたいプレハブをInspectorで設定しておく.
/// </summary>
public class ObjectGenerator : MonoBehaviour 
{
	/// ロード用プレハブ
	[SerializeField]
	private GameObject[] prefabs;
	/// 恒久的にオブジェクトを残すかどうか
	[SerializeField]
	protected bool isDontDestroy = false;

	/// <summary>
	/// 名前からプレハブをインスタンス化
	/// </summary>
	public GameObject InstantiatePrefab(GameObject parent, string name)
	{
		return InstantiatePrefab(parent, this.GetPrefab(name) as GameObject);
	}
	/// <summary>
	/// リストからプレハブをインスタンス化
	/// </summary>
	public GameObject InstantiatePrefab(GameObject parent, GameObject prefab)
	{
		var obj	= GameObject.Instantiate(prefab) as GameObject;
		if(parent != null){
			parent.AddChild(obj);
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

	// リストからプレハブを取得する
	private Object GetPrefab(string name)
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
}
