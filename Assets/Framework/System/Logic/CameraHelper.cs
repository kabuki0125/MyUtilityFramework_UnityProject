using UnityEngine;
using System.Collections;

/// <summary>
/// 各種カメラのヘルパー.カメラにアクセスしたいときはここから.
/// </summary>
public class CameraHelper : ViewBase
{
    
    /// <summary>
    /// 共通インスタンス.
    /// </summary>
    public static CameraHelper SharedInstance { get; private set; }
    
    /// <summary>カメラを取得.</summary>
    public Camera MainCamera { get { return this.GetScript<Camera>("MainCamera"); } }
    
    /// <summary>UI用のカメラ取得.</summary>
    public Camera UICamera { get { return this.GetScript<Camera>("UI_Camera"); } }
    
    

    /// <summary>位置的にフォーカス終了してる？</summary>
    public bool IsEndFocus 
    {
        get {
            if(m_targetObj == null){
                return false;
            }
            var targetPos = m_basePos+m_targetObj.transform.position;
            var maxX = Mathf.Max( Mathf.Abs(targetPos.x), Mathf.Abs(this.MainCamera.transform.position.x) );
            var minX = Mathf.Min( Mathf.Abs(targetPos.x), Mathf.Abs(this.MainCamera.transform.position.x) );
            var maxY = Mathf.Max( Mathf.Abs(targetPos.y), Mathf.Abs(this.MainCamera.transform.position.y) );
            var minY = Mathf.Min( Mathf.Abs(targetPos.y), Mathf.Abs(this.MainCamera.transform.position.y) );
            return (maxX - minX) <= 0.05f && (maxY - minY) <= 0.05f;
        }
    }
    
    
    void Awake()
    {
        if(SharedInstance != null){
            Debug.LogError("[CameraController2D] Error!! : Already exist instance.");
            return;
        }
        SharedInstance = this;
    }
    
	void Start()
    {
        m_basePos = this.MainCamera.transform.position;
    }
    
    /// <summary>
    /// 指定ターゲットに時間をかけてフォーカスする.
    /// </summary>
    /// <param name="target">Target.</param>
    public void ForcusTargetDelay(GameObject target, int forcusSpeed)
    {
        this.StopCoroutine("UpdatePosition");
        this.StopCoroutine("UpdatePositionDelay");
        
        m_targetObj = target;
        this.StartCoroutine("UpdatePositionDelay", forcusSpeed);
    }
    private IEnumerator UpdatePositionDelay(int forcusSpeed)
    {
        var targetPos = new Vector3( m_basePos.x+m_targetObj.transform.position.x, m_basePos.y+m_targetObj.transform.position.y, this.MainCamera.transform.position.z );
        while(m_targetObj != null){
            this.MainCamera.transform.position = Vector3.Lerp(this.MainCamera.transform.position, targetPos, Time.deltaTime * (float)forcusSpeed);
            yield return null;
        }
    }
    
    /// <summary>
    /// 指定ターゲットにカメラをフォーカスする.
    /// </summary>
    public void ForcusTarget(GameObject target, float timeLeft = 0f)
    {
        this.StopCoroutine("UpdatePosition");
        this.StopCoroutine("UpdatePositionDelay");
        
        m_targetObj = target;
        this.StartCoroutine("UpdatePosition", timeLeft);
    }
    private IEnumerator UpdatePosition(float timeLeft)
    {
        // フォーカス時間の制限があればその時間だけフォーカス.
        var startTime = Time.time;
        while(timeLeft > 0f ? timeLeft > Time.time-startTime : m_targetObj != null){
            this.MainCamera.transform.position = new Vector3( m_basePos.x+m_targetObj.transform.position.x, m_basePos.y+m_targetObj.transform.position.y, this.MainCamera.transform.position.z );
            yield return null;
        }
        this.EndForcus();
    }
    
    /// <summary>
    /// フォーカス終了.
    /// </summary>
    public void EndForcus(bool bImmediate = false)
    {
        if(m_targetObj == null){
            Debug.LogWarning("[CameraController2D] EndForcus Warning : Not set target.");
            return;
        }
        this.StopCoroutine("UpdatePosition");
        if(bImmediate){
            this.MainCamera.transform.position = m_basePos;
        }else{
            this.StartCoroutine("ResetPosition");
        }
        m_targetObj = null;
    }
    private IEnumerator ResetPosition(float timeLeft)
    {
        var pos = this.transform.position;
        while(pos != m_basePos){
            pos = Vector3.Lerp(this.MainCamera.transform.position, m_basePos, Time.deltaTime);
            this.MainCamera.transform.position = pos;
            yield return null;
        }
    }
    
    /// <summary>
    /// カメラ位置一時停止.
    /// </summary>
    public void StopPosition()
    {
        this.StopCoroutine("UpdatePosition");
    }
    
    private GameObject m_targetObj;
    private Vector3 m_basePos;
}
