using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// コルーチン目的だけでMonoBehaviourを継承するのを避けるため、
/// コルーチン処理の実行を代行するクラス。
/// メソッドチェーンで、コルーチン終了後に別のコルーチンを実行する事も出来る。
/// </summary>
public class CoroutineAgent : MonoBehaviour {

	/// <summary>
	/// コルーチンを色々する為のクラス
	/// </summary>
	public class CoroutineInfo {

		public int				ID { get; protected set; }

		protected IEnumerator	coroutine;
		protected CoroutineInfo	nextCoroutine;


		protected CoroutineInfo(){}


		/// <summary>コルーチン終了後に実行するコルーチンを設定する</summary>
		public CoroutineInfo Next( IEnumerator c ){
			nextCoroutine = new CoroutineInfoInternal( c );
			return nextCoroutine;
		}
		public CoroutineInfo Next( CoroutineInfo c ){
			nextCoroutine = c;
			return nextCoroutine;
		}
	}


	// CoroutineAgent内部処理用のCoroutineInfo
	private class CoroutineInfoInternal : CoroutineInfo {
		public IEnumerator		Coroutine { get{ return coroutine; } }
		public CoroutineInfo	NextCoroutine { get{ return nextCoroutine; } }

		public CoroutineInfoInternal( IEnumerator c ){
			ID = ++CoroutineAgent.counter;
			coroutine = c;
		}
	}




	private static CoroutineAgent			instance;
	private static int						counter;

	private Dictionary<int,Coroutine>		coroutines;



	void Awake(){
		coroutines = new Dictionary<int,Coroutine>();
		instance = this;
		GameObject.DontDestroyOnLoad( this.gameObject );
	}


	/// <summary>
	/// CoroutineAgentで使える形式のコルーチンデータを生成する
	/// </summary>
	public static CoroutineInfo CreateCoroutine( IEnumerator coroutine ){
		return new CoroutineInfoInternal( coroutine );
	}


	/// <summary>
	/// コルーチンを実行する
	/// </summary>
	/// <param name="coroutine">実行するコルーチン</param>
	/// <returns>登録したコルーチンを扱う為の情報</returns>
	public static CoroutineInfo Execute( IEnumerator coroutine ){
		return Execute( CreateCoroutine( coroutine ) );
	}
	public static CoroutineInfo Execute( CoroutineInfo c ){
		instance.ExecuteCoroutine( c );
		return c;
	}

	private void ExecuteCoroutine( CoroutineInfo c ){
		coroutines[c.ID] = this.StartCoroutine( _ExecuteCoroutine( c ) );
	}

	private static IEnumerator _ExecuteCoroutine( CoroutineInfo c ){
		var CII = c as CoroutineInfoInternal;
		yield return CII.Coroutine;
		instance.coroutines.Remove( CII.ID );
		if( CII.NextCoroutine != null ) instance.ExecuteCoroutine( CII.NextCoroutine );
	}


	/// <summary>
	/// 指定コルーチンを停止する。
	/// 次に処理するコルーチンがあり、それを実行したい場合はisExecNextにtrueを指定する
	/// </summary>
	public static void Stop( CoroutineInfo c, bool isExecNext = false ){
		if( instance.coroutines.ContainsKey( c.ID ) ){
			var CII = c as CoroutineInfoInternal;
			instance.StopCoroutine( CII.Coroutine );
			instance.coroutines.Remove( c.ID );
			if( ( CII.NextCoroutine != null ) && isExecNext ) instance.ExecuteCoroutine( CII.NextCoroutine );
		}
	}


	/// <summary>実行中の全てのコルーチンを停止する</summary>
	public static void StopAll(){
		foreach( var c in instance.coroutines ){
			instance.StopCoroutine( c.Value );
		}
		instance.coroutines.Clear();
	}


}
