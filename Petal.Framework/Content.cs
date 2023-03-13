using System;
using Petal.Framework.Util;

namespace Petal.Framework;

public sealed class Content<TContent>
{
	public NamespacedString ReferenceName
	{
		get;
	}

	public TContent? Item
	{
		get;
	}

	public Content(NamespacedString referenceName, TContent item)
	{
		ReferenceName = referenceName;
		Item = item;
	}

	public Content(NamespacedString referenceName)
	{
		ReferenceName = referenceName;
		Item = default; // todo be able to retrieve an item from some content bank mechanism
	}

	public T? To<T>()
	{
		if (Item is not T item)
			throw new InvalidCastException($"Can not cast {typeof(T)} to {typeof(TContent)}.");

		return item;
	}
}