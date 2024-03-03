using Petal.Framework.Modding.New;
using Petal.Framework.Util.Logging;

namespace Hgm.Game;

public abstract class HedgemenModProxy : IPetalModProxy
{
	public ILogger Logger
	{
		get;
		private set;
	}

	public bool Awake(PetalModProxyLoadArgs args)
	{
		Logger = args.Game.Logger;

		OnAwake();
		return true;
	}

	protected virtual void OnAwake()
	{

	}
}