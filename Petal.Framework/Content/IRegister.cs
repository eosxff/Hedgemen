namespace Petal.Framework.Content;

public delegate TContent ContentSupplier<out TContent>();

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

	public RegistryObject<TContent> CreateRegistryObject<TContent>(NamespacedString id);
	public void ReceiveDeferredRegister(IDeferredRegister register);
}