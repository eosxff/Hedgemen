using System.Threading.Tasks;
using Petal.Framework;
using Petal.Framework.EC;
using Petal.Framework.Scenery;
using Petal.Framework.Scenery.Nodes;
using Petal.Framework.Util;

namespace Hgm.Game.WorldGeneration;

public interface IGenerationPass
{
	public bool ShouldPerformGenerationStep(WorldGenerationInfo genInfo);
	public void PerformGenerationStep(WorldGenerationInfo genInfo);
	public Task PerformGenerationStepScenic(Canvas canvas, WorldGenerationInfo genInfo);

	protected static Map<MapCell> GenerateEmptyCellMap(WorldGenerationInfo genInfo)
	{
		var dimensions = genInfo.NoiseGenArgs.Dimensions;
		var map = new Map<MapCell>(dimensions);

		map.Populate(() => new MapCell());

		return map;
	}

	public static void NormalizeNoiseMap(Map<float> noiseMap, WorldGenerationInfo genInfo)
	{
		var dimensions = genInfo.NoiseGenArgs.Dimensions;
		int width = dimensions.X;
		int height = dimensions.Y;

		float minHeight = float.MaxValue;
		float maxHeight = float.MinValue;

		for (int y = 0; y < height; ++y)
		{
			for (int x = 0; x < width; ++x)
			{
				float noise = noiseMap[x, y];

				if (noise > maxHeight)
					maxHeight = noise;
				else if (noise < minHeight)
					minHeight = noise;
			}
		}

		for (int y = 0; y < height; ++y)
		{
			for (int x = 0; x < width; ++x)
			{
				float noise = noiseMap[x, y];
				noiseMap[x, y] = 1.0f - Mathf.Lerp(noise, minHeight, maxHeight);
			}
		}
	}
}
