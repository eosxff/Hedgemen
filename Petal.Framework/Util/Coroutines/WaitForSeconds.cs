using Microsoft.Xna.Framework;

namespace Petal.Framework.Util.Coroutines;

public class WaitForSeconds : ICoroutineUpdateable
{
	public float Seconds
	{
		get;
	}

	private float _elapsedTimeInSeconds;

	public WaitForSeconds(float seconds)
	{
		Seconds = seconds;
		_elapsedTimeInSeconds = 0.0f;
	}
	public void Update(GameTime gameTime)
	{
		_elapsedTimeInSeconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
	}

	public void Tick()
	{
		_elapsedTimeInSeconds = 0.0f;
	}

	public bool ShouldFinish()
	{
		return _elapsedTimeInSeconds >= Seconds;
	}
}
