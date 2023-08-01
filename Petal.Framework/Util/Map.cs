using System;
using Microsoft.Xna.Framework;
using Petal.Framework.EC;

namespace Petal.Framework.Util;

public sealed class Map<T>
{
	private T[,] _array;

	public int Width
		=> _array.GetLength(0);

	public int Height
		=> _array.GetLength(1);

	public Map() : this(0, 0)
	{

	}

	public Map(int dimensions) : this(dimensions, dimensions)
	{

	}

	public Map(Vector2Int dimensions) : this(dimensions.X, dimensions.Y)
	{

	}

	public Map(int width, int height)
	{
		_array = new T[width, height];
	}

	public void Populate(Func<T> elementCreator)
	{
		for (int y = 0; y < Height; ++y)
		{
			for (int x = 0; x < Width; ++x)
				_array[x, y] = elementCreator.Invoke();
		}
	}

	public void Populate(Func<Vector2Int, T> elementCreator)
	{
		for (int y = 0; y < Height; ++y)
		{
			for (int x = 0; x < Width; ++x)
				_array[x, y] = elementCreator.Invoke(new Vector2Int(x, y));
		}
	}

	public void Iterate(Action<T, Vector2Int> iterateAction)
	{
		for (int y = 0; y < Height; ++y)
		{
			for (int x = 0; x < Width; ++x)
				iterateAction(this[x, y], new Vector2Int(x, y));
		}
	}

	public T this[int x, int y]
	{
		get => _array[x, y];
		set => _array[x, y] = value;
	}

	public T this[Vector2Int position]
	{
		get => _array[position.X, position.Y];
		set => _array[position.X, position.Y] = value;
	}
}
