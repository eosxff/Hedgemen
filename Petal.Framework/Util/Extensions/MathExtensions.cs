using Microsoft.Xna.Framework;

namespace Petal.Framework.Util.Extensions;

public static class MathExtensions
{
	public static Rectangle ToRectanglePosition(this Vector2Int self)
		=> new(self.X, self.Y, 0, 0);

	public static Rectangle ToRectangleSize(this Vector2Int self)
		=> new(0, 0, self.X, self.Y);
}
