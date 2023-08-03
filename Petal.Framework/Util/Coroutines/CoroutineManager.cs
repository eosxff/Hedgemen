using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Petal.Framework.Util.Coroutines;

public sealed class CoroutineManager
{
	private readonly object _lock = new(); // todo multithreading needs some work
	private readonly LinkedList<Coroutine> _unblockedCoroutines = new();
	private readonly LinkedList<Coroutine> _runningNextFrame = new();

	private CoroutineManagerState _state = CoroutineManagerState.None;

	public bool HasState(CoroutineManagerState state)
	{
		lock (_lock)
		{
			return state == _state;
		}
	}

	public void RequestToClearAllCoroutines()
		=> AddToState(CoroutineManagerState.ClearRequested);

	private void AddToState(CoroutineManagerState state)
	{
		lock (_lock)
		{
			_state |= state;
		}
	}

	private void RemoveFromState(CoroutineManagerState state)
	{
		lock (_lock)
		{
			_state &= ~state;
		}
	}

	private void ClearAllCoroutines()
	{
		lock (_lock)
		{
			_unblockedCoroutines.Clear();
			_runningNextFrame.Clear();
			RemoveFromState(CoroutineManagerState.ClearRequested);
		}
	}

	// preferably we would be able to start coroutines in any arbitrary thread
	public Coroutine? StartCoroutine(IEnumerator enumerator)
	{
		var coroutine = new Coroutine
		{
			Enumerator = enumerator
		};

		bool shouldContinueCoroutine = TickCoroutine(coroutine);

		if (!shouldContinueCoroutine)
			return null;

		if(HasState(CoroutineManagerState.InUpdate))
			_runningNextFrame.AddLast(coroutine);
		else
			_unblockedCoroutines.AddLast(coroutine);

		return coroutine;
	}

	public void Update(GameTime gameTime)
	{
		AddToState(CoroutineManagerState.InUpdate);

		if (HasState(CoroutineManagerState.ClearRequested))
		{
			ClearAllCoroutines();
		}

		foreach (var coroutine in _unblockedCoroutines)
		{
			if (coroutine.IsFinished)
				continue;

			if (coroutine.WaitForCoroutine != null)
			{
				if (coroutine.WaitForCoroutine.IsFinished)
					coroutine.WaitForCoroutine = null;
				else
				{
					_runningNextFrame.AddLast(coroutine);
					continue;
				}
			}

			if (coroutine.Enumerator.Current is ICoroutineUpdateable updateable &&
			    !updateable.ShouldFinish())
			{
				updateable.Update(gameTime);
				_runningNextFrame.AddLast(coroutine);
				continue;
			}

			if(TickCoroutine(coroutine))
				_runningNextFrame.AddLast(coroutine);
		}

		_unblockedCoroutines.Clear();

		foreach (var coroutine in _runningNextFrame)
			_unblockedCoroutines.AddLast(coroutine);

		_runningNextFrame.Clear();

		RemoveFromState(CoroutineManagerState.InUpdate);
	}

	private bool TickCoroutine(Coroutine coroutine)
	{
		if (!coroutine.Enumerator.MoveNext() || coroutine.IsFinished)
		{
			return false;
		}

		if (coroutine.Enumerator.Current is ICoroutineUpdateable updateable)
		{
			updateable.Tick();
		}

		else if (coroutine.Enumerator.Current is IEnumerator enumerator)
		{
			coroutine.WaitForCoroutine = StartCoroutine(enumerator);
		}

		else if (coroutine.Enumerator.Current is Coroutine coroutineCurrent)
		{
			coroutine.WaitForCoroutine = coroutineCurrent;
		}

		return true;
	}
}
