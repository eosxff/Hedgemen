namespace Petal.Framework;

public static class Mathf
{
	public static float Lerp(float value, float min, float max)
	{
		return (value - min) / (max - min);
	}
}
