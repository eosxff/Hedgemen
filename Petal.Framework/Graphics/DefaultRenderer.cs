using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Petal.Framework.Graphics;

public sealed class DefaultRenderer(GraphicsDevice graphicsDevice) : Renderer
{
	private readonly SpriteBatch _renderer = new(graphicsDevice);

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
			data.Origin ?? Vector2.Zero,
			data.SpriteEffects,
			data.LayerDepth);
	}

	public override void DrawNinePatch(RenderData data, NinePatch sourcePatch, NinePatch destinationPatch)
	{
		if (data.Texture is null)
			return;

		Span<Rectangle> sourcePatches = stackalloc Rectangle[9];
		Span<Rectangle> destinationPatches = stackalloc Rectangle[9];

		sourcePatches[0] = sourcePatch.TopLeft;
		sourcePatches[1] = sourcePatch.Top;
		sourcePatches[2] = sourcePatch.TopRight;

		sourcePatches[3] = sourcePatch.Left;
		sourcePatches[4] = sourcePatch.Center;
		sourcePatches[5] = sourcePatch.Right;

		sourcePatches[6] = sourcePatch.BottomLeft;
		sourcePatches[7] = sourcePatch.Bottom;
		sourcePatches[8] = sourcePatch.BottomRight;

		destinationPatches[0] = destinationPatch.TopLeft;
		destinationPatches[1] = destinationPatch.Top;
		destinationPatches[2] = destinationPatch.TopRight;

		destinationPatches[3] = destinationPatch.Left;
		destinationPatches[4] = destinationPatch.Center;
		destinationPatches[5] = destinationPatch.Right;

		destinationPatches[6] = destinationPatch.BottomLeft;
		destinationPatches[7] = destinationPatch.Bottom;
		destinationPatches[8] = destinationPatch.BottomRight;

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
			data.Origin ?? Vector2.Zero,
			data.Scale ?? 1.0f,
			data.SpriteEffects,
			data.LayerDepth);
	}

	public override void Dispose()
	{
		_renderer.Dispose();
	}
}
