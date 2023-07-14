namespace Petal.Framework.Content;

public class DeferredRegister<TContent> : IDeferredRegister
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
	public void ForwardTo(IRegister register)
	{
		throw new System.NotImplementedException();
	}
}