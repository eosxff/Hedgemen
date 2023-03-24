using Petal.Framework;
using Petal.Framework.Windowing;

namespace Hgm;

public class Hedgemen : PetalGame
{
	public static Hedgemen Instance
	{
		get;
		private set;
	}

	public Hedgemen()
	{
		Instance = this;
	}
	
	protected override GameSettings GetInitialGameSettings()
	{
		return new GameSettings
		{
			PreferredFramerate = 60,
			Vsync = false,
			WindowWidth = 1200,
			WindowHeight = 700,
			WindowMode = WindowMode.Windowed,
			IsMouseVisible = true
		};
	}
}