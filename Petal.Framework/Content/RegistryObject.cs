using System;
using System.Diagnostics.CodeAnalysis;

namespace Petal.Framework.Content;

public sealed class RegistryObject<TContent>
{
	public static RegistryObject<TContent> Empty
		=> new(new ContentKey(NamespacedString.Default, null, null));

	public ContentKey Key
	{
		get;
	}

	public bool HasValidKey
		=> Key.Content is not null && Key.ContentID != NamespacedString.Default && Key.Register != null;

	public RegistryObject(ContentKey key)
	{
		Key = key;
	}

	public TSuppliedContent? Supply<TSuppliedContent>()
	{
		bool success = Supply(out TSuppliedContent content);
		return content;
	}

	public bool Supply<TSuppliedContent>([NotNullWhen(true)] out TSuppliedContent? suppliedContent)
	{
		suppliedContent = default;

		bool success = GetAs(out Supplier<TSuppliedContent> content);

		if (!success)
			return false;

		suppliedContent = content();
		return true;
	}

	public TContent? Get()
	{
		bool success = Get(out var content);
		return content;
	}

	public bool Get([NotNullWhen(true)] out TContent? content)
	{
		object? obj = Key.Content;

		if (obj is not TContent tObj)
		{
			content = default;
			return false;
		}

		content = tObj;
		return true;
	}

	public T? GetAs<T>()
	{
		bool found = GetAs(out T item);
		return item;
	}

	public bool GetAs<T>([NotNullWhen(true)] out T? content)
	{
		content = default;

		bool found = Get(out var item);

		if (!found)
			return false;

		if (item is not T tItem)
			return false;

		content = tItem;
		return true;
	}

	public RegistryObject<TBaseContent> ReferenceAs<TBaseContent>()
	{
		return new RegistryObject<TBaseContent>(Key);
	}
}
