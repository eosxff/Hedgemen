using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Petal.Framework.Util.Extensions;

namespace Petal.Framework.Util;

public sealed class Map<T> : IEnumerable<T>
{
	private readonly T[,] _array;

	public int Width
		=> _array.GetLength(0);

	public int Height
		=> _array.GetLength(1);

	public Vector2Int Dimensions
		=> new(Width, Height);

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

	public void Clear()
	{
		for (int y = 0; y < Height; ++y)
		{
			for (int x = 0; x < Width; ++x)
				_array[x, y] = default;
		}
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

	public void Iterate(Func<T, Vector2Int, bool> iterateFunc)
	{
		for (int y = 0; y < Height; ++y)
		{
			for (int x = 0; x < Width; ++x)
			{
				bool shouldContinue = iterateFunc(this[x, y], new Vector2Int(x, y));
				
				if(!shouldContinue)
					return;
			}
		}
	}

	public Map<TLocal> Select<TLocal>(Func<T, TLocal> selector)
	{
		var map = new Map<TLocal>(Width, Height);

		for (int y = 0; y < Height; ++y)
		{
			for (int x = 0; x < Width; ++x)
				map[x, y] = selector(this[x, y]);
		}

		return map;
	}

	public void PopulateSection(Rectangle section, Func<Vector2Int, T> elementCreator)
	{
		for (int y = section.Y; y < section.Y + section.Height; ++y)
		{
			for (int x = section.X; x < section.X + section.Width; ++x)
				elementCreator(new Vector2Int(x, y));
		}
	}

	public void IterateSection(Rectangle section, Action<T, Vector2Int> iterateAction)
	{
		for (int y = section.Y; y < section.Y + section.Height; ++y)
		{
			for (int x = section.X; x < section.X + section.Width; ++x)
				iterateAction(this[x, y], new Vector2Int(x, y));
		}
	}

	public Map<TLocal> SelectSection<TLocal>(Rectangle section, Func<T, TLocal> selector)
	{
		var map = new Map<TLocal>(section.Width, section.Height);

		for (int y = 0; y < section.Height; ++y)
		{
			for (int x = 0; x < section.Width; ++x)
				map[x, y] = selector(this[x + section.X, y + section.Y]);
		}

		return map;
	}

	public T[] ToArray()
	{
		var array = new T[Width * Height];

		for (int y = 0; y < Height; ++y)
		{
			for (int x = 0; x < Width; ++x)
				array[x + (y * Width)] = _array[x, y];
		}

		return array;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsInBounds(Vector2Int position)
		=> IsInBounds(position.X, position.Y);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsInBounds(int x, int y)
		=> x >= 0 && x < Width && y >= 0 && y < Height;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsInBounds(Rectangle section)
		=> IsInBounds(section.X, section.Y) && IsInBounds(section.X + section.Width, section.Y + section.Height);

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

	public IEnumerator<T> GetEnumerator()
	{
		return _array.ToEnumerable().GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
