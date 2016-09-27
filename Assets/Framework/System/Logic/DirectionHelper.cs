/**
 * @file    DirectionHelper.cs
 * @brief
 *
 * @author  $Author$
 * @date    $Date$
 * @version $Rev$
 */
namespace MyLibrary.Unity
{
    using UnityEngine;
    using System.Collections;
    
    /// <summary>8方向の向き列挙.</summary>
    public enum Direction8
    {
        Up,
        UpRight,
        Right,
        DownRight,
        Down,
        DownLeft,
        Left,
        UpLeft,
        None,
    }
    
    /// <summary>
    /// クラス：向きサポートクラス
    /// </summary>
    public static class DirectionHelper
    {
        /// <summary>
        /// 指定角度(ディグリー角度)を8方向enumに変換.
        /// </summary>
        /// <returns>ディグリー角度に対応するDirection8.</returns>
        /// <param name="degree">ディグリー角度.</param>
        public static Direction8 Deg2Dir8(float degree)
        {
			if (0 <= degree && degree < 22.5f) {
				return Direction8.Left;         // 左

			} else if (22.5f <= degree && degree < 67.5f) {
				return Direction8.DownLeft;     // 左斜め下

			} else if (67.5f <= degree && degree < 112.5f) {
				return Direction8.Down;         // 下

			} else if (112.5f <= degree && degree < 157.5f) {
				return Direction8.DownRight;    // 右斜め下

			} else if (157.5f <= degree && degree < 202.5f) {
				return Direction8.Right;        // 右

			} else if (202.5f <= degree && degree < 247.5f) {
				return Direction8.UpRight;      // 右斜め上

			} else if (247.5f <= degree && degree < 292.5f) {
				return Direction8.Up;           // 上

			} else if (292.5f <= degree && degree < 337.5f) {
				return Direction8.UpLeft;       // 左斜め上

			} else if (337.5 <= degree && degree <= 360f) {
				return Direction8.Left;         // 左
			}
            return Direction8.None;
        }
    }
}