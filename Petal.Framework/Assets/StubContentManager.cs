using System;
using Microsoft.Xna.Framework.Content;

namespace Petal.Framework.Assets;

internal class StubContentManager : ContentManager
{
	public StubContentManager(IServiceProvider serviceProvider) : base(serviceProvider)
	{
		Dispose();
	}

	public StubContentManager(IServiceProvider serviceProvider, string rootDirectory) : base(serviceProvider, rootDirectory)
	{
		Dispose();
	}

	public override T Load<T>(string assetName)
	{
		throw new ApplicationException("Do not use PetalGame.Content!");
	}
}