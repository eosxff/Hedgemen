using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Petal.Framework.Util;

namespace Petal.Framework.Scenery;

public sealed class SignalHandler
{
	private readonly Scene _scene;
	// todo maybe allow these signals to choose how/when to fire
	private readonly LinkedList<DeferredSignal> _deferredSignals = new();

	public SignalHandler(Scene scene)
	{
		_scene = scene;
	}

	public void Update(GameTime gameTime)
	{
		if (_deferredSignals.Count <= 0 || _deferredSignals.First is null)
			return;

		var signal = _deferredSignals.First.Value;

		if(!signal.HasBeenFired)
			signal.Fire(_scene);

		if (signal.ShouldStop())
		{
			signal.Stop();
			PopDeferredSignal();
			return;
		}

		signal.Update(gameTime);
	}

	public void Handle(ImmediateSignal signal)
	{
		PetalExceptions.ThrowIfNull(signal);
		signal.Fire(_scene);
	}

	public void Handle(DeferredSignal signal)
	{
		PetalExceptions.ThrowIfNull(signal);
		_deferredSignals.AddLast(signal);
	}

	private void PopDeferredSignal()
	{
		_deferredSignals.RemoveFirst();
	}
}
