﻿using System;
using System.Collections.Generic;
using Petal.Framework.Util;

namespace Petal.Framework.Content;

public class DeferredRegister<TContent> : IDeferredRegister
{
	private readonly Dictionary<NamespacedString, TContent> _content = new();

	public event EventHandler? OnForwarded;

	public Registry Registry
	{
		get;
	}

	public NamespacedString RegistryName
	{
		get;
	}

	public NamespacedString ModID
	{
		get;
	}

	public DeferredRegister(NamespacedString registryName, NamespacedString modID, Registry registry)
	{
		RegistryName = registryName;
		ModID = modID;
		Registry = registry;

		if(registry.GetRegister(registryName, out var register))
			ForwardToRegister(register);
		else
			Registry.OnRegisterAdded += RegistryOnRegisterAdded;
	}

	public bool AddKey(NamespacedString id, TContent content)
	{
		lock (_content)
		{
			if (id == NamespacedString.Default)
				return false;

			if (_content.ContainsKey(id))
				return false;

			_content.Add(id, content);
			return true;
		}
	}

	public bool AddKey(NamespacedString id, object content)
	{
		if (content is not TContent tContent)
			return false;

		return AddKey(id, tContent);
	}

	public bool RemoveKey(NamespacedString id)
	{
		lock (_content)
		{
			bool removed = _content.Remove(id);
			return removed;
		}
	}

	public bool ReplaceKey(NamespacedString id, TContent content)
	{
		lock (_content)
		{
			bool replaced = _content.ChangeValue(id, content);
			return replaced;
		}
	}

	public bool ReplaceKey(NamespacedString id, object content)
	{
		if (content is not TContent tContent)
			return false;

		return ReplaceKey(id, tContent);
	}

	public void ForwardToRegister(IRegister register)
	{
		lock (_content)
		{
			foreach (var kvp in _content)
			{
				register.AddKey(kvp.Key, kvp.Value);
			}
		}

		OnForwarded?.Invoke(this, EventArgs.Empty);
	}

	private void RegistryOnRegisterAdded(object sender, Registry.RegisterAdded args)
	{
		if (args.Register.RegistryName != RegistryName)
			return;

		ForwardToRegister(args.Register);
		Registry.OnRegisterAdded -= RegistryOnRegisterAdded;
	}
}