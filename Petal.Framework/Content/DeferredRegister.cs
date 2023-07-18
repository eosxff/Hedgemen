using System;

namespace Petal.Framework.Content;

public class DeferredRegister<TContent> : IDeferredRegister
{
	public event EventHandler<IDeferredRegister.ForwardedToRegisterArgs>? OnForwardedToRegister;
	
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
	public bool IsForwarded
	{
		get;
		private set;
	}
	
	public DeferredRegister(NamespacedString registryName, NamespacedString modID, Registry registry)
	{
		RegistryName = registryName;
		ModID = modID;
		Registry = registry;
	}
	
	public void ForwardTo(IRegister register)
	{
		var args = new IDeferredRegister.ForwardedToRegisterArgs
		{
			Register = register
		};

		IsForwarded = true;
		OnForwardedToRegister?.Invoke(this, args);
	}
}