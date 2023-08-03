using Microsoft.Xna.Framework;

namespace Petal.Framework.Util.Coroutines;

public interface ICoroutineUpdateable
{
	public void Update(GameTime gameTime);
	public void Tick();
	public bool ShouldFinish();
}
