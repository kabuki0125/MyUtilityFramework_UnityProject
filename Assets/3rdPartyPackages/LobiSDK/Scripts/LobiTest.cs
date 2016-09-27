#if UNITY_5
#define UNITY_5_AND_LATER
#endif

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Reflection;
using System;

using Kayac.Lobi.SDK;

/// <summary>
/// Lobi の動作確認用テストボタンを表示する
/// </summary>
public class LobiTest : MonoBehaviour
{
	private const string SETTING_FILE = "LobiSettings";
	private const string LOBI_REC_BRIDGE = "Kayac.Lobi.SDK.LobiRecBridge";
	private const string LOBI_RANKING_BRIDGE = "Kayac.Lobi.SDK.LobiRankingBridge";

	private float BUTTON_WIDTH = Math.Min( Screen.width, Screen.height) * 0.4f;
	private float BUTTON_HEIGHT = Math.Min( Screen.width, Screen.height) * 0.1f;
	private float BUTTON_SPACE = Math.Min( Screen.width, Screen.height) * 0.03f;

	enum MODE
	{
		NONE = -1,
		MAIN = 0,
		CORE,
		REC,
		RANKING
	};

	private static GUIStyle s_buttonStyle;

	private MODE _currentMode;
	private bool _isTestButtonsEnable;	// テストボタンを表示するか
	private bool _isCapturing;			// 録画中かどうか

	// 各SDKのクラス
	private Type _recType;
	private Type _rankingType;

	private float _displayScale;		// 1ポイントあたりのピクセル数。Retina なら 2.0 になる

	// Use this for initialization
	void Start ()
	{
		#if (UNITY_IOS || UNITY_IPHONE) && !UNITY_5_AND_LATER
		AudioSettings.outputSampleRate = 44100;
		#endif	

		_currentMode = MODE.MAIN;
		_displayScale = Math.Min (Screen.width, Screen.height) / 320.0f;
		_isCapturing = false;

		// テスト用ボタンを表示するかの設定値を取得
		LobiSettings settings = (LobiSettings)Resources.Load (SETTING_FILE);
		_isTestButtonsEnable = settings.IsValid == true && settings.IsEnabled == true && settings.testButtonsEnabled;
		if (_isTestButtonsEnable == false) {
			Debug.Log ("Invalid or missing game credentials, Lobi disabled");
			Debug.Log (settings.IsValid + ", " + settings.IsEnabled);
			return;
		}

		// 各SDKのクラスを取得する。Unityプロジェクトにインポートされていないクラスはnullが返る
		_recType = Type.GetType (LOBI_REC_BRIDGE);
		_rankingType = Type.GetType (LOBI_RANKING_BRIDGE);

		// テストボタンのスタイルを設定する
		s_buttonStyle = new GUIStyle();
		s_buttonStyle.wordWrap = true;
		s_buttonStyle.fontSize = (int)(BUTTON_HEIGHT * 0.5);
		s_buttonStyle.alignment = TextAnchor.MiddleCenter;
		s_buttonStyle.normal.textColor = new Color (0.1f, 0.1f, 0.1f);
		Texture2D texture = new Texture2D( 1, 1, TextureFormat.ARGB32, false );
		texture.SetPixel(0,0, new Color(1.0f, 1.0f, 1.0f, 0.8f) );
		texture.Apply();
		s_buttonStyle.normal.background = texture;
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	void OnGUI ()
	{
		if (_isTestButtonsEnable == false) {
			return;
		}

		switch (_currentMode) {
		case MODE.MAIN:
			int index = 0;
			if (_recType != null) {
				if (CustomButton (GetButtonRect (index), "LobiRec")) {
					_currentMode = MODE.REC;
				}	
				index++;
			}
			if (_rankingType != null) {
				if (CustomButton (GetButtonRect (index), "LobiRanking")) {
					_currentMode = MODE.RANKING;
				}	
			}
			break;
		case MODE.REC:
			if (CustomButton (GetButtonRect (0), "<- back")) {
				_recType.GetMethod ("StopCapturing").Invoke (null, null);
				_isCapturing = false;
				_currentMode = MODE.MAIN;
			}
			if (_isCapturing == false) {
				if (CustomButton (GetButtonRect (1), "StartCapturing")) {
					_recType.GetMethod ("SetMicEnable").Invoke (null, new object[]{ true });
					_recType.GetMethod ("SetMicVolume").Invoke (null, new object[]{ 1.0f });
					_recType.GetMethod ("SetGameSoundVolume").Invoke (null, new object[]{ 0.2f });
					_recType.GetMethod ("SetLiveWipeStatus", new Type[] { Type.GetType (LOBI_REC_BRIDGE + "+LiveWipeStatus") }).Invoke (null, new object[]{ 1 });
					_recType.GetMethod ("SetWipePositionX").Invoke (null, new object[]{ Screen.width / _displayScale - 100.0f - 10.0f });
					_recType.GetMethod ("SetWipePositionY").Invoke (null, new object[]{ 10.0f });
					_recType.GetMethod ("SetWipeSquareSize").Invoke (null, new object[]{ 100.0f });
					_recType.GetMethod ("SetPreventSpoiler").Invoke (null, new object[]{ false });
					_recType.GetMethod ("SetCapturePerFrame").Invoke (null, new object[]{ 2 });
					_recType.GetMethod ("StartCapturing").Invoke (null, null);
					_isCapturing = true;
				}
			} else {
				if (CustomButton (GetButtonRect (1), "StopCapturing")) {
					_recType.GetMethod ("StopCapturing").Invoke (null, null);
					_isCapturing = false;
				}
			}
			if (CustomButton(GetButtonRect(2), "PresentLobiPost")){
				_recType.GetMethod("PresentLobiPost",
					new Type[] { typeof(string),typeof(string),typeof(System.Int64),typeof(string),typeof(string)})
					.Invoke(null,new object[]{"sample title","sample description",0,"","{\"type\":\"sample video\",\"game_engine\":\"unity\"}"});
			}
			if (CustomButton(GetButtonRect(3), "PresentLobiPlay")){
				_recType.GetMethod("PresentLobiPlay",
					new Type[] {})
					.Invoke(null,null);
//				_recType.GetMethod("PresentLobiPlay",
//					new Type[] { typeof(string),typeof(string),typeof(bool),typeof(string)})
//					.Invoke(null,new object[]{"","",false,"{\"game_engine\":\"unity\"}"});
			}
			break;
		case MODE.RANKING:
			if (CustomButton(GetButtonRect(0), "<- back")){
				_currentMode = MODE.MAIN;
			}
			if (CustomButton(GetButtonRect(1), "PresentRanking")){
				_rankingType.GetMethod("PresentRanking").Invoke(null,null);
			}
			break;
		default:
			break;
		}
	}

	// 上から i_index 番目のボタンの Rect を返す
	Rect GetButtonRect(int i_index){
		return new Rect (BUTTON_SPACE, BUTTON_SPACE + (BUTTON_SPACE + BUTTON_HEIGHT) * i_index, BUTTON_WIDTH, BUTTON_HEIGHT);
	}

	// スタイルを指定した Button を返す
	private static bool CustomButton(Rect i_position, string i_text){
		return GUI.Button (i_position, i_text, s_buttonStyle);
	}
}
