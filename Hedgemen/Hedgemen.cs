using Petal;
using Petal.Windowing;

namespace Hgm;

public class Hedgemen : PetalGame
{
	public Hedgemen()
	{
		
	}

	protected override GameSettings GetInitialGameSettings()
	{
		return new GameSettings
		{
			PreferredFramerate = 60,
			Vsync = false,
			WindowWidth = 1200,
			WindowHeight = 800,
			WindowMode = WindowMode.Windowed
		};
	}
}