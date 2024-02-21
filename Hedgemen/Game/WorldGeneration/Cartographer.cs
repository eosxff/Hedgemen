using System.Collections.Generic;
using System.Threading.Tasks;
using Hgm.Vanilla.WorldGeneration;
using Microsoft.Xna.Framework;
using Petal.Framework.EC;
using Petal.Framework.Scenery.Nodes;
using Petal.Framework.Util;

namespace Hgm.Game.WorldGeneration;

public enum CartographyStage
{
	None,
	Terrain,
	Biome,
	Simulation
}

public sealed class Cartographer
{
	public Cartographer()
	{

	}

	public CartographyStage Stage
	{
		get;
		set;
	} = CartographyStage.None;

	public List<Supplier<IGenerationPass>> GenerationPasses
	{
		get;
		private set;
	} = [];

	public NoiseArgs NoiseGenerationArgs
	{
		get;
		set;
	} = NoiseArgs.New();

	public WorldMap Generate()
	{
		var cells = new Map<MapCell>(NoiseGenerationArgs.Dimensions.X, NoiseGenerationArgs.Dimensions.Y);
		cells.Populate(() => new MapCell());

		var genInfo = new WorldGenerationInfo(NoiseGenerationArgs)
		{
			Cells = cells
		};

		foreach (var generationPassSupplier in GenerationPasses)
		{
			var generationPass = generationPassSupplier();

			if (!generationPass.ShouldPerformGenerationStep(genInfo))
				continue;

			generationPass.PerformGenerationStep(genInfo);
		}

		var worldMap = new WorldMap(genInfo.Cells);
		return worldMap;
	}

	public async Task<WorldMap> GenerateScenic(Canvas canvas)
	{
		var cells = new Map<MapCell>(NoiseGenerationArgs.Dimensions.X, NoiseGenerationArgs.Dimensions.Y);
		cells.Populate(() => new MapCell());

		var genInfo = new WorldGenerationInfo(NoiseGenerationArgs)
		{
			Cells = cells
		};

		foreach (var generationPassSupplier in GenerationPasses)
		{
			var generationPass = generationPassSupplier();

			if (!generationPass.ShouldPerformGenerationStep(genInfo))
				continue;

			await generationPass.PerformGenerationStepScenic(canvas, genInfo);
		}

		var worldMap = new WorldMap(genInfo.Cells);
		return worldMap;
	}

	public static Map<Color> QueryCurrentMapGenerationProgress(Map<MapCell> cells)
	{
		var mapPixelColorQuery = new MapPixelColorQuery();
		var colorMap = new Map<Color>(cells.Width, cells.Height);

		for (int y = 0; y < colorMap.Height; ++y)
		{
			for (int x = 0; x < colorMap.Width; ++x)
			{
				var cell = cells[x, y];
				mapPixelColorQuery.Reset();

				cell.PropagateEventIfResponsive(mapPixelColorQuery);
				colorMap[x, y] = mapPixelColorQuery.MapPixelColor;
			}
		}

		return colorMap;
	}
}
