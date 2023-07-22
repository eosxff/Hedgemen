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

	public TContent? Get()
	{
		object? obj = Key.Content;

		if (obj is null)
			return default;

		return obj is not TContent tObj ? default : tObj;
	}

	public bool Get([NotNullWhen(true)] out TContent content)
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

	public T? GetAs<T>() where T : TContent
	{
		var found = Get(out var item);

		if (!found)
			return default;

		if (item is T tItem)
			return tItem;

		return default;
	}

	public bool GetAs<T>([NotNullWhen(true)] out T content) where T : TContent
	{
		content = default;

		var found = Get(out var item);

		if (!found)
			return false;

		if (item is T tItem)
			content = tItem;

		return true;
	}
}
