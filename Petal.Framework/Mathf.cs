using System;
using Microsoft.Xna.Framework;

namespace Petal.Framework;

public static class Mathf
{
	public static float Lerp(float value, float min, float max)
	{
		return (value - min) / (max - min);
	}

	public static Vector3 Torus(Vector2 position, int xMax, int yMax)
	{
		float x1 = 0, x2 = 1;
		float y1 = 0, y2 = 1;
		float dx = x2 - x1;
		float dy = y2 - y1;

		float s = position.X / xMax;
		float t = position.Y / yMax;

		float nx = (float)(x1 + Math.Cos (s * 2 * Math.PI) * dx / (2 * Math.PI));
		float ny = (float)(x1 + Math.Sin (s * 2 * Math.PI) * dx / (2 * Math.PI));
		float nz = t;

		return new Vector3(nx, ny, nz);
	}
}
