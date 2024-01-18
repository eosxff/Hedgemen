using System.Threading.Tasks;
using Petal.Framework.Scenery;
using Petal.Framework.Scenery.Nodes;

namespace Hgm.Game.WorldGeneration;

public interface IGenerationPass
{
	public bool ShouldPerformGenerationStep(WorldGenerationInfo genInfo);
	public void PerformGenerationStep(WorldGenerationInfo genInfo);
	public Task PerformGenerationStepScenic(Canvas canvas, WorldGenerationInfo genInfo);
}
