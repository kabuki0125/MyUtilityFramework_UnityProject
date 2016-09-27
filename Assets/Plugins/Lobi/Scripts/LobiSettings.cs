using UnityEngine;
using System.Collections;

/// <summary>
/// Lobi SDK の設定
/// </summary>
public class LobiSettings : ScriptableObject
{
	public string clientId;				// クライアントID
	public string baseName;				// BaseName
	
	public bool chatEnabled;			// Chat SDKがUnityプロジェクトにインポートされているか否か
	public bool recEnabled;				// Rec SDKがUnityプロジェクトにインポートされているか否か
	public bool rankingEnabled;			// Ranking SDKがUnityプロジェクトにインポートされているか否か

	public bool iosSupportEnabled;
	public bool androidSupportEnabled;
	
	public bool testButtonsEnabled;		// テストボタンを表示するか否か

	/// <summary>
	/// ビルドターゲットでビルド設定をするか
	/// </summary>
	public bool IsEnabled {
		get {
			#if UNITY_IPHONE
			return iosSupportEnabled;
			#elif UNITY_ANDROID
			return androidSupportEnabled;
			#else
			return false;
			#endif
		}
	}

	/// <summary>
	/// Lobi 設定値（Inspectorパネルで設定するクライアントIDなど）が有効か
	/// </summary>
	public bool IsValid {
		get {
			if(clientId != null && baseName != null) {
				if(clientId.Trim().Length > 0 && baseName.Trim().Length > 0) {
					return true;
				}
			}
			return false;
		}
	}
}