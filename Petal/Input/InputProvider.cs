using System;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Petal.Input;

public class InputProvider : IKeyboardProvider, IMouseProvider
	{
		private StringBuilder _typedChars;

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
				switch (c)
				{
					case '\b': return;
					case '\n': _typedChars.Append('\n'); break;
				}
				
				_typedChars.Append(c); 
			};
		}
		
		public void Update(GameTime gameTime, Matrix scaleMatrix)
		{
			if(_shouldClearTypedChars)
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
			_cursorPosition = Vector2.Transform(_cursorPosition, Matrix.Invert(scaleMatrix));
		}

		public string GetTypedChars()
		{
			string chars = _typedChars.ToString();
			_shouldClearTypedChars = true;
			return chars;
		}

		public bool KeyDown(Keys key)
		{
			return _currentKeys.IsKeyDown(key);
		}

		public bool KeyPressed(Keys key)
		{
			return _previousKeys.IsKeyDown(key) && !_currentKeys.IsKeyDown(key);
		}

		public bool KeyReleased(Keys key)
		{
			return _currentKeys.IsKeyUp(key);
		}

		public bool AnyKeysDown(params Keys[] keys)
		{
			return keys.Any(KeyDown);
		}

		public bool AnyKeysPressed(params Keys[] keys)
		{
			return keys.Any(KeyPressed);
		}

		public bool AnyKeysReleased(params Keys[] keys)
		{
			return keys.Any(KeyReleased);
		}

		public Vector2 MousePosition => _cursorPosition;
		
		public Vector2 MousePositionDiff => new (0, 0); // todo
		
		public void UpdateMousePosition(Vector2 pos)
		{
			_cursorPosition = pos;
		}

		public bool MouseButtonClick(MouseButtons button)
		{
			return MouseStateFired(_currentButtons, button, ButtonState.Pressed);
		}

		public bool AnyMouseButtonClick(params MouseButtons[] buttons)
		{
			return buttons.Any(e => MouseStateFired(_currentButtons, e, ButtonState.Pressed));
		}

		public bool MouseButtonClicked(MouseButtons button)
		{
			return MouseStateFired(_previousButtons, button, ButtonState.Pressed) &&
					!MouseStateFired(_currentButtons, button, ButtonState.Pressed);
		}

		public bool AnyMouseButtonClicked(params MouseButtons[] buttons)
		{
			return buttons.Any(e => 
				MouseStateFired(_previousButtons, e, ButtonState.Pressed) && !MouseStateFired(_currentButtons, e, ButtonState.Pressed));
		}

		public bool MouseButtonUp(MouseButtons button)
		{
			return MouseStateFired(_currentButtons, button, ButtonState.Released);
		}

		public bool AnyMouseButtonUp(params MouseButtons[] buttons)
		{
			return buttons.Any(e => 
				MouseStateFired(_previousButtons, e, ButtonState.Released) && 
				!MouseStateFired(_currentButtons, e, ButtonState.Released));
		}

		public bool MouseButtonReleased(MouseButtons button)
		{
			return MouseStateFired(_currentButtons, button, ButtonState.Released) && MouseStateFired(_previousButtons, button, ButtonState.Pressed);
		}

		public bool AnyMouseButtonReleased(params MouseButtons[] buttons)
		{
			return buttons.Any(e => 
				MouseStateFired(_currentButtons, e, ButtonState.Released) && MouseStateFired(_previousButtons, e, ButtonState.Pressed));
		}

		public int MouseWheel => _currentButtons.ScrollWheelValue;

		public int MouseWheelChange => Math.Abs(_currentButtons.ScrollWheelValue - _previousButtons.ScrollWheelValue);

		private bool MouseStateFired(MouseState mouseState, MouseButtons button, ButtonState state)
		{
			switch (button)
			{
				case MouseButtons.LeftButton: return mouseState.LeftButton == state;
				case MouseButtons.MiddleButton: return mouseState.MiddleButton == state;
				case MouseButtons.RightButton: return mouseState.RightButton == state;
				case MouseButtons.XButton1: return mouseState.XButton1 == state;
				case MouseButtons.XButton2: return mouseState.XButton2 == state;
			}

			return false;
		}
	}