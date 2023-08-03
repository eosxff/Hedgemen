using System;

namespace Petal.Framework.Util.Coroutines;

[Flags]
public enum CoroutineManagerState
{
	None = 1 << 0,
	InUpdate = 1 << 1,
	ClearRequested = 1 << 2
}
