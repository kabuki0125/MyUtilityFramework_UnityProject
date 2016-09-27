using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// クラス：ボタン押した時のSE再生用スクリプト.
/// </summary>
public class ButtonSound : MonoBehaviour
{

    [SerializeField]
    private ReceiveType receiveType = ReceiveType.Button;
    
    [SerializeField]
    private SoundClipName soundClip = SoundClipName.SE_Button;

    void Awake()
    {
        this.SetReceiver();
    }
    
    // レシーバーに設定.
    private void SetReceiver()
    {
        switch(receiveType){
        case ReceiveType.Button:
            // ボタンタップイベント
            Button button = this.GetComponent<Button>();
            button.onClick.AddListener(() => {
                SoundControll.SharedInstance.PlaySE(soundClip);
            });
            break;
        case ReceiveType.Toggle:
            // トグルタップイベント
            Toggle toggle = this.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener((bChange) => {

                // なんらかのグループに所属しているトグルだった場合はアクティブ時にのみ音を再生させる.
                if(toggle.group != null){
                    if(!bChange){
                        SoundControll.SharedInstance.PlaySE(soundClip);
                    }
                }
                // それ以外は値が切り替わるごとに再生
                else{
                    SoundControll.SharedInstance.PlaySE(soundClip);
                }
                
            });
            break;
        }
    }
    
    // サウンドを鳴らす対象タイプ.
    private enum ReceiveType
    {
        Button, 
        Toggle,
    }
}
