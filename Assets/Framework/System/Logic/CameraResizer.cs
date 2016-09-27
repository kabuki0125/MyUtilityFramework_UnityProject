using UnityEngine;
using System.Collections;


/// <summary>
/// クラス : カメラリサイザー.カメラのサイズを縦方向に併せてリサイズする.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraResizer : MonoBehaviour
{
    [SerializeField]
    private float width = 720f;     // 設定したい画面幅
    [SerializeField]
    private float height = 1280f;    // 設定したい画面高さ
    
    [SerializeField]
    private Color32 backGroundColor = Color.black;  // 背景色.
    
    [SerializeField]
    private float pixelPerUnit = 100f;  // 画像のPixel Per Unit.
    

	void Awake()
    {
        // 背景塗り潰し用カメラ作成.
        this.CreateBackgroundCamera();
        
        // 指定の縦の長さに合わせてカメラリサイズ.
        var cam = GetComponent<Camera>();
        cam.orthographicSize = height / 2f / pixelPerUnit;  // orthographicSizeがカメラの半分.合わせて画像サイズをPixelPerUnitで割った値を入れとく.
        
        // 余白を切り取る.縦、横両方対応.
        var aspect = (float)Screen.height / (float)Screen.width;
        var bgAcpect = height / width;
        if(bgAcpect > aspect){
            var bgScale = height / Screen.height;
            var camWidth = width / (Screen.width * bgScale);
            cam.rect = new Rect((1f - camWidth) / 2f, 0f, camWidth, 1f);    // ０〜１で正規化されているので合わせる.
        }else{
            var bgScale = width / Screen.width;
            var camHeight = height / (Screen.height * bgScale);
            cam.rect = new Rect(0f, (1f - camHeight) / 2f, 1f, camHeight);    // ０〜１で正規化されているので合わせる.
        }
    }
    
    void CreateBackgroundCamera()
    {
#if UNITY_EDITOR
        if(!UnityEditor.EditorApplication.isPlaying){
            return;
        }
#endif
        if(m_bgCamera != null){
            return;
        }
        var bgCamObj = new GameObject("Background Color Camera");
        m_bgCamera = bgCamObj.AddComponent<Camera> ();
        m_bgCamera.depth = -99;
        m_bgCamera.fieldOfView = 1;
        m_bgCamera.farClipPlane = 1.1f;
        m_bgCamera.nearClipPlane = 1; 
        m_bgCamera.cullingMask = 0;
        m_bgCamera.depthTextureMode = DepthTextureMode.None;
        m_bgCamera.backgroundColor = backGroundColor;
        m_bgCamera.renderingPath = RenderingPath.VertexLit;
        m_bgCamera.clearFlags = CameraClearFlags.SolidColor;
        m_bgCamera.useOcclusionCulling = false;
        m_bgCamera.hideFlags = HideFlags.NotEditable;
        
        this.gameObject.AddInChild(bgCamObj);
    }
    
    private Camera m_bgCamera;  // 背景を塗りつぶすようのカメラ.これがないと最後に描画したものが余白に残ったりなんだりする.
}
