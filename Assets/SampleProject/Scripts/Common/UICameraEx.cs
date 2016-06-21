using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// UICamera拡張クラス.なお、NGUI3.xでは不要.
/// </summary>
public partial class UICamera
{
	public static Action<UICamera> DidCreateInstanceEvent = delegate{};
	public static ReadOnlyCollection<UICamera> list { get { return mList.AsReadOnly(); } }
}