using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace Petal.Framework.Scenery.Nodes;

public class Stage : Node
{
	// we are currently making this thread safe, maybe it shouldn't be?
	private Dictionary<NamespacedString, Node> _nodes = new();

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

				if (target == null)
					continue;
				
				selection.Target = target;
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

	public Node? Find(NamespacedString name)
	{
		lock(_nodes)
		{
			_nodes.TryGetValue(name, out var node);
			return node;
		}
	}
	
	public void DestroyAllMarkedNodes()
	{
		lock (_nodes)
		{
			foreach (var node in _nodes.Values)
			{
				if (node.IsMarkedForDeletion)
					node.InternalDestroy();
			}
		}
	}

	internal void InternalAddNode(Node node)
	{
		lock (_nodes)
		{
			node.Name = SanitizeNodeName(node);
			_nodes.Add(node.Name, node);
		}
	}

	private NamespacedString SanitizeNodeName(Node node)
	{
		if (!_nodes.ContainsKey(node.Name))
			return node.Name;
		
		var name = node.Name;
		var duplicateCount = 1;
			
		do
		{
			name.Name = $"{node.Name.Name}-{duplicateCount}";
			duplicateCount++;
		} while (_nodes.ContainsKey(name));

		return name;
	}

	internal bool InternalRemoveNode(Node node)
	{
		lock(_nodes)
		{
			return _nodes.Remove(node.Name);
		}
	}

	internal void InternalUpdateNodeEntry(NamespacedString oldName)
	{
		lock (_nodes)
		{
			if (!_nodes.ContainsKey(oldName))
				return;
			
			var node = _nodes[oldName];
			_nodes.Remove(oldName);
			_nodes.Add(node.Name, node); // node.Name should already be updated
		}
	}
}