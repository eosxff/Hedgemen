using Petal.Framework.EC;
using Petal.Framework.Util;

namespace Hgm.WorldGeneration;

public interface ILandscaper
{
	public void PerformGenerationStep(Map<MapCell> cells, CartographyOptions options);
	public bool ShouldPerformGenerationStep(Map<MapCell> cells, CartographyOptions options);
}