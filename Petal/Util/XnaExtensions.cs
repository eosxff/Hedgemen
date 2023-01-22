using Microsoft.Xna.Framework;

namespace Petal.Util;

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
}