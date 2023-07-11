using Microsoft.Xna.Framework.Graphics;

namespace Petal.Framework.Graphics;

public sealed class DefaultRenderer : Renderer
{
	private readonly SpriteBatch _renderer;

	public DefaultRenderer()
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
		if (data.Texture is null)
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

	public override void DrawNinePatch(RenderData data, NinePatch sourcePatch, NinePatch destinationPatch)
	{
		if (data.Texture is null)
			return;

		var sourcePatches = sourcePatch.ToArray();
		var destinationPatches = destinationPatch.ToArray();
		
		for (int i = 0; i < 9; ++i)
		{
			var patchDrawData = new RenderData(data)
			{
				SrcRect = sourcePatches[i],
				DstRect = destinationPatches[i]
			};
			
			Draw(patchDrawData);
		}
	}

	public override void Draw(RenderStringData data)
	{
		if (data.Font is null)
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