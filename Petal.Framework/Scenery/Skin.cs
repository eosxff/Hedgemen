using System.Reflection;
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
		public ContentReference<Texture2D> RegularTexture;
		public ContentReference<Texture2D> HoverTexture;
		public ContentReference<Texture2D> InputTexture;
	}
}