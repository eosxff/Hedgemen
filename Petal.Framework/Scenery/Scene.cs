using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Petal.Framework.Graphics;
using Petal.Framework.Input;
using Petal.Framework.Scenery.Nodes;

namespace Petal.Framework.Scenery;

public class Scene
{
	// todo letterboxing
	
	public Renderer Renderer
	{
		get;
	} = null;
	
	public Color BackgroundColor
	{
		get;
		set;
	} = Color.CornflowerBlue;

	public Stage Root
	{
		get;
	}

	public InputProvider Input
	{
		get;
	}

	public NodeSelection NodeSelector
	{
		get;
	} = new ();

	public Skin Skin
	{
		get;
		set;
	}
	
	public event EventHandler BeforeUpdate;
	public event EventHandler AfterUpdate;
	
	public event EventHandler BeforeDraw;
	public event EventHandler AfterDraw;

	public event EventHandler AfterInitialize;

	public event EventHandler BeforeExit;
	public event EventHandler AfterExit;
	
	private readonly Dictionary<NamespacedString, Node> _nodesInScene = new();

	public Node? FindNode(NamespacedString name)
	{
		_nodesInScene.TryGetValue(name, out var node);
		return node;
	}

	public Scene(Stage root, Skin skin)
	{
		Skin = skin;
		Input = new InputProvider();
		Root = root;
		Root.Scene = this;
		
		Renderer = new SceneRenderer();
		Renderer.RenderState.TransformationMatrix = Root.VirtualResolutionScaleMatrix;
		
		Root.OnVirtualResolutionChanged += stage =>
		{
			if (stage.Scene == null)
				return;
			
			stage.Scene.Renderer.RenderState.TransformationMatrix = Root.VirtualResolutionScaleMatrix;
		};
	}

	public void Update(GameTime time)
	{
		Input.Update(time, Renderer.RenderState.TransformationMatrix);
		
		BeforeUpdate?.Invoke(this, EventArgs.Empty);
		
		NodeSelector.Update();
		Root.SearchForTargetNode(NodeSelector);
		Root.Update(time, NodeSelector);
		DestroyAllMarkedNodes();

		AfterUpdate?.Invoke(this, EventArgs.Empty);
	}

	public void Draw(GameTime time)
	{
		Renderer.RenderState.Graphics.GraphicsDevice.Clear(BackgroundColor);
		
		BeforeDraw?.Invoke(this, EventArgs.Empty);
		
		Root.Draw(time);
		
		AfterDraw?.Invoke(this, EventArgs.Empty);
	}

	public void Exit()
	{
		BeforeExit?.Invoke(this, EventArgs.Empty);
		Renderer.Dispose();
		AfterExit?.Invoke(this, EventArgs.Empty);
	}

	private void DestroyAllMarkedNodes()
	{
		foreach (var node in _nodesInScene.Values)
		{
			if (node.MarkedForDeletion)
				node.Destroy();
		}
	}

	public void Initialize()
	{
		AfterInitialize?.Invoke(this, EventArgs.Empty);
	}
	
	internal void InternalAddNode(Node node)
	{
		_nodesInScene.Add(node.Name, node);
		node.Scene = this;
	}

	internal void InternalRemoveNode(Node node)
	{
		_nodesInScene.Remove(node.Name);
		node.Scene = null;
	}
}