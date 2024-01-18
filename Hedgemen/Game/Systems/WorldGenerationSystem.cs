using System.Threading.Tasks;
using Hgm.Game.WorldGeneration;
using Petal.Framework.EC;
using Petal.Framework.Scenery;
using Petal.Framework.Scenery.Nodes;
using Petal.Framework.Util;

namespace Hgm.Game.Systems;

public static class WorldGenerationSystem
{
	public static WorldMap GenerateWorldMap(Cartographer cartographer)
	{
		return cartographer.Generate();
	}

	public async static Task<WorldMap> GenerateWorldMapScenic(Cartographer cartographer, Canvas canvas)
	{
		return await cartographer.GenerateScenic(canvas);
	}
}
