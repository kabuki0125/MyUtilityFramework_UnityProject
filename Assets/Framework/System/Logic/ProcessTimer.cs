using UnityEngine;
using System.Collections;

namespace MyLibrary.Unity
{
    /// <summary>
    /// 処理時間計測タイマークラス.
    /// </summary>
    public static class ProcessTimer
    {
        /// <summary>
        /// 計測開始してる？
        /// </summary>
        public static bool IsStartMeasurement { get { return m_startTime >= 0f; } }
        
        /// <summary>
        /// 計測開始.
        /// </summary>
        public static void StartMeasurement()
        {
            m_startTime = Time.realtimeSinceStartup;
        }
        
        /// <summary>
        /// 計測終了.予め計測始点にStartMeasurementを設定しておく必要がある.
        /// </summary>
        /// <returns>The measurement.</returns>
        public static float EndMeasurement()
        {
            if(m_startTime < 0f){
                Debug.LogError("[ProcessTimer] EndMeasurement Error!! : 処理時間を計測するにはStartMeasurementとEndMeasurementで処理を挟んでください.");
                return 0f;
            }
            return Time.realtimeSinceStartup - m_startTime;;
        }
        
        private static float m_startTime = -1f;
    }
}
