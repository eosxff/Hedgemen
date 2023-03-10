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
	public event NodeEvent<Node> OnBeforeDraw;
	public event NodeEvent<Node> OnAfterDraw;

	public event NodeEvent<Node> OnBeforeUpdate;
	public event NodeEvent<Node> OnAfterUpdate;

	public event NodeEvent<Node> OnDestroy;

	public event NodeEvent<Node> OnChildAdded;
	public event NodeEvent<Node> OnAddedToParent;
	
	public event NodeEvent<Node> OnChildRemoved;
	public event NodeEvent<Node> OnRemovedFromParent;

	public event NodeEvent<Node> OnFocusGained;
	public event NodeEvent<Node> OnFocusLost;

	public event NodeEvent<Node> OnMouseHover;
	public event NodeEvent<Node> OnMouseDown;
	public event NodeEvent<Node> OnMousePressed;
	public event NodeEvent<Node> OnMouseReleased;


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
		ArgumentNullException.ThrowIfNull(Scene, $"{nameof(Scene)} cannot be null when being updated!");
		
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

		if (IsInteractable)
		{
			bool isTarget = selection.Target == this;
			bool isMouseDown = Scene.Input.MouseButtonClick(MouseButtons.LeftButton);
			bool isMousePressed = Scene.Input.MouseButtonClicked(MouseButtons.LeftButton);
			bool isMouseReleased = Scene.Input.MouseButtonReleased(MouseButtons.LeftButton);
			
			UpdateNodeState(selection, isMouseDown, isMousePressed, isMouseReleased);
			
			if(State == NodeState.MouseHover)
				OnMouseHover?.Invoke(this);
			
			else if(State == NodeState.MouseDown && isMouseReleased)
				OnMouseReleased?.Invoke(this);
			
			else if(State == NodeState.MouseDown && isMousePressed)
				OnMousePressed?.Invoke(this);
			
			else if(State == NodeState.MouseDown && isMouseDown)
				OnMouseDown?.Invoke(this);
			
			if(isTarget && selection.PreviousTarget != this)
				OnFocusGained?.Invoke(this);
			
			else if(!isTarget && selection.PreviousTarget == this)
				OnFocusLost?.Invoke(this);
		}
		
		OnAfterUpdate?.Invoke(this);
	}

	protected virtual void OnUpdate(GameTime time, NodeSelection selection) { }

	protected virtual Rectangle GetDefaultBounds()
		=> Rectangle.Empty;

	public void Draw(GameTime time)
	{
		ArgumentNullException.ThrowIfNull(Scene, $"{nameof(Scene)} cannot be null when being drawn!");
		
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

		var absBounds = bounds;

		switch (Anchor)
		{
			case Anchor.TopLeft:
				absBounds.X += parentBounds.Left;
				absBounds.Y += parentBounds.Top;
				break;
			case Anchor.TopCenter:
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
			case Anchor.BottomCenter:
				absBounds.X += parentBounds.Center.X - (bounds.Width / 2);
				absBounds.Y += parentBounds.Bottom - (bounds.Height);
				break;
			case Anchor.BottomRight:
				absBounds.X += parentBounds.Right - (bounds.Width);
				absBounds.Y += parentBounds.Bottom - (bounds.Height);
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
		if (relativeBounds == Rectangle.Empty)
			relativeBounds = GetDefaultBounds();
		
		_absoluteBounds = CalculateBounds(relativeBounds);
	}

	private void UpdateNodeState(NodeSelection selection, bool isMouseDown, bool isMousePressed, bool isMouseReleased)
	{
		if (Scene == null)
			return;

		bool isTarget = selection.Target == this;

		if (isTarget)
		{
			State = NodeState.MouseHover;
		}

		if ((isMouseDown || isMousePressed) && isTarget)
		{
			State = NodeState.MouseDown;
		}
	}
}