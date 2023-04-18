using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace Petal.Framework.Scenery.Nodes;

public class Stage : Node
{
	private Dictionary<NamespacedString, Node> _children = new();

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
	{
		ArgumentNullException.ThrowIfNull(Scene);

		var virtualResolution = Scene.ViewportAdapter.VirtualResolution;
		return new Rectangle(0, 0, virtualResolution.X, virtualResolution.Y);
	}
}