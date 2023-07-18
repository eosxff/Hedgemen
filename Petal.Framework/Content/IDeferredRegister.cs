using System;

namespace Petal.Framework.Content;

public interface IDeferredRegister
{
	public class ForwardedToRegisterArgs : EventArgs
	{
		public IRegister Register
		{
			get;
			init;
		}
	}

	public event EventHandler<ForwardedToRegisterArgs>? OnForwardedToRegister;
	
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