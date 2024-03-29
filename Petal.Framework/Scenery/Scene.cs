﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework.Graphics;
using Petal.Framework.Input;
using Petal.Framework.Scenery.Nodes;

namespace Petal.Framework.Scenery;

public abstract class Scene : IDisposable
{
	public event EventHandler<SkinChangedEventArgs>? OnSkinChanged;
	public event EventHandler? BeforeUpdate;
	public event EventHandler? AfterUpdate;
	public event EventHandler? BeforeDraw;
	public event EventHandler? AfterDraw;
	public event EventHandler? AfterInitialize;
	public event EventHandler? BeforeExit;
	public event EventHandler? AfterExit;
	public event EventHandler? OnViewportAdapterChanged;

	public bool IsFinishedLoading
	{
		get;
		private set;
	} = false;

	public PetalGame Game
	{
		get;
	}

	public NamespacedString Name
	{
		get;
		protected set;
	} = NamespacedString.Default;

	public Renderer Renderer
	{
		get;
	}

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
	} = new();

	public ViewportAdapter ViewportAdapter
	{
		get => _viewportAdapter;
		set
		{
			_viewportAdapter = value;
			_viewportAdapter.Reset();

			OnViewportAdapterChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public Skin Skin
	{
		get => _skin;
		set
		{
			var args = new SkinChangedEventArgs
			{
				OldSkin = _skin,
				NewSkin = value
			};

			_skin = value;
			_skin.Refresh();

			OnSkinChanged?.Invoke(this, args);
		}
	}

	private RenderTarget2D? _renderTarget;
	private ViewportAdapter _viewportAdapter;
	private Skin _skin = new();


	protected Scene(Stage root, Skin skin, PetalGame? game = null)
	{
		game ??= PetalGame.Petal;
		Game = game;

		Skin = skin;

		Input = new InputProvider();
		Root = root;
		Root.Scene = this;

		Renderer = new DefaultRenderer(game.GraphicsDevice);

		ViewportAdapter = new DefaultViewportAdapter(
			Renderer.RenderState.Graphics.GraphicsDevice, game.Window);

		Renderer.RenderState.TransformationMatrix = ViewportAdapter.GetScaleMatrix();

		Reset();
	}

	protected Scene()
	{
		Game = PetalGame.Petal;
		Input = new InputProvider();
		Root = new Stage
		{
			Scene = this
		};
		Renderer = new DefaultRenderer(Game.GraphicsDevice);
		ViewportAdapter = new DefaultViewportAdapter(
			Renderer.RenderState.Graphics.GraphicsDevice, Game.Window);

		Renderer.RenderState.TransformationMatrix = ViewportAdapter.GetScaleMatrix();

		Reset();
	}

	public void Update(GameTime time)
	{
		Input.Update(time, ViewportAdapter.GetScaleMatrix(), TransformCursorPosition);

		BeforeUpdate?.Invoke(this, EventArgs.Empty);

		NodeSelector.Update();
		Root.SearchForTargetNode(NodeSelector);
		Root.Update(time, NodeSelector);
		Root.PurgeAllMarkedNodes();

		AfterUpdate?.Invoke(this, EventArgs.Empty);
	}

	public void Draw(GameTime time)
	{
		Renderer.RenderState.TransformationMatrix = ViewportAdapter.GetScaleMatrix();

		var graphicsDevice = Renderer.RenderState.Graphics.GraphicsDevice;

		graphicsDevice.SetRenderTarget(_renderTarget);
		graphicsDevice.Clear(BackgroundColor);

		BeforeDraw?.Invoke(this, EventArgs.Empty);
		Root.Draw(time);
		AfterDraw?.Invoke(this, EventArgs.Empty);

		graphicsDevice.SetRenderTarget(null);

		ViewportAdapter.Reset();

		if (_renderTarget is not null)
		{
			Renderer.Begin();
			Renderer.Draw(new RenderData
			{
				Texture = _renderTarget,
				SrcRect = new Rectangle(0, 0, _renderTarget.Width, _renderTarget.Height),
				DstRect = new Rectangle(
					0,
					0,
					ViewportAdapter.VirtualResolution.X,
					ViewportAdapter.VirtualResolution.Y)
			});

			ViewportAdapter.Reset();
			Renderer.End();
		}
	}

	public void Exit()
	{
		BeforeExit?.Invoke(this, EventArgs.Empty);
		Dispose();
		OnDispose(); // should we call this before or after disposing everything else?
		AfterExit?.Invoke(this, EventArgs.Empty);
	}

	public void Load()
	{
		PetalGame.Petal.Window.ClientSizeChanged += OnWindowClientSizeChanged;
		ViewportAdapter.Reset();

		OnLoad();
		IsFinishedLoading = true;
		AfterInitialize?.Invoke(this, EventArgs.Empty);
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);

		PetalGame.Petal.Window.ClientSizeChanged -= OnWindowClientSizeChanged;
		_renderTarget?.Dispose();
		Renderer?.Dispose();
		ViewportAdapter?.Dispose();
	}

	protected virtual void OnDispose()
	{

	}

	protected virtual void OnLoad()
	{

	}

	private Vector2 TransformCursorPosition(Vector2 position)
	{
		var point = ViewportAdapter.PointToScreen(position.X, position.Y);
		return new Vector2(point.X, point.Y);
	}

	private void OnSkinRefreshed(object? sender, EventArgs args)
	{
		Root?.MarkNodeTreeAsDirty();
	}

	private void ResetRenderTarget()
	{
		var graphicsDevice = Renderer.RenderState.Graphics.GraphicsDevice;

		_renderTarget?.Dispose();
		_renderTarget = new RenderTarget2D(
			graphicsDevice,
			graphicsDevice.Viewport.Width,
			graphicsDevice.Viewport.Height,
			false,
			graphicsDevice.PresentationParameters.BackBufferFormat,
			DepthFormat.Depth24);
	}

	private void Reset()
	{
		ViewportAdapter.Reset();
		ResetRenderTarget();
	}

	private void OnWindowClientSizeChanged(object? sender, EventArgs args)
	{
		Reset();
	}

	public class SkinChangedEventArgs : EventArgs
	{
		public Skin OldSkin
		{
			get;
			init;
		}

		public Skin NewSkin
		{
			get;
			init;
		}
	}
}
