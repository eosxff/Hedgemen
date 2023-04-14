using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework.Graphics;
using Petal.Framework.Input;
using Petal.Framework.Scenery.Nodes;

namespace Petal.Framework.Scenery;

public class Scene : IDisposable
{
	private readonly RenderTarget2D? _renderTarget = null;
	
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

	public ViewportAdapter ViewportAdapter
	{
		get;
		private set;
	}

	private Skin _skin = new();

	public Skin Skin
	{
		get => _skin;
		set
		{
			_skin = value;
			OnSkinChanged?.Invoke(this, EventArgs.Empty); // maybe add event args?
		}
	}

	public event EventHandler BeforeUpdate;
	public event EventHandler AfterUpdate;
	
	public event EventHandler BeforeDraw;
	public event EventHandler AfterDraw;

	public event EventHandler AfterInitialize;

	public event EventHandler BeforeExit;
	public event EventHandler AfterExit;

	public event EventHandler OnSkinChanged;

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

		Renderer = new DefaultRenderer();

		var graphicsDevice = Renderer.RenderState.Graphics.GraphicsDevice;

		Renderer.RenderState.TransformationMatrix = Root.VirtualResolutionScaleMatrix;
		Root.UpdateResolutionScalar();

		_renderTarget = new RenderTarget2D(
			graphicsDevice,
			graphicsDevice.PresentationParameters.BackBufferWidth,
			graphicsDevice.PresentationParameters.BackBufferHeight,
			false,
			graphicsDevice.PresentationParameters.BackBufferFormat,
			DepthFormat.Depth24);
		
		Root.OnVirtualResolutionChanged += stage =>
		{
			if (stage.Scene == null)
				return;
			
			stage.Scene.Renderer.RenderState.TransformationMatrix = Root.VirtualResolutionScaleMatrix;
			stage.Scene.Root.UpdateResolutionScalar();
		};

		ViewportAdapter = new BoxingViewportAdapter(
			Renderer.RenderState.Graphics.GraphicsDevice,
			PetalGame.Petal.Window,
			new Vector2Int(640, 360),
			new Vector2Int(0, 0));

		/*ViewportAdapter = new ScalingViewportAdapter(
			Renderer.RenderState.Graphics.GraphicsDevice,
			new Vector2Int(640, 360));*/
	}

	public void Update(GameTime time)
	{
		//Input.Update(time, Root.VirtualResolutionScaleMatrix);
		Input.Update(time, ViewportAdapter.GetScaleMatrix());

		BeforeUpdate?.Invoke(this, EventArgs.Empty);
		
		NodeSelector.Update();
		Root.SearchForTargetNode(NodeSelector);
		Root.Update(time, NodeSelector);
		DestroyAllMarkedNodes();

		AfterUpdate?.Invoke(this, EventArgs.Empty);
	}

	public void Draw(GameTime time)
	{
		var graphicsDevice = Renderer.RenderState.Graphics.GraphicsDevice;

		//graphicsDevice.SetRenderTarget(_renderTarget);
		
		graphicsDevice.Clear(BackgroundColor);

		Renderer.RenderState.TransformationMatrix = ViewportAdapter.GetScaleMatrix();
		
		BeforeDraw?.Invoke(this, EventArgs.Empty);
		Root.Draw(time);
		AfterDraw?.Invoke(this, EventArgs.Empty);
		
		//graphicsDevice.SetRenderTarget(null);
	}

	public void Exit()
	{
		BeforeExit?.Invoke(this, EventArgs.Empty);
		Dispose();
		AfterExit?.Invoke(this, EventArgs.Empty);
	}

	private void DestroyAllMarkedNodes()
	{
		foreach (var node in _nodesInScene.Values)
		{
			if (node.IsMarkedForDeletion)
				node.Destroy();
		}
	}

	public void Initialize()
	{
		var petal = PetalGame.Petal;
		
		petal.Window.ClientSizeChanged += OnWindowClientSizeChanged;
		Root.UpdateResolutionScalar();
		
		AfterInitialize?.Invoke(this, EventArgs.Empty);
	}

	private void OnWindowClientSizeChanged(object? sender, EventArgs args)
	{
		//Root.UpdateResolutionScalar();
		//Renderer.RenderState.TransformationMatrix = Root.VirtualResolutionScaleMatrix;

		ViewportAdapter.Reset();
		Renderer.RenderState.TransformationMatrix = ViewportAdapter.GetScaleMatrix();
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

	public void Dispose()
	{
		_renderTarget?.Dispose();
		Renderer?.Dispose();
	}
}