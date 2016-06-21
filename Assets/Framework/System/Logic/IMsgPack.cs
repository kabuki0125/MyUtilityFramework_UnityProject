using UnityEngine;
using System.Collections;

/// <summary>
/// インターフェイス ： メッセージパック解析
/// </summary>
public interface IMsgPack
{
	void Parse(object[] obj);
}