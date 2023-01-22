using System;
using Microsoft.Xna.Framework.Content;

namespace Petal.Assets;

internal class StubContentManager : ContentManager
{
	public StubContentManager(IServiceProvider serviceProvider) : base(serviceProvider)
	{
	}

	public StubContentManager(IServiceProvider serviceProvider, string rootDirectory) : base(serviceProvider, rootDirectory)
	{
	}

	public override T Load<T>(string assetName)
	{
		throw new ApplicationException("Do not use PetalGame.Content!");
	}
}