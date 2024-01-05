using System;
using System.Diagnostics.CodeAnalysis;
using Petal.Framework.Util;

namespace Petal.Framework.Content;

public sealed class RegistryObject<TContent>
{
	public static RegistryObject<TContent> Empty
		=> new(new ContentKey(NamespacedString.Default, null, null));

	private TContent? _content;
	private NamespacedString _location;

	public NamespacedString Location
		=> _location;

	/// <summary>
	/// Returns true if this <see cref="RegistryObject{TContent}"/> points to valid content, otherwise false.
	/// </summary>
	public bool IsPresent
		=> _content is not null && Location != NamespacedString.Default;

	public RegistryObject(ContentKey key)
	{
		_location = key.Location;
		_content = (TContent)key.Content;
	}

	public TSuppliedContent Supply<TSuppliedContent>()
	{
		PetalExceptions.ThrowIfNull(_content);

		if (_content is not Supplier<TSuppliedContent> supplier)
		{
			throw new InvalidCastException(
				$"Can not cast {_content.GetType()} to type {typeof(Supplier<TSuppliedContent>)}.");
		}

		return supplier();
	}

	public bool Supply<TSuppliedContent>([NotNullWhen(true)] out TSuppliedContent? suppliedContent)
	{
		suppliedContent = default;

		if (_content is null)
			return false;

		if (_content is not Supplier<TSuppliedContent> supplier)
			return false;

		suppliedContent = supplier();
		return true;
	}

	public TContent Get()
	{
		PetalExceptions.ThrowIfNull(_content);
		return _content;
	}

	public bool Get(out TContent? content)
	{
		content = _content;
		return content is not null;
	}

	public T? GetAs<T>()
	{
		PetalExceptions.ThrowIfNull(_content);
		return _content is not T tContent ? default : tContent;
	}

	public bool GetAs<T>([NotNullWhen(true)] out T? content)
	{
		content = default;

		if (_content is null)
			return false;

		if (_content is not T tContent)
		{
			content = default;
			return false;
		}

		content = tContent;
		return true;
	}
}
