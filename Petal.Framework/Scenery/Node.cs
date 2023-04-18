using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Petal.Framework.Input;

namespace Petal.Framework.Scenery;

public delegate void NodeEvent<in TNode>(TNode node) where TNode : Node;

public abstract class Node
{
	public event EventHandler? OnBeforeDraw;
	public event EventHandler? OnAfterDraw;

	public event EventHandler? OnBeforeUpdate;
	public event EventHandler? OnAfterUpdate;

	public event EventHandler? OnDestroy;

	public event EventHandler? OnChildRemoved;
	public event EventHandler? OnRemovedFromParent;

	public event EventHandler? OnFocusGained;
	public event EventHandler? OnFocusLost;

	public event EventHandler? OnMouseHover;
	public event EventHandler? OnMouseDown;
	public event EventHandler? OnMousePressed;
	public event EventHandler? OnMouseReleased;

	public event EventHandler? OnParentChanged;

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

	public NodeState State
	{
		get;
		private set;
	} = NodeState.Default;

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

	private Node? _parent;

	public Node? Parent
	{
		get => _parent;
		private set
		{
			_parent = value;
			OnParentChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	private Scene? _scene;
	
	public Scene? Scene
	{
		get => _scene;
		internal set
		{
			_scene = value;
			OnSceneSet();
		}
	}

	private Rectangle _bounds = Rectangle.Empty;
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

	public void Update(GameTime time, NodeSelection selection)
	{
		ArgumentNullException.ThrowIfNull(Scene, nameof(Scene));
		
		if (_isDirty)
		{
			UpdateBounds();
			UpdateChildrenBounds();
		}
		
		OnBeforeUpdate?.Invoke(this, EventArgs.Empty);
		
		OnUpdate(time, selection);

		foreach (var child in Children)
		{
			child.Update(time, selection);
		}

		if (IsInteractable)
		{
			bool isTarget = selection.Target == this;
			bool isMouseDown = Scene.Input.MouseButtonClick(MouseButtons.LeftButton);
			bool isMousePressed = Scene.Input.MouseButtonClicked(MouseButtons.LeftButton);
			bool isMouseReleased = Scene.Input.MouseButtonReleased(MouseButtons.LeftButton);
			
			UpdateNodeState(selection, isMouseDown, isMousePressed, isMouseReleased);

			if(State == NodeState.Hover)
				OnMouseHover?.Invoke(this, EventArgs.Empty);
			
			if(State == NodeState.Input && isMouseReleased)
				OnMouseReleased?.Invoke(this, EventArgs.Empty);
			
			if(State == NodeState.Input && isMousePressed)
				OnMousePressed?.Invoke(this, EventArgs.Empty);
			
			if(State == NodeState.Input && isMouseDown)
				OnMouseDown?.Invoke(this, EventArgs.Empty);

			switch (isTarget)
			{
				case true when selection.PreviousTarget != this:
					OnFocusGained?.Invoke(this, EventArgs.Empty);
					break;
				case false when selection.PreviousTarget == this:
					OnFocusLost?.Invoke(this, EventArgs.Empty);
					break;
			}
		}
		
		OnAfterUpdate?.Invoke(this, EventArgs.Empty);
	}

	protected virtual void OnUpdate(GameTime time, NodeSelection selection) { }

	protected virtual Rectangle GetDefaultBounds()
		=> Rectangle.Empty;

	public void Draw(GameTime time)
	{
		ArgumentNullException.ThrowIfNull(Scene, $"{nameof(Scene)} cannot be null when being drawn!");
		
		if (!IsVisible)
			return;
		
		OnBeforeDraw?.Invoke(this, EventArgs.Empty);
		
		OnDraw(time);

		foreach (var child in Children)
		{
			child.Draw(time);
		}
		
		OnAfterDraw?.Invoke(this, EventArgs.Empty);
	}

	protected virtual void OnDraw(GameTime time) { }
	
	public TNode Add<TNode>(TNode child) where TNode : Node
	{
		_children.Add(child);
		child.Parent = this;
		child.Scene = Scene;
		Scene?.InternalAddNode(child);
		
		child.OnAddedToParent();
		OnChildAdded(child);
		
		return child;
	}

	protected virtual void OnAddedToParent()
	{
		
	}

	protected virtual void OnChildAdded(Node child)
	{
		
	}

	protected virtual void OnSceneSet()
	{
		
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
		Scene?.InternalRemoveNode(child);
		
		child.OnRemovedFromParent?.Invoke(this, EventArgs.Empty);
		
		OnChildRemoved?.Invoke(this, EventArgs.Empty);
	}

	public virtual bool IsHovering(Vector2 position)
		=> AbsoluteBounds.Contains((int) position.X, (int) position.Y);

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

	public virtual bool IsMarkedForDeletion
	{
		get;
		set;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void InternalDestroy()
		=> Destroy();

	protected virtual void Destroy()
	{
		foreach (var child in Children.ToList())
		{
			child.Destroy();
		}
		
		Parent?.Remove(this);
		Scene?.InternalRemoveNode(this);

		IsMarkedForDeletion = false;
		
		OnDestroy?.Invoke(this, EventArgs.Empty);
	}

	public bool IsAnyParentMarkedForDeletion
	{
		get
		{
			var parent = Parent;

			while (parent != null)
			{
				if (parent.IsMarkedForDeletion)
				{
					return true;
				}
				
				parent = parent.Parent;
			}

			return false;
		}
	}
	
	public bool IsAnyMarkedForDeletion
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => IsMarkedForDeletion || IsAnyParentMarkedForDeletion;
	}

	protected virtual Rectangle CalculateBounds(Rectangle bounds)
	{
		if (Scene == null)
			return bounds;
		
		var parentBounds = Scene.Root.AbsoluteBounds;

		if(Parent != null)
			parentBounds = Parent.AbsoluteBounds;

		var absBounds = bounds;

		switch (Anchor)
		{
			case Anchor.TopLeft:
				absBounds.X += parentBounds.Left;
				absBounds.Y += parentBounds.Top;
				break;
			case Anchor.Top:
				absBounds.X += parentBounds.Center.X - (bounds.Width / 2);
				absBounds.Y += parentBounds.Top;
				break;
			case Anchor.TopRight:
				absBounds.X += parentBounds.Right - (bounds.Width);
				absBounds.Y += parentBounds.Top;
				break;
			case Anchor.CenterLeft:
				absBounds.X += parentBounds.Left;
				absBounds.Y += parentBounds.Center.Y - (bounds.Height / 2);
				break;
			case Anchor.Center: 
				absBounds.X += parentBounds.Center.X - (bounds.Width / 2);
				absBounds.Y += parentBounds.Center.Y - (bounds.Height / 2);
				break;
			case Anchor.CenterRight:
				absBounds.X += parentBounds.Right - (bounds.Width);
				absBounds.Y += parentBounds.Center.Y - (bounds.Height / 2);
				break;
			case Anchor.BottomLeft:
				absBounds.X += parentBounds.Left;
				absBounds.Y += parentBounds.Bottom - (bounds.Height);
				break;
			case Anchor.Bottom:
				absBounds.X += parentBounds.Center.X - (bounds.Width / 2);
				absBounds.Y += parentBounds.Bottom - (bounds.Height);
				break;
			case Anchor.BottomRight:
				absBounds.X += parentBounds.Right - (bounds.Width);
				absBounds.Y += parentBounds.Bottom - (bounds.Height);
				break;
			default:
				throw new ArgumentOutOfRangeException(Anchor.ToString());
		}
		
		return absBounds;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void MarkAsDirty()
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
		if (relativeBounds == Rectangle.Empty)
			relativeBounds = GetDefaultBounds();
		
		_absoluteBounds = CalculateBounds(relativeBounds);
	}

	private void UpdateNodeState(NodeSelection selection, bool isMouseDown, bool isMousePressed, bool isMouseReleased)
	{
		State = NodeState.Default;
		
		if (Scene == null)
			return;

		bool isTarget = selection.Target == this;

		if (isTarget)
		{
			State = NodeState.Hover;
		}

		if ((isMouseDown || isMousePressed) && isTarget)
		{
			State = NodeState.Input;
		}
	}
}