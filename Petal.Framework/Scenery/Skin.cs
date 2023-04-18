using Microsoft.Xna.Framework.Graphics;

namespace Petal.Framework.Scenery;

public sealed class Skin
{
	public ContentRegistry Registry
	{
		get;
		init;
	}
	
	public ButtonData Button
	{
		get;
		init;
	}

	public sealed class ButtonData
	{
		public ContentReference<Texture2D> RegularTexture;
		public ContentReference<Texture2D> HoverTexture;
		public ContentReference<Texture2D> InputTexture;
	}
}