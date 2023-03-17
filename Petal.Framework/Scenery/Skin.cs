using Microsoft.Xna.Framework.Graphics;

namespace Petal.Framework.Scenery;

public sealed class Skin
{
	public ButtonData Button
	{
		get;
		set;
	}

	public sealed class ButtonData
	{
		// todo should not be nullable
		public ContentReference<Texture2D>? ButtonRegularTexture;
		public ContentReference<Texture2D>? ButtonHoverTexture;
		public ContentReference<Texture2D>? ButtonDownTexture;
	}
}