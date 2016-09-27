using System;
using System.Collections;
using System.Collections.Generic;

using System.Runtime.InteropServices;

namespace Kayac.Lobi.SDK
{
	public enum APIStatusCode {
		Success       = 0,
		NetworkError  = 100,
		ResponseError = 101,
		FatalError    = 102,
	}
}
