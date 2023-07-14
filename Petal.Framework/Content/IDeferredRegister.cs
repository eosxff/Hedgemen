namespace Petal.Framework.Content;

public interface IDeferredRegister
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

	public bool IsForwarded
	{
		get;
	}
	
	public void ForwardTo(IRegister register);
}