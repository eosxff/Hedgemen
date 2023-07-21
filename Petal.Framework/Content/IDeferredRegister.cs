using System;

namespace Petal.Framework.Content;

public interface IDeferredRegister
{
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

	public bool AddKey(NamespacedString id, object content);
	public bool RemoveKey(NamespacedString id);
	public bool ReplaceKey(NamespacedString id, object content);

	public void ForwardToRegister(IRegister register);
}
