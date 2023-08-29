using System.Collections;

namespace Petal.Framework.Util.Coroutines;

public class Coroutine
{
	public IEnumerator Enumerator
	{
		get;
		internal set; // should this be public?
	}

	public Coroutine? WaitForCoroutine
	{
		get;
		internal set; // should this be public?
	}

	public bool IsFinished
	{
		get;
		private set;
	}

	public void Stop()
	{
		IsFinished = true;
	}
}
