using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ポップアップ：Yes/No選択
/// </summary>
public class PopupYN : ViewBase
{
	public void Init(string msg, Action didTapYes, Action didTapNo)
	{
		m_didTapYes	= didTapYes;
		m_didTapNo	= didTapNo;

		this.GetScript<Text>("Lab_popup").text = msg;

		this.SetButtonMsg("btn_yes", DidTapYes);
		this.SetButtonMsg("btn_no", DidTapNo);
	}
	
	void PopupClose()
	{
		GameObject.Destroy(this.gameObject);
	}
	
	void DidTapYes()
	{
		if( m_didTapYes != null ){
			m_didTapYes();
		}
		PopupClose();
	}
	
	void DidTapNo()
	{
		if( m_didTapNo != null ){
			m_didTapNo();
		}
		PopupClose();
	}
	
	private Action	m_didTapYes;
	private Action	m_didTapNo;
}

