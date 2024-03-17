using System;
using System.Threading.Tasks;
using Petal.Framework.Scenery.Nodes;

namespace Hgm.Game.WorldGeneration;

public abstract class WorldDecorationGenerationPass : IGenerationPass
{
	public bool ShouldPerformGenerationStep(WorldGenerationInfo genInfo)
	{
		throw new NotImplementedException();
	}

	public void PerformGenerationStep(WorldGenerationInfo genInfo)
	{
		throw new NotImplementedException();
	}

	public Task PerformGenerationStepScenic(Canvas canvas, WorldGenerationInfo genInfo)
	{
		throw new NotImplementedException();
	}
}
