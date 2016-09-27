using System;
using UnityEngine;


namespace UnityStandardAssets._2D
{
    /// <summary>
    /// Standard Asset クラス：カメラの移動範囲制限.簡単なクラスだったので色々テコ入れしてる.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraFollow : MonoBehaviour
    {
        public float xMargin = 1f;  // カメラに追従する前に移動可能なx軸距離.
        public float yMargin = 1f;  // カメラに追従する前に移動可能なy軸距離.
        
        public float xSmooth = 8f;  // x方向のカメラ移動時のスムージング値.
        public float ySmooth = 8f;  // y方向のカメラ移動時のスムージング値.
        
        [SerializeField]
        private Vector2 maxXAndY;    // xとyの移動制限最大値.
        [SerializeField]
        private Vector2 minXAndY;    // xとyの移動制限最小値.

        /// <summary>共通インスタンス.</summary>
        public static CameraFollow SharedInstance { get; private set; }
        
        
        /// <summary>
        /// フォーカス対象を設定.自動的にフォーカス処理が開始される.
        /// </summary>
        public void ForcusTarget(Transform targetTransform)
        {
            m_forcusTarget = targetTransform;
        }
        
        void Update()
        {
            TrackPlayer();
        }
        private void TrackPlayer()
        {
            // フォーカス対象がいなければ何もしない.
            if(m_forcusTarget == null){
                return;
            }
            
            // 既定ではターゲットxとyカメラの座標は、現在のx座標とy座標.
            float targetX = transform.position.x;
            float targetY = transform.position.y;

            // プレイヤーがxマージンを超えて移動した場合.
            if (CheckXMargin()) {
                //ターゲットのx座標は、カメラの現在のx位置とプレイヤーの現在のx位置との間Lerpでなければならない.
                targetX = Mathf.Lerp(transform.position.x, m_forcusTarget.position.x, xSmooth*Time.deltaTime);
            }

            // プレイヤーがyマージンを超えて移動した場合.
            if (CheckYMargin()) {
                //ターゲットのy座標は、カメラの現在のy位置とプレイヤーの現在のy位置との間Lerpでなければならない.
                targetY = Mathf.Lerp(transform.position.y, m_forcusTarget.position.y+m_yFixVal, ySmooth*Time.deltaTime);
            }

            // ターゲットのx,y座標は最大値よりも大きく, または最小値より小さくならない.
            targetX = Mathf.Clamp(targetX, minXAndY.x, maxXAndY.x);
            targetY = Mathf.Clamp(targetY, minXAndY.y, maxXAndY.y);

            // ターゲット位置にカメラ位置を設定.(zは元の位置のまま.)
            transform.position = new Vector3(targetX, targetY, transform.position.z);
        }
        
        // プレイヤー間の距離がxMarginより大きい場合trueを返す.
        private bool CheckXMargin()
        {
            return Mathf.Abs(transform.position.x - m_forcusTarget.position.x) > xMargin;
        }
        // プレイヤー間の距離がyMarginより大きい場合trueを返す.
        private bool CheckYMargin()
        {
            return Mathf.Abs(transform.position.y - m_forcusTarget.position.y) > yMargin;
        }
        
        // 傾きを考慮した目視地点の修正.
        private void FixViewPoint()
        {
            var dif = Mathf.Abs(transform.localPosition.z) * Mathf.Tan(transform.eulerAngles.x*Mathf.Deg2Rad);
            m_yFixVal = dif;
            minXAndY = new Vector2(minXAndY.x, (minXAndY.y+m_yFixVal) / GetComponent<Camera>().orthographicSize);
            maxXAndY = new Vector2(maxXAndY.x, (maxXAndY.y+m_yFixVal) / GetComponent<Camera>().orthographicSize);
            Debug.Log("[CameraFollow] FixViewPoint : yFixVal="+dif);
        }
        
        /// <summary>
        /// xとyの移動制限最小値設定.
        /// </summary>
        /// <param name="XAndY">X and y.</param>
        public void SetMinXandY(Vector2 XAndY)
        {
            this.FixViewPoint();
            minXAndY = new Vector2(XAndY.x, (XAndY.y+m_yFixVal) / GetComponent<Camera>().orthographicSize);
        }
        /// <summary>
        /// xとyの移動制限最大値設定.
        /// </summary>
        public void SetMaxXandY(Vector2 XAndY)
        {
            this.FixViewPoint();
            maxXAndY = new Vector2(XAndY.x, (XAndY.y+m_yFixVal) / GetComponent<Camera>().orthographicSize);
        }
        
#if UNITY_EDITOR
        
        // 設定範囲がデフォのままだと見えないので見えるように.　※tsubakiさんのGistから拝借.
        void OnDrawGizmosSelected()
        {
            // カメラの移動可能な範囲(右下/左上)を取得
            Vector3 maxXMinY = new Vector3(maxXAndY.x, minXAndY.y);
            Vector3 minXMaxY = new Vector3 (minXAndY.x, maxXAndY.y);
            
            // カメラの移動範囲のGizmoを描画
            UnityEditor.Handles.DrawSolidRectangleWithOutline(
                new Vector3[]{ maxXAndY, maxXMinY, minXAndY, minXMaxY },
                new Color(1, 0, 0, 0.1f), Color.white );
            
            // カメラの描画する縦幅・横幅を取得
            Camera camera = GetComponent<Camera> ();
            float cameraWidthHalf = camera.orthographicSize * camera.aspect;
            Vector3 cameraMaxXMinY = new Vector2 (cameraWidthHalf, -camera.orthographicSize);
            Vector3 cameraMaxXMaxY = new Vector3 (cameraWidthHalf, camera.orthographicSize);
            
            // カメラの描画範囲のGizmoを描画描画          
            UnityEditor.Handles.DrawSolidRectangleWithOutline( new Vector3[] {
                maxXAndY + (Vector2)cameraMaxXMaxY, maxXMinY + cameraMaxXMinY,
                minXAndY - (Vector2)cameraMaxXMaxY, minXMaxY - cameraMaxXMinY
            }, new Color(1, 0, 0, 0.1f), Color.white);
        }
#endif
        
        void Awake()
        {
            if(SharedInstance != null){
                Debug.LogError("[CameraFollow] Error!! : Instance is already exist.");
                return;
            }
            SharedInstance = this;
            this.FixViewPoint();
        }
        
        private float m_yFixVal;            // 傾きを考慮したy座標修正値.
        private Transform m_forcusTarget;   // フォーカスターゲットのTransform参照.
    }
}
