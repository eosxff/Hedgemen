using Hgm.Game;

namespace Example;

public class ExampleModProxy : HedgemenModProxy
{
	protected override void OnAwake()
	{
		Logger.Critical($"Hello from {typeof(ExampleModProxy)}!");
	}
}