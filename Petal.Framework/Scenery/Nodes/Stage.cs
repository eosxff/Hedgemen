using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Petal.Framework.Scenery.Nodes;

public class Stage : Node
{
	public event NodeEvent<Stage> OnVirtualResolutionChanged;

	private Dictionary<NamespacedString, Node> _children = new();

	private Vector2Int _virtualResolution = new(640, 360);

	public Vector2Int Bleed
	{
		get;
		set;
	}
	
	public Vector2Int VirtualResolution
	{
		get => _virtualResolution;
		set
		{
			_virtualResolution = value;
			Bounds = new Rectangle(0, 0, _virtualResolution.X, _virtualResolution.Y);
			OnVirtualResolutionChanged?.Invoke(this);
		}
	}

	public Vector2Int WindowResolution
	{
		get
		{
			if (Scene == null)
				throw new InvalidOperationException("Stage does not have a scene!");

			var graphicsDevice = Scene.Renderer.RenderState.Graphics.GraphicsDevice;
			int windowWidth = graphicsDevice.PresentationParameters.BackBufferWidth;
			int windowHeight = graphicsDevice.PresentationParameters.BackBufferHeight;
			
			return new Vector2Int(windowWidth, windowHeight);
		}
	}

	public Vector2Int ViewportResolution
	{
		get
		{
			if (Scene == null)
				throw new InvalidOperationException("Stage does not have a scene!");

			var viewport = Scene.Renderer.RenderState.Graphics.GraphicsDevice.Viewport;
			return new Vector2Int(viewport.Width - viewport.X, viewport.Height - viewport.Y);
			//return new Vector2Int(viewport.Width, viewport.Height);
		}
	}

	public Vector2 VirtualResolutionScale
		=> new(
			ViewportResolution.X / (float)VirtualResolution.X,
			ViewportResolution.Y / (float)VirtualResolution.Y);

	public Matrix VirtualResolutionScaleMatrix
		=> Matrix.CreateScale(VirtualResolutionScale.X, VirtualResolutionScale.Y, 1.0f);

	public float VirtualResolutionAspectRatio
		=> (float)VirtualResolution.X / VirtualResolution.Y;

	public Stage()
	{
		IsInteractable = false;
	}

	public void SearchForTargetNode(NodeSelection selection)
	{
		if (Scene == null)
			return;

		var mousePosition = Scene.Input.MousePosition;

		if (Children.Count > 0)
		{
			for (int i = Children.Count - 1; i >= 0; --i)
			{
				var child = Children[i];
				var target = child.GetHoveredNode(mousePosition);

				if (target != null)
				{
					selection.Target = target;
					break;
				}
			}
		}

		if (IsInteractable && IsHovering(mousePosition))
			selection.Target = this;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SearchForTargetNode(out NodeSelection selection)
	{
		selection = new NodeSelection();
		SearchForTargetNode(selection);
	}

	protected override Rectangle CalculateBounds(Rectangle bounds)
		=> new(0, 0, VirtualResolution.X, VirtualResolution.Y);

	public BoxingMode BoxingMode
	{
		get;
		private set;
	} = BoxingMode.None;
	
	public void UpdateResolutionScalar()
	{
		if (Scene == null)
			return;

		/*var clientBounds = PetalGame.Petal.Window.ClientBounds;

		var worldScale = new Vector2(
			(float) clientBounds.Width / VirtualResolution.X,
			(float) clientBounds.Height / VirtualResolution.Y);

		var safeScale = new Vector2(
			(float) clientBounds.Width / (VirtualResolution.X - Bleed.X),
			(float) clientBounds.Height / (VirtualResolution.Y - Bleed.Y));

		float worldScale1d = MathHelper.Max(worldScale.X, worldScale.Y);
		float safeScale1d = MathHelper.Min(safeScale.X, safeScale.Y);
		float scale = MathHelper.Min(worldScale1d, safeScale1d);

		int width = (int) (scale * VirtualResolution.X + 0.5f);
		int height = (int) (scale * VirtualResolution.Y + 0.5f);

		if (height >= clientBounds.Height && width < clientBounds.Width)
			BoxingMode = BoxingMode.PillarBox;
		else
		{
			if (width >= clientBounds.Height && height <= clientBounds.Height)
				BoxingMode = BoxingMode.LetterBox;
			else
				BoxingMode = BoxingMode.None;
		}

		int x = clientBounds.Width / 2 - width / 2;
		int y = clientBounds.Height / 2 - height / 2;

		Scene.Renderer.RenderState.Graphics.GraphicsDevice.Viewport = new Viewport(x, y, width, height);*/
	}
}