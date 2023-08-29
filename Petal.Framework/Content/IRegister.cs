using System.Diagnostics.CodeAnalysis;

namespace Petal.Framework.Content;

public delegate T Supplier<out T>();

public interface IRegister
{
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

	public bool AddKey(NamespacedString id, object content);
	public bool GetKey(NamespacedString id, out ContentKey key);
	public bool RemoveKey(NamespacedString id);
	public bool ReplaceKey(NamespacedString id, object content);
	public bool KeyExists(NamespacedString id);

	public RegistryObject<TContent> MakeReference<TContent>(NamespacedString id);
}
