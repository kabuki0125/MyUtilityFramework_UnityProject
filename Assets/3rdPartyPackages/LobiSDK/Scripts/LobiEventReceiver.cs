using UnityEngine;
using System.Collections;


namespace Kayac.Lobi.SDK
{
	public class LobiEventReceiver : MonoBehaviour {
		
		private static LobiEventReceiver instance = null;

		void Awake() {
			if(instance != null) {
				Destroy(this);
				return;
			}
			DontDestroyOnLoad(this.gameObject);
			instance = this;
		}
		
		public static LobiEventReceiver Instance {
			get {
				return instance;
			}
		}

		public delegate void DissmissedDelegate();
		private DissmissedDelegate _DissmissedAction = null;
		public DissmissedDelegate DissmissedAction {
			get {
				return _DissmissedAction;
			}
			set {
				#if UNITY_ANDROID && !UNITY_EDITOR
				Debug.Log("unsupported");
				#elif ((UNITY_IOS || UNITY_IPHONE) && ! UNITY_EDITOR)
				_DissmissedAction = value;
				#endif
			}
		}

		private void Dissmissed(string message){
			if (_DissmissedAction != null) {
				_DissmissedAction();
			}
		}

		public delegate void IsBoundWithLobiAccountDelegate(bool bound);
		public IsBoundWithLobiAccountDelegate IsBoundWithLobiAccountAction = null;

		private void IsBoundWithLobiAccount(string message){
			if (IsBoundWithLobiAccountAction != null) {
				IsBoundWithLobiAccountAction("1".Equals(message));
			}
		}
	}
}