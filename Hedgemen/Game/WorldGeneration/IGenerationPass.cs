namespace Hgm.Game.WorldGeneration;

public interface IGenerationPass
{
	public bool ShouldPerformGenerationStep(WorldGenerationInfo genInfo);
	public void PerformGenerationStep(WorldGenerationInfo genInfo);
}
