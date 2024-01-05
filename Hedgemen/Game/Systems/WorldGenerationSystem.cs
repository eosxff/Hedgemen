using Hgm.Game.WorldGeneration;

namespace Hgm.Game.Systems;

public static class WorldGenerationSystem
{
	public static WorldMap GenerateWorldMap(Cartographer cartographer)
	{
		return cartographer.Generate();
	}
}
