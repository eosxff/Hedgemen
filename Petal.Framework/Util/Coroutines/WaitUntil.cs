using System;
using Microsoft.Xna.Framework;

namespace Petal.Framework.Util.Coroutines;

public sealed class WaitUntil : ICoroutineUpdateable
{
	private readonly Func<bool> _predicate;

	public WaitUntil(Func<bool> predicate)
	{
		PetalExceptions.ThrowIfNull(predicate, nameof(predicate));
		_predicate = predicate;
	}

	public void Update(GameTime gameTime)
	{

	}

	public void Tick()
	{

	}

	public bool ShouldFinish()
	{
		return _predicate();
	}
}
