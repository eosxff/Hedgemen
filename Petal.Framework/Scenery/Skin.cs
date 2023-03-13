using Microsoft.Xna.Framework.Graphics;

namespace Petal.Framework.Scenery;

public sealed class Skin
{
	public ButtonData Button;

	public sealed class ButtonData
	{
		public Content<Texture2D> ButtonRegularTexture;
		public Content<Texture2D> ButtonHoverTexture;
		public Content<Texture2D> ButtonDownTexture;
	}
}