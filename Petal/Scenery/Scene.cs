using Microsoft.Xna.Framework;
using Petal.Graphics;
using Petal.Scenery.Nodes;

namespace Petal.Scenery;

public abstract class Scene
{
	public Color BackgroundColor
	{
		get;
		set;
	} = Color.CornflowerBlue;

	public Node Root
	{
		get;
		protected set;
	}

	public Renderer Renderer
	{
		get;
		protected set;
	}

	protected Scene()
	{
		
	}

	public void Initialize()
	{
		Renderer = new SceneRenderer();
		Root = new Group();
		AfterInitialize();
	}

	protected virtual void AfterInitialize()
	{
		
	}

	public void Update()
	{
		BeforeUpdate();
		
		AfterUpdate();
	}

	protected virtual void BeforeUpdate()
	{
		
	}

	protected virtual void AfterUpdate()
	{
		
	}

	public void Draw()
	{
		Renderer.RenderState.Graphics.GraphicsDevice.Clear(BackgroundColor);
		
		BeforeDraw();

		AfterDraw();
	}

	protected virtual void BeforeDraw()
	{
		
	}

	protected virtual void AfterDraw()
	{
		
	}

	public virtual void Exit()
	{
		
	}
}