using Hgm;
using Petal.Framework.Modding;

namespace Example;

public class ExampleMod : PetalMod
{
	public static Hedgemen Game
		=> Hedgemen.Instance;

	protected override void OnLoadedToPetalModLoader()
	{
		Game.Logger.Debug($"Loaded {nameof(ExampleMod)}");
	}
}