using System.Threading.Tasks;
using Petal.Framework.Scenery;
using Petal.Framework.Scenery.Nodes;
using Petal.Framework.Vendor;

namespace Hgm.Game.WorldGeneration;

public abstract class BiomeGenerationPass : IGenerationPass
{
	protected FastNoiseLite NoiseGen
	{
		get;
		private set;
	}

	public abstract bool ShouldPerformGenerationStep(WorldGenerationInfo genInfo);

	public void PerformGenerationStep(WorldGenerationInfo genInfo)
	{
		NoiseGen = new FastNoiseLite(genInfo.NoiseGenArgs.Seed);
		PrepareNoiseGen(genInfo);
	}

	public async Task PerformGenerationStepScenic(Canvas canvas, WorldGenerationInfo genInfo)
	{

	}

	protected virtual void PrepareNoiseGen(WorldGenerationInfo genInfo)
	{
		NoiseGen.SetFractalType(FastNoiseLite.FractalType.FBm);
		NoiseGen.SetFractalLacunarity(genInfo.NoiseGenArgs.Lacunarity);
		NoiseGen.SetFractalOctaves(genInfo.NoiseGenArgs.Octaves);
		NoiseGen.SetFrequency(genInfo.NoiseGenArgs.Frequency);
	}
}
