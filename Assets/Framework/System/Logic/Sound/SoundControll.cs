using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// クラス：サウンド操作.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SoundControll : ViewBase
{

    /// <summary>共通インスタンス.</summary>
    public static SoundControll SharedInstance { get; private set; }
    
    /// <summary>
    /// プロパティ：BGM 音量. [0, 1.0f]
    /// </summary>
    public float VolumeBGM
    {
        get { return m_trackBGM.volume; }
        set { m_trackBGM.volume = value; }
    }
    
    /// <summary>
    /// プロパティ：SE 音量. [0, 1.0f]
    /// </summary>
    public float VolumeSE
    {
        get { return m_volumeSE; }
        set {
            m_volumeSE = value;
            foreach(var i in m_tracksSE){
                i.volume = m_volumeSE;
            }
        }
    }
    private float m_volumeSE;
    
    /// <summary>
    /// サウンドが鳴っている？
    /// </summary>
    public bool IsPlay
    {
        get {
            return this.IsPlayBGM && this.IsPlaySE;
        }
    }
    /// <summary>
    /// サウンドが鳴っている？ BGM
    /// </summary>
    public bool IsPlayBGM
    {
        get {
            return m_trackBGM.isPlaying;
        }
    }
    /// <summary>
    /// サウンドが鳴っている？ : SE
    /// </summary>
    public bool IsPlaySE
    {
        get {
            foreach(var i in m_tracksSE){
                if( i.isPlaying ){
                    return true;
                }
            }
            return false;
        }
    }
    
    /// <summary>
    /// 全停止.
    /// </summary>
    public void StopAll()
    {
        this.StopBGM();
        this.StopSE();
    }
    /// <summary>
    /// BGM停止.
    /// </summary>
    public void StopBGM()
    {
        m_trackBGM.Stop();
    }
    /// <summary>
    /// SE停止.
    /// </summary>
    public void StopSE()
    {
        foreach(var i in m_tracksSE){
            i.Stop();
            GameObject.Destroy(i);
        }
        m_tracksSE.Clear();
    }
    
    /// <summary>
    /// 指定のSEを止める.なければ何もしない.
    /// </summary>
    /// <param name="clip">Clip.</param>
    public void StopThisSE(SoundClipName clip)
    {
        var audioClip = ObjectGenerator.SharedInstance.GetPrefab(clip.GetFileName()) as AudioClip;
        var track = m_tracksSE.Find( se => se.clip.name == audioClip.name+"(Clone)" );
        if(track != null){
            track.Stop();
        }
    }

    /// <summary>
    /// SEのループをやめる.（ループSE以外は無視）
    /// </summary>
    public void QuitLoop()
    {
        foreach (var i in m_tracksSE){
            if (!i.loop){
                continue;
            }
            i.loop = false;
        }
    }

    /// <summary>
    /// BGM事前ロード
    /// </summary>
    public void PreLoadBGM(params SoundClipName[] clips)
    {
        m_chacheBGM.Clear();
        var idx = 0;
        foreach(var cn in clips){
            if(idx >= CAPACITY_BGM_CACHE){
                break;  // 指定キャパシティまでキャッシュ.
            }
            var name = cn.GetFileName();
            m_chacheBGM.Add(cn, ObjectGenerator.SharedInstance.InstantiatePrefab<AudioClip>(name));
            idx++;
        }
    }
    
    /// <summary>
    /// 指定時間で指定音量まで音をフェードアウトさせる.
    /// </summary>
    public void BGMVolumeFade(float targetVol, float duration)
    {
        this.StartCoroutine(this.BGMVolumeFadeEX(this.VolumeBGM, targetVol, duration));
    }
    IEnumerator BGMVolumeFadeEX(float volumeStart, float volumeEnd, float duration)
    {
        var defaultVol = 0.5f;
        float currentTime   = 0.0f;
        while(duration > currentTime){
            currentTime     +=Time.fixedDeltaTime;
            this.VolumeBGM  = defaultVol * (volumeStart + (volumeEnd - volumeStart) * currentTime / duration);
            
            yield return new WaitForSeconds(0.02f);
        }
        this.VolumeBGM  = volumeEnd;
    }

    /// <summary>
    /// 再生位置を先頭まで巻き戻す.
    /// </summary>
    public void RewindBGM()
    {
        m_trackBGM.time = 0.0f;
    }

    /// <summary>
    /// BGM再生.
    /// </summary>
    public void PlayBGM(SoundClipName clipName, bool bLoop = false)
    {
        AudioClip clip = null;
        if(!m_chacheBGM.TryGetValue(clipName, out clip)){
            var name = clipName.GetFileName();
            clip = ObjectGenerator.SharedInstance.InstantiatePrefab<AudioClip>(name);
        }
        this.Play(m_trackBGM, clip, bLoop);
    }
    /// <summary>
    /// SE再生.
    /// </summary>
    public void PlaySE(SoundClipName clipName, bool bLoop = false)
    {
        var name = clipName.GetFileName();
        var clip = ObjectGenerator.SharedInstance.InstantiatePrefab<AudioClip>(name);
        var track = this.gameObject.AddComponent<AudioSource>();
        track.volume = m_volumeSE;
        this.Play(track, clip, bLoop);
        m_tracksSE.Add(track);
    }
    
    // 再生
    private void Play(AudioSource track, AudioClip clip, bool bLoop)
    {
        track.Stop();
        track.loop = bLoop;
        track.clip = clip;
        track.Play();
    }


    /// <summary>
    /// SE連続再生.
    /// </summary>
    public void PlaySECont(SoundClipName clipName, int num, float wait)
    {
        var name = clipName.GetFileName();
        var clip = ObjectGenerator.SharedInstance.InstantiatePrefab<AudioClip>(name);
        var track = this.gameObject.AddComponent<AudioSource>();
        track.volume = m_volumeSE;
        StartCoroutine(this.IEContPlaySE(track, clip, num, wait));
        m_tracksSE.Add(track);
    }
    IEnumerator IEContPlaySE(AudioSource track, AudioClip clip, int num, float wait)
    {
        track.clip = clip;
        for (int i = 0; i < num; i++)
        {   
            track.Play();
            yield return new WaitForSeconds(wait);
        }
    }

    void Awake()
    {
        if(SharedInstance != null){
            Debug.LogError("[SoundControll] Error!! : Instance already exist.");
            return;
        }
        SharedInstance = this;
        m_trackBGM = this.GetComponent<AudioSource>();
    }
    void Start()
    {
//        this.VolumeBGM = LocalDataManager.Data.Volume_BGM;
//        this.VolumeSE = LocalDataManager.Data.Volume_SE;
        InvokeRepeating("UpdateTracksSE", 0.5f, 0.5f);
    }
    // SEの再生状況更新.
    private void UpdateTracksSE()
    {
        var list = m_tracksSE.ToArray();
        for(int i = 0; i < list.Length; ++i){
            if( !list[i].isPlaying ){
                GameObject.Destroy(list[i]);
                m_tracksSE.Remove(list[i]);
            }
        }
    }
    
    private AudioSource m_trackBGM;
    private List<AudioSource> m_tracksSE = new List<AudioSource>();
    
    private Dictionary<SoundClipName, AudioClip> m_chacheBGM = new Dictionary<SoundClipName, AudioClip>();  // PreLoadした音楽ファイルについてはキャッシュする.
    private readonly int CAPACITY_BGM_CACHE = 10;           // BGMキャッシュ可能数.
}

