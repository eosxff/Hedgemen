using System.Collections.Generic;
using Hgm.Game.Scenes;
using Petal.Framework.Content;
using Petal.Framework.Util;

namespace Hgm.Game.WorldGeneration;

using LandscaperSupplierRO = RegistryObject<Supplier<ILandscaper>>;

public sealed class Cartographer
{
	public List<LandscaperSupplierRO> Landscapers
	{
		get;
	} = new();

	public WorldMap Generate(CartographyOptions options)
	{
		var cells = new Map<MapCell>(options.MapDimensions);
		cells.Populate(() => new MapCell());

		foreach (var landscaperRO in Landscapers)
		{
			if (!landscaperRO.IsPresent)
				continue;

			var landscaper = landscaperRO.Supply<ILandscaper>();

			if (!landscaper.ShouldPerformGenerationStep(cells, options))
				continue;

			landscaper.PerformGenerationStep(cells, options);
		}

		return new WorldMap(cells);
	}

	public WorldMap GenerateScenic(CampaignGenerationScene scene, CartographyOptions options)
	{
		return null; // todo stub
	}
}
