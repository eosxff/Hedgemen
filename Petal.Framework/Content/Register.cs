﻿using System;
using System.Collections.Generic;
using Petal.Framework.Util;

namespace Petal.Framework.Content;

public sealed class Register<TContent> : IRegister
{
	private readonly Dictionary<NamespacedString, ContentKey> _content = new();

	public event EventHandler? OnKeyAdded;
	public event EventHandler? OnKeyRemoved;
	public event EventHandler? OnKeyReplaced;

	public NamespacedString RegistryName
	{
		get;
	}

	public NamespacedString ModID
	{
		get;
	}

	public Registry Registry
	{
		get;
	}

	public Register(NamespacedString registryName, NamespacedString modID, Registry registry)
	{
		RegistryName = registryName;
		ModID = modID;
		Registry = registry;
	}

	public bool AddKey(NamespacedString id, TContent content)
	{
		if (id == NamespacedString.Default)
			return false;

		if (_content.ContainsKey(id))
			return false;

		_content.Add(id, new ContentKey(id, this, content));

		OnKeyAdded?.Invoke(this, EventArgs.Empty);

		return true;
	}

	public bool GetKey(NamespacedString id, out ContentKey key)
	{
		key = default;

		if (id == NamespacedString.Default || !_content.ContainsKey(id))
			return false;

		key = _content[id];
		return true;
	}

	public bool AddKey(NamespacedString id, object content)
	{
		if (content is not TContent tContent)
			return false;

		return AddKey(id, tContent);
	}

	public bool RemoveKey(NamespacedString id)
	{
		bool removed = _content.Remove(id);

		if(removed)
			OnKeyRemoved?.Invoke(this, EventArgs.Empty);

		return removed;
	}

	public bool ReplaceKey(NamespacedString id, TContent content)
	{
		bool replaced = _content.ChangeValue(id, new ContentKey(id, this, content));

		if(replaced)
			OnKeyReplaced?.Invoke(this, EventArgs.Empty);

		return replaced;
	}

	public bool ReplaceKey(NamespacedString id, object content)
	{
		if (content is not TContent tContent)
			return false;

		return ReplaceKey(id, tContent);
	}

	public bool KeyExists(NamespacedString id)
	{
		return _content.ContainsKey(id);
	}

	public ICollection<ContentKey> RegisteredContent
		=> _content.Values;

	public RegistryObject<TContentLocal> MakeReference<TContentLocal>(NamespacedString id)
	{
		if (_content.TryGetValue(id, out var key))
		{
			return new RegistryObject<TContentLocal>(key);
		}

		return new RegistryObject<TContentLocal>(new ContentKey(id, this, null));
	}

	public RegistryObject<TContent> MakeReference(NamespacedString id)
	{
		if (_content.TryGetValue(id, out var key))
		{
			return new RegistryObject<TContent>(key);
		}

		return new RegistryObject<TContent>(new ContentKey(id, this, null));
	}

	public RegistryObject<TDerivedContent> MakeDerivedReference<TDerivedContent>(NamespacedString id)
		where TDerivedContent : TContent
	{
		if (_content.TryGetValue(id, out var key))
		{
			return new RegistryObject<TDerivedContent>(key);
		}

		return new RegistryObject<TDerivedContent>(new ContentKey(id, this, null));
	}
}
