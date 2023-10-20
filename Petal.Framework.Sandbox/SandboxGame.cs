namespace Petal.Framework.Sandbox;

using Framework;

public class SandboxGame : PetalGame
{
	public static SandboxGame Instance
	{
		get;
		private set;
	}

	public SandboxGame()
	{
		Instance = this;
	}
}
