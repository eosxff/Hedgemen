using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Xna.Framework; // ;)

[Serializable]
public struct Vector2Int
{
	public static readonly Vector2Int Zero = new(0, 0);
	public static readonly Vector2Int One = new(1, 1);
	public static readonly Vector2Int UnitX = new(1, 0);
	public static readonly Vector2Int UnitY = new(0, 1);

	public int X;
	public int Y;

	public Vector2Int(int x, int y)
	{
		X = x;
		Y = y;
	}

	public Vector2Int(int value)
	{
		X = value;
		Y = value;
	}

	public bool Equals(Vector2Int vector)
	{
		return X == vector.X &&
		       Y == vector.Y;
	}

	public override bool Equals(object obj)
		=> obj is Vector2Int vector && Equals(vector);

	public override int GetHashCode()
		=> X.GetHashCode() + Y.GetHashCode();

	public int Length()
		=> (int) Math.Sqrt(X * X + Y * Y);

	public int LengthSquared()
		=> X * X + Y * Y;

	public override string ToString()
		=> $"{{X: {X} Y: {Y}}}";

	public static Vector2Int Add(Vector2Int a, Vector2Int b)
		=> new()
		{
			X = a.X + b.X,
			Y = a.Y + b.Y
		};
	
	public static Vector2Int Subtract(Vector2Int a, Vector2Int b)
		=> new()
		{
			X = a.X - b.X,
			Y = a.Y - b.Y
		};
	
	public static Vector2Int Multiply(Vector2Int a, Vector2Int b)
		=> new()
		{
			X = a.X * b.X,
			Y = a.Y * b.Y
		};
	
	public static Vector2Int Divide(Vector2Int a, Vector2Int b)
		=> new()
		{
			X = a.X / b.X,
			Y = a.Y / b.Y
		};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Add(Vector2Int a, Vector2Int b, out Vector2Int result)
		=> result = Add(a, b);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Subtract(Vector2Int a, Vector2Int b, out Vector2Int result)
		=> result = Subtract(a, b);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Multiply(Vector2Int a, Vector2Int b, out Vector2Int result)
		=> result = Multiply(a, b);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Divide(Vector2Int a, Vector2Int b, out Vector2Int result)
		=> result = Divide(a, b);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2Int operator +(Vector2Int a, Vector2Int b)
		=> Add(a, b);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2Int operator -(Vector2Int a, Vector2Int b)
		=> Subtract(a, b);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2Int operator *(Vector2Int a, Vector2Int b)
		=> Multiply(a, b);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2Int operator /(Vector2Int a, Vector2Int b)
		=> Divide(a, b);
}