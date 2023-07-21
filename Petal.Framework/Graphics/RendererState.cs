using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Petal.Framework.Graphics;

public class RendererState
{
	public GraphicsDeviceManager Graphics
	{
		get;
		set;
	}

	public RenderTarget2D? RenderTarget
	{
		get;
		internal set;
	} = null;

	public Rectangle Scissor
	{
		get;
		set;
	}

	public SpriteSortMode SortMode
	{
		get;
		set;
	}

	public BlendState BlendState
	{
		get;
		set;
	}

	public SamplerState SamplerState
	{
		get;
		set;
	}

	public DepthStencilState DepthStencilState
	{
		get;
		set;
	}

	public Effect DefaultEffect
	{
		get;
		set;
	}

	public Matrix TransformationMatrix
	{
		get;
		set;
	}

	public RasterizerState RasterizerState
	{
		get;
		set;
	}

	public RendererState()
	{
		Graphics = PetalGame.Petal.Graphics;
		RasterizerState = new RasterizerState
		{
			CullMode = CullMode.CullCounterClockwiseFace,
			DepthBias = 0.0f,
			FillMode = FillMode.Solid,
			MultiSampleAntiAlias = false,
			ScissorTestEnable = true,
			SlopeScaleDepthBias = 0.0f
		};
		Scissor = new Rectangle(0, 0, 0, 0);
		SortMode = SpriteSortMode.Deferred;
		BlendState = BlendState.AlphaBlend;
		SamplerState = SamplerState.PointClamp;
		DepthStencilState = DepthStencilState.None;
		DefaultEffect = null;
		TransformationMatrix = Matrix.Identity;
	}
}