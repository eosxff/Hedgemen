namespace Petal.Framework.Content;

public struct ContentKey
{
	public IRegister? Register
	{
		get;
		internal set;
	}
	
	public NamespacedString ContentID
	{
		get;
		internal set;
	}

	public object? GetObject()
	{
		if (Register is null)
			return null;

		return null; // todo
	}

	public TContent? GetObject<TContent>()
	{
		object? obj = GetObject();

		if (obj is null)
			return default;

		return obj is not TContent tObj ? default : tObj;
	}
}