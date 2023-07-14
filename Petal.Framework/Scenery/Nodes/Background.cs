using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework.Graphics;

namespace Petal.Framework.Scenery.Nodes;

public class Background : Node
{
	public Color Color
	{
		get;
		set;
	} = Color.White;

	public ContentReference<Texture2D> Image
	{
		get;
		set;
	}
	
	public Background()
	{
		IsInteractable = false;
		IsActive = false;
	}

	protected override void OnUpdate(GameTime time, NodeSelection selection)
	{
	}

	protected override void OnDraw(GameTime time)
	{
		if (Scene is null || Image.Item is null)
			return;

		Scene.Renderer.Begin();

		Scene.Renderer.Draw(new RenderData
		{
			Texture = Image.Item,
			DstRect = AbsoluteBounds,
			Color = Color
		});

		Scene.Renderer.End();
	}
	
	protected override Rectangle CalculateBounds(Rectangle bounds)
	{
		if(Scene is null)
			return Rectangle.Empty;

		var virtualResolution = Scene.ViewportAdapter.VirtualResolution;
		return new Rectangle(0, 0, virtualResolution.X, virtualResolution.Y);
	}
}