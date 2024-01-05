using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Petal.Framework.Input;
using Petal.Framework.Util;

namespace Petal.Framework.Scenery;

public abstract class Node
{
	public static NamespacedString GenerateDefaultNodeName(Node node)
		=> new($"{NamespacedString.DefaultNamespace}:{node.GetType().Name.ToLowerInvariant()}@{node.GetHashCode()}");

	public event EventHandler<ParentChangedEventArgs>? OnParentChanged;
	public event EventHandler<ChildAddedEventArgs>? OnChildAdded;
	public event EventHandler<ChildRemovedEventArgs>? OnChildRemoved;

	public event EventHandler? OnBeforeDraw;
	public event EventHandler? OnAfterDraw;
	public event EventHandler? OnBeforeUpdate;
	public event EventHandler? OnAfterUpdate;
	public event EventHandler? OnDestroy;
	public event EventHandler? OnFocusGained;
	public event EventHandler? OnFocusLost;
	public event EventHandler? OnMouseHover;
	public event EventHandler? OnMouseDown;
	public event EventHandler? OnMousePressed;
	public event EventHandler? OnMouseReleased;

	public IReadOnlyList<Node> Children
		=> _children;

	public int NestedChildrenCount
		=> GetNestedChildrenCount();

	public string Tag
	{
		get;
		set;
	} = string.Empty;

