using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Petal.Framework.Util.Extensions;

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
	{
		return self.Width > 0 && self.Height > 0;
	}

	public static Point ToPoint(this Vector2 self)
	{
		return new Point((int)self.X, (int)self.Y);
	}

	public static Vector2Int ToSize(this Rectangle self)
	{
		return new Vector2Int(self.Width, self.Height);
	}

	public static Vector2Int ToPosition(this Rectangle self)
	{
		return new Vector2Int(self.X, self.Y);
	}
}
