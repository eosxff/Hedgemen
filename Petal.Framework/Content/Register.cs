using System.Collections.Generic;

namespace Petal.Framework.Content;

public sealed class Register<TContent> : IRegister
{
	private Dictionary<NamespacedString, ContentKey> _content = new();

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
	public RegistryObject<TContentLocal> CreateRegistryObject<TContentLocal>(NamespacedString id)
	{
		return null; // todo
	}

	public void ReceiveDeferredRegister(IDeferredRegister register)
	{
		// todo
	}

	public Register(NamespacedString registryName, NamespacedString modID, Registry registry)
	{
		RegistryName = registryName;
		ModID = modID;
		Registry = registry;
	}
}