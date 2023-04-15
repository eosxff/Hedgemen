using Petal.Framework.Graphics;

namespace Petal.Framework.Sandbox;

using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Framework;
using Windowing;

public class SandboxGame : PetalGame
{
	private class Animal
	{
		public virtual string AnimalName
			=> "Animal";
	}

	private class Dog : Animal
	{
		public override string AnimalName
			=> "Dog";
	}

	private class Cat : Animal
	{
		public override string AnimalName
			=> "Cat";
	}
	
	public static SandboxGame? Instance
	{
		get;
		private set;
	}

	public SandboxGame()
	{
		Instance = this;
	}

	public ContentRegistry GameContent
	{
		get;
	} = new ();

	private Renderer _renderer;
	private ViewportAdapter _viewportAdapter;
	private Texture2D _image;
	private RenderTarget2D _renderTarget;

	protected override void Initialize()
	{
		base.Initialize();

		_renderer = new DefaultRenderer();
		_viewportAdapter = new ScalingViewportAdapter(GraphicsDevice, new Vector2Int(640, 360));
		
		Window.AllowUserResizing = true;
		Window.ClientSizeChanged += WindowOnClientSizeChanged;
		_image = Assets.LoadAsset<Texture2D>(new FileInfo("peach.png").Open(FileMode.Open));
		_renderTarget = new RenderTarget2D(GraphicsDevice, 960, 540);
	}

	private void WindowOnClientSizeChanged(object? sender, EventArgs e)
	{
		//_renderTarget.Dispose();
		//_renderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
		//_viewportAdapter.Reset();
	}

	protected override void Draw(GameTime gameTime)
	{
		//_renderer.RenderState.TransformationMatrix = _viewportAdapter.GetScaleMatrix();
		_renderer.RenderState.TransformationMatrix = Matrix.Identity;
		
		GraphicsDevice.SetRenderTarget(_renderTarget);
		
		_renderer.Begin();
		
		_renderer.Draw(new RenderData
		{
			DstRect = new Rectangle(0, 0, 640, 360),
			Texture = _image
		});
		
		_renderer.End();
		
		GraphicsDevice.SetRenderTarget(null);
		_viewportAdapter.Reset();
		
		_renderer.Begin();
		
		_renderer.Draw(new RenderData
		{
			SrcRect = new Rectangle(0, 0, _renderTarget.Width, _renderTarget.Height),
			DstRect = new Rectangle(0, 0, 640, 360),
			Texture = _renderTarget
		});
		_viewportAdapter.Reset();

		_renderer.End();
	}

	protected override void Update(GameTime gameTime)
	{
		
	}

	protected override GameSettings GetInitialGameSettings()
	{
		return new GameSettings
		{
			PreferredFramerate = 60,
			Vsync = false,
			WindowWidth = 960,
			WindowHeight = 540,
			WindowMode = WindowMode.Windowed,
			IsMouseVisible = true
		};
	}

	private int _frames = 0;
	private Random _rng = new();
	private bool ShouldResetAnchor()
	{
		return false;
		_frames++;
		if(_frames % 60 == 0)
			Console.WriteLine($"Frames: {_frames}");
		return _frames % 60 == 0;
	}

	protected override void OnExiting(object sender, EventArgs args)
	{
		base.OnExiting(sender, args);
	}
}