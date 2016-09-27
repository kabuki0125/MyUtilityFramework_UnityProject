using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// フリック検知コントローラ.
/// </summary>
public class FlickController : ViewBase
{
    // フリックとして認識する閾値.
    [SerializeField]
    private float m_thresholdFlick = 0f;
    
    /// <summary>ドラッグ時のイベント.引数はスワイプorフリックの入力角度(ラジアン値).</summary>
    public event Action<float/*radian*/> DidDrag;
    /// <summary>ドラッグ時に一回だけ呼ばれるイベント.</summary>
    public event Action<float/*radian*/> DidDragOnce;
    /// <summary>ドラッグ開始時のイベント.</summary>
    public event Action DidStartDrag;
    /// <summary>ドラッグ終了時のイベント.</summary>
    public event Action DidEndDrag;
    
    
    void OnPress(bool bDown)
    {
        // 押した
        if(bDown){
            if(!m_bTap){
                m_bTap = true;
                this.SetStartPosition();
                this.StopCoroutine("PressHolding");
                this.StartCoroutine("PressHolding");
                if(DidStartDrag != null){
                    DidStartDrag();
                }
                m_bCallDragOnce = false;    // タップのタイミングで都度ドラッグ検知フラグリセット.
            }
        }
        // 離した
        else{
            m_bTap = false;
            if(DidEndDrag != null){
                DidEndDrag();
            }
        }
    }
    // タップした時に進行方向やや後ろ目に仮想スティックの中心点を設けることで操作性を向上させる.
    private void SetStartPosition()
    {
        var rad = Mathf.Atan2(m_prevVec.y, m_prevVec.x);
        var deg = ((rad * Mathf.Rad2Deg) + 180f) % 360f;
        var x = Mathf.Cos(deg * Mathf.Deg2Rad) * 80f;
        var y = Mathf.Sin(deg * Mathf.Deg2Rad) * 80f;
        m_startPos = new Vector3(x, y) + Input.mousePosition;
    }
    
    // 押しっぱなしならドラッグとしてイベントを送信し続ける.
    private IEnumerator PressHolding()
    {
        while(m_bTap){
            if(m_startPos == Input.mousePosition){
                yield return null;
                continue;
            }
            var inputVelocity = Input.mousePosition - m_startPos;
            var rad = Mathf.Atan2(inputVelocity.y, inputVelocity.x);
            if(inputVelocity.magnitude > m_thresholdFlick){
                // 一回だけのドラッグイベントコール.
                if(!m_bCallDragOnce){
                    m_bCallDragOnce = true;
                    if(DidDragOnce != null){
                        DidDragOnce(rad);
                    }
                }
                // ドラッグイベントコール.
                if(DidDrag != null){
                    DidDrag(rad);
                }
            }
            m_prevVec = inputVelocity;
            yield return null;
        }
    }
    
    void Start()
    {
        // タッチ領域の設定.
        this.InitCamera();
        
        // イベントトリガー準備
        this.GetScript<EventTrigger>("Button").triggers = new List<EventTrigger.Entry>();
        
        // タップ処理追加.
        var entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener( _ => OnPress(true) );
        this.GetScript<EventTrigger>("Button").triggers.Add(entry);
        
        // タップ終了時の処理追加.
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener( _ => OnPress(false) );
        this.GetScript<EventTrigger>("Button").triggers.Add(entry);
    }
    // キャンバス設定.
    protected virtual void InitCamera()
    {
        var canvas = this.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = CameraHelper.SharedInstance.UICamera;
    }
    
    private bool m_bTap;
    private Vector3 m_startPos;
    protected Vector3 m_prevVec;      // タップ開始位置を進行方向やや後ろに設定するために前回どの方向にスワイプしたのかベクトルを保持しておく

    private bool m_bCallDragOnce = false;   // 一回ドラッグ検知フラグ.
}
