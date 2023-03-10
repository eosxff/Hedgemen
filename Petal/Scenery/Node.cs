using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

	private Anchor _anchor = Anchor.TopLeft;
	
	public Anchor Anchor
	{
		get => _anchor;
		set
		{
			_anchor = value;
			MarkAsDirty();
		}
	}

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

	private Rectangle _bounds;
	private Rectangle _absoluteBounds;
	
	public Rectangle Bounds
	{
		get => _bounds;
		set
		{
			_bounds = value;
			MarkAsDirty();
		}
	}

	public Rectangle AbsoluteBounds
		=> _absoluteBounds;

	public Rectangle Size
		=> new(0, 0, Bounds.Width, Bounds.Height);

	private bool _isDirty = true;

	protected Node()
	{
		_bounds = GetDefaultBounds();
	}

	public void Update(GameTime time, NodeSelection selection)
	{
		if (_isDirty)
		{
			UpdateBounds();
			UpdateChildrenBounds();
		}
		
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
	
	public TNode Add<TNode>(TNode child) where TNode : Node
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
		for (int i = Children.Count - 1; i >= 0; --i)
		{
			var child = Children[i];

			if (!child.IsActive)
				continue;
			
			var node = child.GetHoveredNode(position);

			if (node != null)
				return node;
		}

		if(IsActive)
			return IsHovering(position) ? this : null;

		return null;
	}

	public void Destroy()
	{
		foreach (var child in Children.ToList())
		{
			child.Destroy();
		}
		
		Parent?.Remove(this);
		Scene?.RemoveNode(this);
		
		OnDestroy?.Invoke(this);
	}
	
	protected virtual Rectangle CalculateBounds(Rectangle bounds)
	{
		if (Scene == null)
			return bounds;
		
		var parentBounds = Scene.Root.AbsoluteBounds;

		if(Parent != null)
			parentBounds = Parent.AbsoluteBounds;

		var relBounds = bounds;
		var absBounds = relBounds;

		switch (Anchor)
		{
			case Anchor.TopLeft:
				absBounds.X += parentBounds.Left;
				absBounds.Y += parentBounds.Top;
				break;
			case Anchor.TopCenter:
				absBounds.X += parentBounds.Center.X - (relBounds.Width / 2);
				absBounds.Y += parentBounds.Top;
				break;
			case Anchor.TopRight:
				absBounds.X += parentBounds.Right - (relBounds.Width);
				absBounds.Y += parentBounds.Top;
				break;
			case Anchor.CenterLeft:
				absBounds.X += parentBounds.Left;
				absBounds.Y += parentBounds.Center.Y - (relBounds.Height / 2);
				break;
			case Anchor.Center: 
				absBounds.X += parentBounds.Center.X - (relBounds.Width / 2);
				absBounds.Y += parentBounds.Center.Y - (relBounds.Height / 2);
				break;
			case Anchor.CenterRight:
				absBounds.X += parentBounds.Right - (relBounds.Width);
				absBounds.Y += parentBounds.Center.Y - (relBounds.Height / 2);
				break;
			case Anchor.BottomLeft:
				absBounds.X += parentBounds.Left;
				absBounds.Y += parentBounds.Bottom - (relBounds.Height);
				break;
			case Anchor.BottomCenter:
				absBounds.X += parentBounds.Center.X - (relBounds.Width / 2);
				absBounds.Y += parentBounds.Bottom - (relBounds.Height);
				break;
			case Anchor.BottomRight:
				absBounds.X += parentBounds.Right - (relBounds.Width);
				absBounds.Y += parentBounds.Bottom - (relBounds.Height);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
		
		return absBounds;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool MarkAsDirty()
		=> _isDirty = true;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void MarkAsClean()
		=> _isDirty = false;

	private void UpdateBounds()
	{
		CalculateAbsoluteBounds(_bounds);
		MarkAsClean();
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void UpdateChildrenBounds()
	{
		CalculateAbsoluteBounds(_bounds);
		MarkAsClean();
		
		foreach (var child in Children)
		{
			child.UpdateChildrenBounds();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CalculateAbsoluteBounds(Rectangle relativeBounds)
	{
		_absoluteBounds = CalculateBounds(relativeBounds);
	}
}