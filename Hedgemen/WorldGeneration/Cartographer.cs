using System.Collections.Generic;
using Petal.Framework.Content;
using Petal.Framework.EC;
using Petal.Framework.Util;

namespace Hgm.WorldGeneration;

public sealed class Cartographer
{
	public List<RegistryObject<ContentSupplier<ILandscaper>>> Landscapers
	{
		get;
	} = new();

	public Cartographer()
	{

	}

	public WorldMap Generate(CartographyOptions options)
	{
		var cells = new Map<MapCell>(options.MapDimensions);
		cells.Populate(() => new MapCell());

		foreach (var landscaperRO in Landscapers)
		{
			if(!landscaperRO.Supply(out ILandscaper landscaper))
				continue;

			if(landscaper.ShouldPerformGenerationStep(cells, options))
				landscaper.PerformGenerationStep(cells, options);
		}

		return new WorldMap(cells);
	}
}
