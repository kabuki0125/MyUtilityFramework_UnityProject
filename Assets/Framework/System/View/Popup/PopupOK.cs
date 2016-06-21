using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ポップアップ：閉じるのみ
/// </summary>
public class PopupOK : ViewBase
{
	public void Init(string msg, Action didTapOk)
	{
		m_didTapOk	= didTapOk;

		this.GetScript<UILabel>("Lab_popup").text	= msg;
		this.SetButtonMsg("btn_close", "DidTapOk");
	}
	
	void PopupClose()
	{
		GameObject.Destroy(this.gameObject);
	}
	
	void DidTapOk()
	{
		if( m_didTapOk != null ){
			m_didTapOk();
		}
		PopupClose();
	}
	
	private Action	m_didTapOk;
}

