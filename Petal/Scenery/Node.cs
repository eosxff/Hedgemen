using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Petal.Framework.Scenery;

public delegate void NodeEvent<in TNode>(TNode node) where TNode : Node;

public abstract class Node
{
	public event NodeEvent<Node> OnBeforeDraw;
	public event NodeEvent<Node> OnAfterDraw;

	public event NodeEvent<Node> OnBeforeUpdate;
	public event NodeEvent<Node> OnAfterUpdate;

	public event NodeEvent<Node> OnDestroy;

	public event NodeEvent<Node> OnChildAdded;
	public event NodeEvent<Node> OnAddedToParent;
	
	public event NodeEvent<Node> OnChildRemoved;
	public event NodeEvent<Node> OnRemovedFromParent; 

	private readonly List<Node> _children = new();

	public IReadOnlyList<Node> Children
		=> _children;

	public string Tag
	{
		get;
		set;
	} = string.Empty;

	public NamespacedString Name
	{
		get;
		set;
	} = NamespacedString.Default;

	public bool IsVisible
	{
		get;
		set;
	} = true;

	public bool IsActive
	{
		get;
		set;
	} = true;

	public bool IsInteractable
	{
		get;
		set;
	} = true;

	public Anchor Anchor
	{
		get;
		set;
	} = Anchor.TopLeft;

	public Node? Parent
	{
		get;
		private set;
	} = null;

	public Scene? Scene
	{
		get;
		internal set;
	} = null;

	public Rectangle Bounds
	{
		get;
		set;
	}

	protected Node()
	{
		Bounds = GetDefaultBounds();
	}

	public void Update(GameTime time, NodeSelection selection)
	{
		OnBeforeUpdate?.Invoke(this);
		
		OnUpdate(time, selection);

		foreach (var child in Children)
		{
			child.Update(time, selection);
		}
		
		OnAfterUpdate?.Invoke(this);
	}

	protected virtual void OnUpdate(GameTime time, NodeSelection selection) { }

	protected virtual Rectangle GetDefaultBounds()
		=> Rectangle.Empty;

	public void Draw(GameTime time)
	{
		if (!IsVisible)
			return;
		
		OnBeforeDraw?.Invoke(this);
		
		OnDraw(time);

		foreach (var child in Children)
		{
			child.Draw(time);
		}
		
		OnAfterDraw?.Invoke(this);
	}

	protected virtual void OnDraw(GameTime time) { }

	public Node Add(Node child)
	{
		_children.Add(child);
		child.Parent = this;
		Scene?.AddNode(child);
		
		child.OnAddedToParent?.Invoke(child);
		
		OnChildAdded?.Invoke(this);
		
		return child;
	}

	public void Add(params Node[] children)
	{
		foreach (var child in children)
			Add(child);
	}

	public void Remove(Node child)
	{
		_children.Remove(child);
		child.Parent = null;
		Scene?.RemoveNode(child);
		
		child.OnRemovedFromParent?.Invoke(child);
		
		OnChildRemoved?.Invoke(this);
	}

	public virtual bool IsHovering(Vector2 position)
		=> Bounds.Contains((int) position.X, (int) position.Y);

	public virtual bool IsHoveringRecursive(Vector2 position)
	{
		foreach (var child in Children)
		{
			if (child.IsHoveringRecursive(position))
				return true;
		}

		return IsHovering(position);
	}

	public Node? GetHoveredNode(Vector2 position)
	{
		foreach (var child in Children)
		{
			if (!child.IsActive)
				continue;
			
			var node = child.GetHoveredNode(position);

			if (node != null)
				return node;
		}

		/*for (int i = Children.Count - 1; i >= 0; --i)
		{
			var child = Children[i];
			var node = child.GetHoveredNode(position);

			if (node != null)
				return node;
		}*/

		if(IsActive)
			return IsHovering(position) ? this : null;

		return null;
	}

	public void Destroy()
	{
		Console.WriteLine(Parent?.Name);
		/*foreach (var child in Children.ToList())
		{
			child.Destroy();
		}
		
		Parent?.Remove(this);
		Scene?.RemoveNode(this);
		
		OnDestroy?.Invoke(this);*/
		
		//Scene?.RemoveNode(this);
		Parent?.Remove(this);
	}
}