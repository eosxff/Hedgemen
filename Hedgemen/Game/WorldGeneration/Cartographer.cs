using System.Collections.Generic;
using Petal.Framework.EC;
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
	} = new();

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
}
