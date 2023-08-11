using Petal.Framework.Content;

namespace Petal.Framework.Persistence;

public interface IManifest
{
	public void ForwardToRegister(IRegister register);
}
