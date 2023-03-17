using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace Petal.Framework.Scenery.Nodes;

public class Stage : Node
{
	public event NodeEvent<Stage> OnVirtualResolutionChanged;

	private Dictionary<NamespacedString, Node> _children = new();

	private Vector2Int _virtualResolution = new(640, 360);
	
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

			int windowWidth = Scene.Renderer.RenderState.Graphics.GraphicsDevice.Viewport.Width;
			int windowHeight = Scene.Renderer.RenderState.Graphics.GraphicsDevice.Viewport.Height;
			return new Vector2Int(windowWidth, windowHeight);
		}
	}

	public Vector2 VirtualResolutionScale
		=> new(
			WindowResolution.X / (float)VirtualResolution.X,
			WindowResolution.Y / (float)VirtualResolution.Y);

	public Matrix VirtualResolutionScaleMatrix
		=> Matrix.CreateScale(VirtualResolutionScale.X , VirtualResolutionScale.Y, 1.0f);

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
}