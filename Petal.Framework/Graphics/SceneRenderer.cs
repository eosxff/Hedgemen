using System;
using Microsoft.Xna.Framework.Graphics;

namespace Petal.Framework.Graphics;

public sealed class SceneRenderer : Renderer
{
	private readonly SpriteBatch _renderer;
	
	public SceneRenderer()
	{
		_renderer = new SpriteBatch(PetalGame.Petal.GraphicsDevice);
	}
	
	public override void Begin()
	{
		_renderer.Begin(
			RenderState.SortMode,
			RenderState.BlendState,
			RenderState.SamplerState,
			RenderState.DepthStencilState,
			RenderState.RasterizerState,
			RenderState.DefaultEffect,
			RenderState.TransformationMatrix);
	}

	public override void End()
	{
		_renderer.End();
	}

	public override void Draw(RenderData data)
	{
		if (data.Texture == null)
			return;
		
		_renderer.Draw(
			data.Texture,
			data.DstRect,
			data.SrcRect,
			data.Color,
			data.Rotation,
			data.Origin,
			data.SpriteEffects,
			data.LayerDepth);
	}

	public override void Draw(RenderStringData data)
	{
		if (data.Font == null)
			return;
		
		_renderer.DrawString(
			data.Font,
			data.Text,
			data.Position,
			data.Color,
			data.Rotation,
			data.Origin,
			data.Scale,
			data.SpriteEffects,
			data.LayerDepth);
	}

	public override void Dispose()
	{
		_renderer.Dispose();
	}
}