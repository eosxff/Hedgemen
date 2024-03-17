using System.Threading.Tasks;
using Hgm.Game.CellComponents;
using Microsoft.Xna.Framework;
using Petal.Framework;
using Petal.Framework.EC;
using Petal.Framework.Scenery.Nodes;
using Petal.Framework.Util;
using Petal.Framework.Vendor;

namespace Hgm.Game.WorldGeneration;

public abstract class BiomeGenerationPass : IGenerationPass
{
	protected FastNoiseLite PrecipitationNoiseGen
	{
		get;
		private set;
	} = new();

	protected FastNoiseLite TemperatureNoiseGen
	{
		get;
		private set;
	} = new();

	public abstract bool ShouldPerformGenerationStep(WorldGenerationInfo genInfo);

	public void PerformGenerationStep(WorldGenerationInfo genInfo)
	{
		PrepareTemperaturePrecipitationNoiseGen(genInfo);
		OnPreTemperaturePrecipitationMapsGenerated(genInfo);

		var temperatureMap = GenerateTemperatureMap(genInfo);
		var precipitationMap = GeneratePrecipitationMap(genInfo);

		IGenerationPass.NormalizeNoiseMap(temperatureMap, genInfo);
		IGenerationPass.NormalizeNoiseMap(precipitationMap, genInfo);

		OnPostTemperaturePrecipitationMapsGenerated(temperatureMap, precipitationMap, genInfo);

		OnTemperatureMapNormalized(temperatureMap, genInfo);
		OnPrecipitationMapNormalized(precipitationMap, genInfo);

		AddComponentsToMapCells(temperatureMap, precipitationMap, genInfo);
		OnComponentsAddedToMapCells(temperatureMap, precipitationMap, genInfo);
	}

	protected virtual void OnComponentsAddedToMapCells(
		Map<float> temperatureMap,
		Map<float> precipitationMap,
		WorldGenerationInfo genInfo)
	{

	}

	public async Task PerformGenerationStepScenic(Canvas canvas, WorldGenerationInfo genInfo)
	{
		await Task.Run(() => PerformGenerationStep(genInfo)); // todo

		Hedgemen.InstanceOrThrow.Logger.Debug($"Updating canvas.");
		var colorMap = Cartographer.QueryCurrentMapGenerationProgress(genInfo.Cells);
		canvas.ColorMap = colorMap;
		canvas.ApplyColorMap();
	}

	protected virtual void OnPreTemperaturePrecipitationMapsGenerated(WorldGenerationInfo genInfo)
	{

	}

	protected virtual void OnPostTemperaturePrecipitationMapsGenerated(
		Map<float> temperatureMap,
		Map<float> precipitationMap,
		WorldGenerationInfo genInfo)
	{

	}

	private Map<float> GeneratePrecipitationMap(WorldGenerationInfo genInfo)
	{
		var map = new Map<float>(genInfo.NoiseGenArgs.Dimensions);

		for (int y = 0; y < map.Height; ++y)
		{
			for (int x = 0; x < map.Width; ++x)
				map[x, y] = SamplePrecipitationAtCoordinate(x, y, genInfo);
		}

		return map;
	}

	protected virtual float SamplePrecipitationAtCoordinate(int x, int y, WorldGenerationInfo genInfo)
	{
		var dimensions = genInfo.NoiseGenArgs.Dimensions;
		var offset = genInfo.NoiseGenArgs.Offset;
		var sample = Mathf.Torus(
			new Vector2(x + offset.X, y + offset.Y),
			dimensions.X,
			dimensions.Y);

		return PrecipitationNoiseGen.GetNoise(sample.X, sample.Y, sample.Z);
	}

	private Map<float> GenerateTemperatureMap(WorldGenerationInfo genInfo)
	{
		var map = new Map<float>(genInfo.NoiseGenArgs.Dimensions);

		for (int y = 0; y < map.Height; ++y)
		{
			for (int x = 0; x < map.Width; ++x)
				map[x, y] = SampleTemperatureAtCoordinate(x, y, genInfo);
		}

		return map;
	}

	protected virtual float SampleTemperatureAtCoordinate(int x, int y, WorldGenerationInfo genInfo)
	{
		var dimensions = genInfo.NoiseGenArgs.Dimensions;
		var offset = genInfo.NoiseGenArgs.Offset;
		var sample = Mathf.Torus(
			new Vector2(x + offset.X, y + offset.Y),
			dimensions.X,
			dimensions.Y);

		return TemperatureNoiseGen.GetNoise(sample.X, sample.Y, sample.Z);
	}

	protected virtual void PrepareTemperaturePrecipitationNoiseGen(WorldGenerationInfo genInfo)
	{
		// fixme using noisegenargs.seed is bad here

		TemperatureNoiseGen.SetSeed(genInfo.NoiseGenArgs.Seed);
		TemperatureNoiseGen.SetFractalType(FastNoiseLite.FractalType.FBm);
		TemperatureNoiseGen.SetFractalLacunarity(genInfo.NoiseGenArgs.Lacunarity);
		TemperatureNoiseGen.SetFractalOctaves(genInfo.NoiseGenArgs.Octaves);
		TemperatureNoiseGen.SetFrequency(genInfo.NoiseGenArgs.Frequency);

		PrecipitationNoiseGen.SetSeed(genInfo.NoiseGenArgs.Seed);
		PrecipitationNoiseGen.SetFractalType(FastNoiseLite.FractalType.FBm);
		PrecipitationNoiseGen.SetFractalLacunarity(genInfo.NoiseGenArgs.Lacunarity);
		PrecipitationNoiseGen.SetFractalOctaves(genInfo.NoiseGenArgs.Octaves);
		PrecipitationNoiseGen.SetFrequency(genInfo.NoiseGenArgs.Frequency);
	}

	protected virtual void AddComponentsToMapCells(
		Map<float> temperatureMap,
		Map<float> precipitationMap,
		WorldGenerationInfo genInfo)
	{
		PetalExceptions.ThrowIfNull(genInfo.Cells);

		int width = temperatureMap.Width;
		int height = temperatureMap.Height;

		var worldCellInfoQuery = new WorldCellInfoQuery();

		for (int y = 0; y < height; ++y)
		{
			for (int x = 0; x < width; ++x)
			{
				worldCellInfoQuery.Reset();
				var cell = genInfo.Cells[x, y];
				float temperature = temperatureMap[x, y];
				float precipitation = precipitationMap[x, y];
				var terrainType = TerrainType.None;

				if(cell.WillRespondToEvent<WorldCellInfoQuery>())
				{
					cell.PropagateEvent(worldCellInfoQuery);
					terrainType = worldCellInfoQuery.TerrainType;
				}

				if(terrainType != TerrainType.None)
					AddComponentsToMapCell(cell, temperature, precipitation, terrainType, genInfo);
			}
		}
	}

	protected abstract void AddComponentsToMapCell(
		MapCell cell,
		float temperature,
		float precipitation,
		TerrainType terrainType,
		WorldGenerationInfo genInfo);

	protected abstract void OnTemperatureMapNormalized(Map<float> temperatureMap, WorldGenerationInfo genInfo);
	protected abstract void OnPrecipitationMapNormalized(Map<float> precipitationMap, WorldGenerationInfo genInfo);
}
