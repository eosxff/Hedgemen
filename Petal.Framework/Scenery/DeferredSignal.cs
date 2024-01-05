using Microsoft.Xna.Framework;

namespace Petal.Framework.Scenery;

public abstract class DeferredSignal : ISignal
{
	public abstract void Fire(Scene scene);
	public abstract void Stop();
	public abstract bool ShouldStop();

	public virtual void Update(GameTime gameTime)
	{

	}
}
