using Microsoft.Xna.Framework;

namespace Petal.Framework.Util.Coroutines;

public class WaitForSeconds(float seconds) : ICoroutineUpdateable
{
	public float Seconds
	{
		get;
	} = seconds;

	private float _elapsedTimeInSeconds = 0.0f;

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