/// <summary>
/// enum：BGM/SE名.
/// </summary>
public enum SoundClipName
{
    // BGM
//    BGM_Opening, 
//    BGM_GameMain,
//    BGM_Clear, 
//    BGM_GameOver,
    BGM_Main,           // 常にループ
    
    // SE
    SE_Button,     // 全体的なボタンタップ音
    SE_Harvest,    // だいこん収穫
    SE_Clash,      // 壁・だいこんに激突してUFOが大破
    SE_Stop,       // 壁・だいこんに激突して一瞬時が止まる
    SE_Revive,     // 緊急回避で激突をなかったことに
    SE_Transfer,   // 開始時、UFO登場　転送オン
    SE_NewAppear,  // 新しいだいこんのカットイン登場
    SE_NewFix,     // 新しいだいこんのカットインFIX
    SE_Clear,      // クリア演出
    SE_Miss,       // ミス演出
    SE_Item,       // スコア、順位の登場音
    SE_Start,      // ステージ決定、開始時
    SE_NewArea,    // 新ワールド開放時、ステージがポンポン出現
    SE_Meteo,      // 隕石エリアに入った時に隕石が複数流れる
    SE_SpeedUp,    // スピードエリアに乗ってる最中はスピードが上がる
    SE_Raid,       // ネギの急襲
    SE_Defense,    // ブロッコリーのディフェンス
    SE_Switch,     // ドア　スイッチ
    SE_Open,       // ドア　開く
    SE_Snake,      // 蛇行床
    SE_Mushroom,   // キノコが急に生えてくる
    
    
}
public static class SoundClipNameHelper
{
    /// <summary>
    /// 指定サウンド名に対応する Assets/Resources/_Sound 以下のサウンドアセットファイル名を返す.
    /// </summary>
    public static string GetFileName(this SoundClipName clip)
    {
        return SOUND_CLIP_FILE_NAME[(int)clip];
    }    
    private static readonly string[] SOUND_CLIP_FILE_NAME = {
        // BGM
//        "BGM_Opening",
//        "BGM_GameMain",
//        "BGM_Clear",
//        "BGM_GameOver",
        "BGM_Main",
        
        // SE
        "SE_Button",  
        "SE_Harvest", 
        "SE_Clash",   
        "SE_Stop",    
        "SE_Revive",  
        "SE_Transfer",
        "SE_NewAppear",   
        "SE_NewFix",   
        "SE_Clear",   
        "SE_Miss",    
        "SE_Item",    
        "SE_Start",   
        "SE_NewArea", 
        "SE_Meteo",   
        "SE_SpeedUp", 
        "SE_Raid",    
        "SE_Defense", 
        "SE_Switch",  
        "SE_Open",    
        "SE_Snake",
        "SE_Mushroom",
    };
}