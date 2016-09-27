using UnityEngine;
using System.Collections;
using MyLibrary.Unity;


/// <summary>
/// アプリケーションのコアをとりまとめたコンポーネント
/// </summary>
public class AppCore : ViewBase
{
	/// <summary>
	/// 初期化が終わっているかどうか
	/// </summary>
	public static bool IsInit { get; private set; }
    
    
    void Awake()
    {
        if(m_instance != null){
            m_instance.Dispose();
        }
        m_instance = this;
        
        IsInit = false;
        DontDestroyOnLoad(this.gameObject);
        
        // システムパスとフォルダの初期化
        GameSystem.Init();
        
        // キャッシュ用のディレクトリがなければ作成.
        FileUtility.CreateDirectory( GameSystem.CachesDirectoryPath + "/0000" );
        FileUtility.CreateDirectory( GameSystem.CachesDirectoryPath + "/0001" );
        FileUtility.CreateDirectory( GameSystem.CachesDirectoryPath + "/0002" );
        
        // ローカルデータロード.
        LocalDataManager.Load();
    }
	IEnumerator Start()
	{
		while( PopupManager.SharedInstance == null){
			yield return null;
		}
		while( ScreenChanger.SharedInstance == null){
			yield return null;
		}
        while( CameraHelper.SharedInstance == null ){
            yield return null;
        }
        while( SoundControll.SharedInstance == null ){
            yield return null;
        }
        while( View_BlackFade.SharedInstance == null ){
            yield return null;
        }
        ScreenChanger.WillChangeScene += DidChangeScene;
		IsInit = true;
	}
    
    // コールバック：シーン切り替え直前に呼び出し.シーンごとに使うObjectGeneratorを切り替える.
    private void DidChangeScene(string sceneName)
    {
        switch(sceneName){
        case "title":
			this.GetScript<ObjectGenerator>("UseTitleObjects").SetSharedInstance();
		// TODO : 必要シーンに応じて処理を追加していく.
            break;
        }
    }
    
    public override void Dispose()
    {
        this.gameObject.DestroyChildren();
        base.Dispose();
    }
    
    private static AppCore m_instance;
}
