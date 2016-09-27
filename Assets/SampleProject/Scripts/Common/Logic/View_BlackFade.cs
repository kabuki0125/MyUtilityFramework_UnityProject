using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

/// <summary>
/// View : 汎用暗幕フェードパネル.
/// </summary>
public class View_BlackFade : ViewBase
{
    /// <summary>共通インスタンス.</summary>
    public static View_BlackFade SharedInstance { get; private set; }
    
    /// <summary>フェード状態？(暗くなってる？).</summary>
    public bool IsFade { get { return m_sptShade.color.a >= 1f; } }
    
    /// <summary>フェード中？</summary>
    public bool IsFadingNow { get; private set; }
    
    
    /// <summary>
    /// 透明な状態から暗くなる.
    /// </summary>
    public void FadeOut(Action didEnd = null, bool bImmediate = false)
    {
        this.GetScript<Canvas>("canvas").gameObject.SetActive(true);
        if(bImmediate){
            var col = m_sptShade.color;
            col.a = 1f;
            m_sptShade.color = col;
            if(didEnd != null){
                didEnd();
            }
            return;
        }
        CoroutineAgent.Execute( this.FadeProc(true, didEnd) ); // 立て続けに呼ばれる状況などを加味してコルーチンを追加していき順番に処理させたい為CroutineAgentを使用する.
    }
    
    /// <summary>
    /// 暗い状態から透明に
    /// </summary>
    public void FadeIn(Action didEnd = null, bool bImmediate = false)
    {
        if(bImmediate){
            var col = m_sptShade.color;
            col.a = 0f;
            m_sptShade.color = col;
            if(didEnd != null){
                didEnd();
            }
            return;
        }
        CoroutineAgent.Execute( this.FadeProc(false, didEnd) );
    }
    
    // フェード処理.
    private IEnumerator FadeProc(bool bFadeOut, Action didEnd)
    {
        this.IsFadingNow = true;
        
        var col = m_sptShade.color;
        var limitVal = bFadeOut ?  1f : 0f;
        var val = bFadeOut ? 0.1f: -0.1f;        
        var bLimit = false;
        while(!bLimit){
            col.a += val;
            bLimit = bFadeOut ? col.a >= limitVal : col.a < limitVal;
            if(bLimit){
                col.a = limitVal;
            }
            m_sptShade.color = col;
            yield return null;
        }
        if(didEnd != null){
            didEnd();
        }
        if(!bFadeOut){
            this.GetScript<Canvas>("canvas").gameObject.SetActive(false);   // 透明な状態なら表示しない.
        }
        
        this.IsFadingNow = false;
    }
    
    // AppCoreに内包されているのでAwakeタイミングだと同一タイミングで初期化されるUICameraが取得できない恐れがある為Startで設定.
    void Start()
    {
        this.GetScript<Canvas>("canvas").worldCamera = CameraHelper.SharedInstance.UICamera;
        m_sptShade = this.GetScript<SpriteRenderer>("shade_panel");
        
        // 最初は黒.
        this.FadeOut(null, true);
    }
    void Awake()
    {
        if(SharedInstance != null){
            Debug.LogError("[View_BlackFade] Awake Error!! : instance already exist.");
            return;
        }
        SharedInstance = this;
    }
    
    private SpriteRenderer m_sptShade;
}
