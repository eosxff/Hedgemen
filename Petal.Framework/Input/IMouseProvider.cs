using Microsoft.Xna.Framework;

namespace Petal.Framework.Input;

public delegate Vector2 CursorPositionTransformation(Vector2 position);

public interface IMouseProvider
{
	public void Update(GameTime gameTime, Matrix scaleMatrix, CursorPositionTransformation? cursorDelegate = null);

	public Vector2 MousePosition
	{
		get;
	}

	public Vector2 MousePositionDiff
	{
		get;
	}

	public void UpdateMousePosition(Vector2 pos);

	public bool IsMouseButtonFired(MouseButtons button);
	public bool IsAnyMouseButtonFired(params MouseButtons[] buttons);

	public bool IsMouseButtonClicked(MouseButtons button);
	public bool IsAnyMouseButtonClicked(params MouseButtons[] buttons);

	public bool IsMouseButtonUp(MouseButtons button);
	public bool IsAnyMouseButtonUp(params MouseButtons[] buttons);

	public bool IsMouseButtonReleased(MouseButtons button);
	public bool IsAnyMouseButtonReleased(params MouseButtons[] buttons);

	public int MouseWheel
	{
		get;
	}
	public int MouseWheelChange
	{
		get;
	}
}