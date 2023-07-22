using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Petal.Framework.Input;

public interface IKeyboardProvider
{
	public void Update(GameTime gameTime, Matrix scaleMatrix);

	public string GetTypedChars();

	public bool IsKeyDown(Keys key);
	public bool IsKeyPressed(Keys key);
	public bool IsKeyReleased(Keys key);

	public bool IsAnyKeyDown(params Keys[] keys);
	public bool IsAnyKeyPressed(params Keys[] keys);
	public bool IsAnyKeyReleased(params Keys[] keys);
}