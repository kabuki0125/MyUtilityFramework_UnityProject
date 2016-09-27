using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// UICamera拡張クラス.
/// </summary>
public partial class UICamera
{
	public static List<UICamera> list { get { return new List<UICamera>(mList); } }

	public static Action<UICamera> DidCreateInstanceEvent = delegate{};
}