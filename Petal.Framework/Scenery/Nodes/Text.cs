using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Petal.Framework.Content;
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

	public Color Color
	{
		get;
		set;
	} = Color.White;

	public Color OutlineColor
	{
		get;
		set;
	} = Color.Black;

	public RegistryObject<SpriteFont> Font
	{
		get;
		set;
	}

	public Vector2 Outline
	{
		get;
		set;
	} = Vector2.Zero;

	public Text()
	{
		IsInteractable = false;
	}

	protected override Rectangle GetDefaultBounds()
	{
		if(Font?.Key.Content is null)
			return Rectangle.Empty;

		var messageSize = Font.Get().MeasureString(Message);
		return new Rectangle(0, 0, (int)(messageSize.X * Scale), (int)(messageSize.Y * Scale));
	}

	protected override void OnUpdate(GameTime time, NodeSelection selection)
	{
	}

	protected override void OnDraw(GameTime time)
	{
		if (Scene is null || !Font.HasValidKey)
			return;

		var textData = new RenderStringData
		{
			Font = Font.Get(),
			Position = new Vector2(AbsoluteBounds.X, AbsoluteBounds.Y),
			Color = Color,
			Text = Message,
			Scale = Scale
		};

		Scene.Renderer.Begin();

		if (Outline is { X: > 0, Y: > 0 })
		{
			var position = textData.Position;
			var resolutionScale = Scene.ViewportAdapter.GetScale();

			var outlineData = new RenderStringData
			{
				Font = Font.Get(),
				Position = new Vector2(AbsoluteBounds.X, AbsoluteBounds.Y),
				Color = OutlineColor,
				Text = Message,
				Scale = Scale
			};

			outlineData.Position = position + Vector2.UnitX * Outline.X / resolutionScale.X;
			Scene.Renderer.Draw(outlineData);

			outlineData.Position = position - Vector2.UnitX * Outline.X / resolutionScale.X;
			Scene.Renderer.Draw(outlineData);

			outlineData.Position = position + Vector2.UnitY * Outline.Y / resolutionScale.Y;
			Scene.Renderer.Draw(outlineData);

			outlineData.Position = position - Vector2.UnitY * Outline.Y / resolutionScale.Y;
			Scene.Renderer.Draw(outlineData);
		}

		Scene.Renderer.Draw(textData);

		Scene.Renderer.End();
	}

	protected override Rectangle CalculateBounds(Rectangle bounds)
	{
		var calculatedBounds = base.CalculateBounds(bounds);

		if (!Font.HasValidKey)
			return calculatedBounds;

		var measuredMessage = Font.Get().MeasureString(Message);
		calculatedBounds.Width = (int)(measuredMessage.X * Scale);
		calculatedBounds.Height = (int)(measuredMessage.Y * Scale);

		return calculatedBounds;
	}
}
