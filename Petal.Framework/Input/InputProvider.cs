using System;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Petal.Framework.Input;

public class InputProvider : IKeyboardProvider, IMouseProvider
{
	private readonly StringBuilder _typedChars;

	private KeyboardState _currentKeys;
	private KeyboardState _previousKeys;

	private MouseState _currentButtons;
	private MouseState _previousButtons;

	private Vector2 _cursorPosition;

	private bool _shouldClearTypedChars = false;

	public InputProvider()
	{
		_typedChars = new StringBuilder();
		TextInputEXT.TextInput += c =>
		{
			if (!IsRecordingTypedChars)
				return;

			switch (c)
			{
				case '\b':
					return;

				case (char)0xD:
					_typedChars.Append('\n');
					break;
			}

			_typedChars.Append(c);
		};
	}

	public void Update(GameTime time, Matrix scaleMatrix)
	{
		Update(time, scaleMatrix, null);
	}

	public void Update(GameTime time, Matrix scaleMatrix, CursorPositionTransformation? cursorDelegate)
	{
		if (_shouldClearTypedChars)
		{
			_typedChars.Clear();
			_shouldClearTypedChars = false;
		}

		_previousKeys = _currentKeys;
		_currentKeys = Keyboard.GetState();

		_previousButtons = _currentButtons;
		_currentButtons = Mouse.GetState();

		var mouseState = Mouse.GetState();
		_cursorPosition = new Vector2(mouseState.X, mouseState.Y);

		if (cursorDelegate is null)
			_cursorPosition = Vector2.Transform(_cursorPosition, Matrix.Invert(scaleMatrix));
		else
			_cursorPosition = cursorDelegate(_cursorPosition);
	}

	public bool IsRecordingTypedChars
	{
		get;
		set;
	} = false;

	public string GetTypedChars()
	{
		string chars = _typedChars.ToString();
		_shouldClearTypedChars = true;
		IsRecordingTypedChars = false;
		return chars;
	}

	public bool IsKeyDown(Keys key)
	{
		return _currentKeys.IsKeyDown(key);
	}

	public bool IsKeyPressed(Keys key)
	{
		return _previousKeys.IsKeyDown(key) && !_currentKeys.IsKeyDown(key);
	}

	public bool IsKeyReleased(Keys key)
	{
		return _currentKeys.IsKeyUp(key);
	}

	public bool IsAnyKeyDown(params Keys[] keys)
	{
		return keys.Any(IsKeyDown);
	}

	public bool IsAnyKeyPressed(params Keys[] keys)
	{
		return keys.Any(IsKeyPressed);
	}

	public bool IsAnyKeyReleased(params Keys[] keys)
	{
		return keys.Any(IsKeyReleased);
	}

	public Vector2 MousePosition
		=> _cursorPosition;

	public Vector2 MousePositionDiff
		=> new(0, 0); // todo

	public void UpdateMousePosition(Vector2 pos)
	{
		_cursorPosition = pos;
	}

	public bool IsMouseButtonFired(MouseButtons button)
	{
		return MouseStateFired(_currentButtons, button, ButtonState.Pressed);
	}

	public bool IsAnyMouseButtonFired(params MouseButtons[] buttons)
	{
		return buttons.Any(e => MouseStateFired(_currentButtons, e, ButtonState.Pressed));
	}

	public bool IsMouseButtonClicked(MouseButtons button)
	{
		return MouseStateFired(_previousButtons, button, ButtonState.Pressed) &&
		       !MouseStateFired(_currentButtons, button, ButtonState.Pressed);
	}

	public bool IsAnyMouseButtonClicked(params MouseButtons[] buttons)
	{
		return buttons.Any(e =>
			MouseStateFired(_previousButtons, e, ButtonState.Pressed) &&
			!MouseStateFired(_currentButtons, e, ButtonState.Pressed));
	}

	public bool IsMouseButtonUp(MouseButtons button)
	{
		return MouseStateFired(_currentButtons, button, ButtonState.Released);
	}

	public bool IsAnyMouseButtonUp(params MouseButtons[] buttons)
	{
		return buttons.Any(e =>
			MouseStateFired(_previousButtons, e, ButtonState.Released) &&
			!MouseStateFired(_currentButtons, e, ButtonState.Released));
	}

	public bool IsMouseButtonReleased(MouseButtons button)
	{
		return MouseStateFired(_currentButtons, button, ButtonState.Released) &&
		       MouseStateFired(_previousButtons, button, ButtonState.Pressed);
	}

	public bool IsAnyMouseButtonReleased(params MouseButtons[] buttons)
	{
		return buttons.Any(e =>
			MouseStateFired(_currentButtons, e, ButtonState.Released) &&
			MouseStateFired(_previousButtons, e, ButtonState.Pressed));
	}

	public int MouseWheel
		=> _currentButtons.ScrollWheelValue;

	public int MouseWheelChange
		=> Math.Abs(_currentButtons.ScrollWheelValue - _previousButtons.ScrollWheelValue);

	private bool MouseStateFired(MouseState mouseState, MouseButtons button, ButtonState state)
	{
		switch (button)
		{
			case MouseButtons.LeftButton:
				return mouseState.LeftButton == state;
			case MouseButtons.MiddleButton:
				return mouseState.MiddleButton == state;
			case MouseButtons.RightButton:
				return mouseState.RightButton == state;
			case MouseButtons.XButton1:
				return mouseState.XButton1 == state;
			case MouseButtons.XButton2:
				return mouseState.XButton2 == state;
			default:
				return false;
		}

		return false;
	}
}
