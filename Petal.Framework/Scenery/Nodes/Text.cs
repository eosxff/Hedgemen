using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework.Graphics;

namespace Petal.Framework.Scenery.Nodes;

public class Text : Node
{
	public string Message
	{
		get;
		set;
	} = string.Empty;

	public float Scale
	{
		get;
		set;
	} = 1.0f;
	
	public static Rectangle DefaultBounds
	{
		get;
		set;
	} = new(0, 0, 128, 32);

	public Color Color
	{
		get;
		set;
	} = Color.White;

	public ContentReference<SpriteFont> Font
	{
		get;
		set;
	}

	protected override Rectangle GetDefaultBounds()
	{
		return DefaultBounds;
	}

	protected override void OnUpdate(GameTime time, NodeSelection selection)
	{
	}

	protected override void OnDraw(GameTime time)
	{
		if (Scene is null || Font.Item is null)
			return;

		Scene.Renderer.Begin();
		
		Scene.Renderer.Draw(new RenderStringData
		{
			Font = Font.Item,
			Position = new Vector2(AbsoluteBounds.X, AbsoluteBounds.Y),
			Color = Color,
			Text = Message,
			Scale = Scale
		});

		Scene.Renderer.End();
	}

	protected override Rectangle CalculateBounds(Rectangle bounds)
	{
		var calculatedBounds = base.CalculateBounds(bounds);

		if (Font.Item is null)
			return calculatedBounds;

		var measuredMessage = Font.Item.MeasureString(Message);
		calculatedBounds.Width = (int)(measuredMessage.X * Scale);
		calculatedBounds.Height = (int)(measuredMessage.Y * Scale);

		return calculatedBounds;
	}
}