using Petal.Framework.Content;

namespace Petal.Framework.Persistence;

public interface IBankManifest
{
	public void ForwardToRegister(IRegister register);
}
