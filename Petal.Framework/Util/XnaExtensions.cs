using Microsoft.Xna.Framework;

namespace Petal.Framework.Util;

public static class XnaExtensions
{
	public static bool IsBorderless(this GameWindow self)
	{
		return self.IsBorderlessEXT;
	}

	public static void SetBorderless(this GameWindow self, bool value)
	{
		self.IsBorderlessEXT = value;
	}

	public static bool HasSize(this Rectangle self)
		=> self is { Width: > 0, Height: > 0 };
}