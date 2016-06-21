/**
 * @file    CountingLock.cs
 * @brief
 *
 * @author  $Author$
 * @date    $Date$
 * @version $Rev$
 */
namespace MyLibrary.Unity
{
    using System;
    using System.Collections;

    /// <summary>
    /// クラス：ロック回数をカウントして状態変化を伝えるクラス.
    /// </summary>
    public class CountingLock
    {
        /// <summary>
		/// プロパティ：ロックされている？ ※trueを入れた回数をカウントする
        /// </summary>
        public bool IsLock
        {
            get {
                return m_bLock;
            }
            set {
                if( value ){
                    this.Lock();
                }else{
                    this.Unlock();
                }
            }
        }
        /// <summary>
        /// プロパティ：ロックカウント数
        /// </summary>
        public int Count
        {
            get {
                return m_cnt;
            }
        }


        /// <summary>
        /// コンストラクタ：ロック状態が変化したときに呼ばれるデリゲートを指定
        /// </summary>
        public CountingLock(Action<bool/*bLock*/> changeDelegate)
        {
            m_changeDelegate = changeDelegate;
            this.Reset();
        }
        private CountingLock(){}


        /// <summary>
        /// 初期状態に戻す
        /// </summary>
        public void Reset()
        {
            m_bLock = false;
            m_cnt = 0;
            m_changeDelegate(false);
        }

        /// <summary>
        /// ロックする
        /// </summary>
        public void Lock()
        {
            m_cnt++;
            m_bLock = m_cnt > 0;
            if( m_cnt == 1 ){
                m_changeDelegate(true);
            }
        }

        /// <summary>
        /// ロック解除
        /// </summary>
        public void Unlock()
        {
            if( m_cnt <= 0 ){
                return;
            }

            m_cnt--;
            m_bLock = m_cnt > 0;
            if( m_cnt == 0 ){
                m_changeDelegate(false);
            }
        }


        private Action<bool>    m_changeDelegate;       ///< ロック状態が変化したときに呼ばれるデリゲート
        private bool            m_bLock;
        private int             m_cnt;
    }
}