	public NamespacedString Name
	{
		get => _name;
		set
		{
			var oldName = _name;
			_name = value;

			Scene?.Root.InternalUpdateNodeEntry(oldName);
		}
	}

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
	} = NodeState.Normal;

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
		get => _parent;
		private set
		{
			var args = new ParentChangedEventArgs
			{
				OldParent = _parent,
				NewParent = value
			};

			_parent = value;
			OnParentChanged?.Invoke(this, args);
		}
	}

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

	public Vector2Int Size
	{
		get => new(Bounds.Width, Bounds.Height);
		set => Bounds = new Rectangle(_bounds.X, _bounds.Y, value.X, value.Y);
	}

	private bool _isDirty = true;

	protected Node()
	{
		Name = GenerateDefaultNodeName(this);
	}

	public void Update(GameTime time, NodeSelection selection)
	{
		PetalExceptions.ThrowIfNull(Scene, nameof(Scene));

		if (_isDirty)
		{
			UpdateBounds();
			UpdateChildrenBounds();
		}

		OnBeforeUpdate?.Invoke(this, EventArgs.Empty);

		OnUpdate(time, selection);

		foreach (var child in Children)
			child.Update(time, selection);

		if (IsInteractable)
		{
			bool isTarget = selection.Target == this;
			bool isMouseDown = Scene.Input.IsMouseButtonFired(MouseButtons.LeftButton);
			bool isMousePressed = Scene.Input.IsMouseButtonClicked(MouseButtons.LeftButton);
			bool isMouseReleased = Scene.Input.IsMouseButtonReleased(MouseButtons.LeftButton);

			UpdateNodeState(selection, isMouseDown, isMousePressed);

			if (State == NodeState.Hover)
				OnMouseHover?.Invoke(this, EventArgs.Empty);

			if (State == NodeState.Input && isMouseReleased)
				OnMouseReleased?.Invoke(this, EventArgs.Empty);

			if (State == NodeState.Input && isMousePressed)
				OnMousePressed?.Invoke(this, EventArgs.Empty);

			if (State == NodeState.Input && isMouseDown)
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

	public int GetNestedChildrenCount(int count = 0)
	{
		foreach (var child in Children)
			count = child.GetNestedChildrenCount(count);

		count += Children.Count;
		return count;
	}

	protected virtual void OnUpdate(GameTime time, NodeSelection selection)
	{
	}

	protected virtual Rectangle GetDefaultBounds()
		=> Rectangle.Empty;

	public void Draw(GameTime time)
	{
		PetalExceptions.ThrowIfNull(Scene, $"{nameof(Scene)} cannot be null when being drawn!");

		if (!IsVisible)
			return;

		OnBeforeDraw?.Invoke(this, EventArgs.Empty);

		OnDraw(time);

		foreach (var child in Children) child.Draw(time);

		OnAfterDraw?.Invoke(this, EventArgs.Empty);
	}

	protected virtual void OnDraw(GameTime time)
	{
	}

	public TNode Add<TNode>(TNode child) where TNode : Node
	{
		PetalExceptions.ThrowIfNull(child, nameof(child));

		if (ReferenceEquals(child, this))
			throw new InvalidOperationException($"Can not add node to itself.");

		if (_children.Contains(child))
			throw new InvalidOperationException($"{child} is already a child in node.");

		_children.Add(child);
		child.Parent = this;
		child.Scene = Scene;
		Scene?.Root.InternalAddNode(child);

		OnChildAdded?.Invoke(this, new ChildAddedEventArgs
		{
			Child = child
		});

		return child;
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
		var parentChangedArgs = new ParentChangedEventArgs
		{
			OldParent = Parent,
			NewParent = null
		};

		_children.Remove(child);
		child.Parent = null;
		Scene?.Root.InternalRemoveNode(child);

		child.OnParentChanged?.Invoke(this, parentChangedArgs);

		OnChildRemoved?.Invoke(this, new ChildRemovedEventArgs
		{
			Child = child
		});
	}

	public virtual bool IsHovering(Vector2 position)
		=> AbsoluteBounds.Contains((int)position.X, (int)position.Y);

	public virtual bool IsHoveringRecursive(Vector2 position)
	{
		foreach (var child in Children)
			if (child.IsHoveringRecursive(position))
				return true;

		return IsHovering(position);
	}

	public Node? GetHoveredNode(Vector2 position)
	{
		if (!IsInteractable)
			return null;

		for (int i = Children.Count - 1; i >= 0; --i)
		{
			var child = Children[i];

			if (!child.IsActive)
				continue;

			var node = child.GetHoveredNode(position);

			if (node is not null)
				return node;
		}

		if (IsActive)
			return IsHovering(position) ? this : null;

		return null;
	}

	public virtual bool IsMarkedForDeletion
	{
		get;
		set;
	}

	protected virtual void Destroy()
	{
		foreach (var child in Children.ToList()) child.Destroy();

		Parent?.Remove(this);
		Scene?.Root.InternalRemoveNode(this);

		IsMarkedForDeletion = false;

		OnDestroy?.Invoke(this, EventArgs.Empty);
	}

	public bool IsAnyParentMarkedForDeletion
	{
		get
		{
			var parent = Parent;

			while (parent is not null)
			{
				if (parent.IsMarkedForDeletion)
					return true;

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
		if (Scene is null)
			return bounds;

		var parentBounds = Parent?.AbsoluteBounds ?? Scene.Root.AbsoluteBounds;
		var calculatedBounds = new Rectangle(0, 0, bounds.Width, bounds.Height);

		switch (Anchor)
		{
			case Anchor.TopLeft:
				calculatedBounds.X = bounds.X + parentBounds.Left;
				calculatedBounds.Y = bounds.Y + parentBounds.Top;
				break;

			case Anchor.Top:
				calculatedBounds.X = bounds.X + parentBounds.Center.X - bounds.Width / 2;
				calculatedBounds.Y = bounds.Y + parentBounds.Top;
				break;

			case Anchor.TopRight:
				calculatedBounds.X = parentBounds.Right - bounds.Width - bounds.X;
				calculatedBounds.Y = bounds.Y + parentBounds.Top;
				break;

			case Anchor.CenterLeft:
				calculatedBounds.X = bounds.X + parentBounds.Left;
				calculatedBounds.Y = bounds.Y + parentBounds.Center.Y - bounds.Height / 2;
				break;

			case Anchor.Center:
				calculatedBounds.X = bounds.X + parentBounds.Center.X - bounds.Width / 2;
				calculatedBounds.Y = bounds.Y + parentBounds.Center.Y - bounds.Height / 2;
				break;

			case Anchor.CenterRight:
				calculatedBounds.X = parentBounds.Right - bounds.Width - bounds.X;
				calculatedBounds.Y = bounds.Y + parentBounds.Center.Y - bounds.Height / 2;
				break;

			case Anchor.BottomLeft:
				calculatedBounds.X = bounds.X + parentBounds.Left;
				calculatedBounds.Y = parentBounds.Bottom - bounds.Height - bounds.Y;
				break;

			case Anchor.Bottom:
				calculatedBounds.X = bounds.X + parentBounds.Center.X - bounds.Width / 2;
				calculatedBounds.Y = parentBounds.Bottom - bounds.Height - bounds.Y;
				break;

			case Anchor.BottomRight:
				calculatedBounds.X = parentBounds.Right - bounds.Width - bounds.X;
				calculatedBounds.Y = parentBounds.Bottom - bounds.Height - bounds.Y;
				break;
			default:

				throw new ArgumentOutOfRangeException(Anchor.ToString());
		}

		return calculatedBounds;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void MarkAsDirty()
		=> _isDirty = true;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void MarkAsClean()
		=> _isDirty = false;

	// PRIVATE

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
			child.UpdateChildrenBounds();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CalculateAbsoluteBounds(Rectangle relativeBounds)
	{
		if (relativeBounds == Rectangle.Empty)
			relativeBounds = GetDefaultBounds();

		_absoluteBounds = CalculateBounds(relativeBounds);
	}

	private void UpdateNodeState(NodeSelection selection, bool isMouseDown, bool isMousePressed)
	{
		State = NodeState.Normal;

		if (Scene is null)
			return;

		bool isTarget = selection.Target == this;

		if (isTarget)
			State = NodeState.Hover;

		if ((isMouseDown || isMousePressed) && isTarget)
			State = NodeState.Input;
	}

	private readonly List<Node> _children = new();
	private Anchor _anchor = Anchor.TopLeft;
	private Node? _parent;
	private Scene? _scene;
	private NamespacedString _name;

	// INTERNAL

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void InternalDestroy()
		=> Destroy();

	// INNER CLASSES

	public sealed class ChildAddedEventArgs : EventArgs
	{
		public required Node Child
		{
			get;
			init;
		}
	}

	public sealed class ChildRemovedEventArgs : EventArgs
	{
		public Node Child
		{
			get;
			init;
		}
	}

	public sealed class ParentChangedEventArgs : EventArgs
	{
		public Node? OldParent
		{
			get;
			init;
		}

		public Node? NewParent
		{
			get;
			init;
		}
	}
}
