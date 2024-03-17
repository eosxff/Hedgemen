using System;
using System.Collections.Generic;
using Hgm.Game.WorldGeneration;
using Microsoft.Xna.Framework;
using Optional.Unsafe;
using Petal.Framework.EC;
using Petal.Framework.Util;
using Petal.Framework.Vendor;

namespace Hgm.Vanilla.WorldGeneration;

public sealed class OverworldBiomeGenerationPass : BiomeGenerationPass
{
	public required IReadOnlyList<BiomeDetails> Biomes
	{
		get;
		init;
	}

	public BiomeDetails? DefaultBiome
	{
		get;
		init;
	}

	public override bool ShouldPerformGenerationStep(WorldGenerationInfo genInfo)
	{
		bool shouldGenerate = true;

		genInfo.Cells.Iterate((mapCell, position) =>
		{
			if(!mapCell.HasComponentOf<Terrain>())
			{
				Hedgemen.Instance.ValueOrDefault()?.Logger.Error(@$"
					Biomes can not generate if there are cells without terrain components! {position}");
				shouldGenerate = false;
				return false;
			}

			return true;
		});

		return shouldGenerate;
	}

	protected override void PrepareTemperaturePrecipitationNoiseGen(WorldGenerationInfo genInfo)
	{
		 // fixme dont use harcoded values for frequency
		var prng = new Random(genInfo.NoiseGenArgs.Seed);
		int temperatureSeed = prng.Next(int.MinValue, int.MaxValue);
		int precipitationSeed = prng.Next(int.MinValue, int.MaxValue);

		TemperatureNoiseGen.SetSeed(temperatureSeed);
		TemperatureNoiseGen.SetFractalType(FastNoiseLite.FractalType.FBm);
		TemperatureNoiseGen.SetFractalLacunarity(genInfo.NoiseGenArgs.Lacunarity);
		TemperatureNoiseGen.SetFractalOctaves(genInfo.NoiseGenArgs.Octaves);
		TemperatureNoiseGen.SetFrequency(genInfo.NoiseGenArgs.Frequency / 3.0f);

		PrecipitationNoiseGen.SetSeed(precipitationSeed);
		PrecipitationNoiseGen.SetFractalType(FastNoiseLite.FractalType.FBm);
		PrecipitationNoiseGen.SetFractalLacunarity(genInfo.NoiseGenArgs.Lacunarity);
		PrecipitationNoiseGen.SetFractalOctaves(genInfo.NoiseGenArgs.Octaves);
		PrecipitationNoiseGen.SetFrequency(genInfo.NoiseGenArgs.Frequency / 3.0f);
	}

	protected override void AddComponentsToMapCell(
		MapCell cell,
		float temperature,
		float precipitation,
		TerrainType terrainType,
		WorldGenerationInfo genInfo)
	{
		if(cell.HasComponentOf<Biome>())
			return;

		var selectedBiome = SelectBiomeDetails(temperature, precipitation, terrainType);

		if(selectedBiome is null)
			return;

		var biome = selectedBiome.BiomeSupplier();
		cell.AddComponent(biome);
	}

	protected override void OnPrecipitationMapNormalized(Map<float> precipitationMap, WorldGenerationInfo genInfo)
	{
		// todo just using a whittaker diagram as reference, maybe don't hardcode
		precipitationMap.Iterate((precipitation, position) =>
		{
			precipitationMap[position] = precipitation * 455.0f;
		});
	}

	protected override void OnTemperatureMapNormalized(Map<float> temperatureMap, WorldGenerationInfo genInfo)
	{
		// todo just using a whittaker diagram as reference, maybe don't hardcode
		temperatureMap.Iterate((temperature, position) =>
		{
			temperatureMap[position] = (temperature * 45.0f) - 15.0f;
		});
	}

	private static bool InRange(float value, Vector2 constraints)
	{
		return value >= constraints.X && value <= constraints.Y;
	}

	private BiomeDetails? SelectBiomeDetails(float temperature, float precipitation, TerrainType terrainType)
	{
		foreach(var biome in Biomes)
		{
			bool temperatureInRange = InRange(temperature, biome.TemperatureRange);
			bool precipitationInRange = InRange(precipitation, biome.PrecipitationRange);
			bool correctTerrainType = terrainType == biome.RequiredTerrainType;

			if(temperatureInRange && precipitationInRange && correctTerrainType)
				return biome;

			// it's not exactly elegant but since the whittaker diagram has a very clear diagonal bias for
			// precipitation and temperature, if we invert the values that aren't in range of any biome then
			// we should be able to find one
			bool inverseTemperatureInRange = InRange(30.0f - temperature, biome.TemperatureRange);
			bool inversePrecipitationInRange = InRange(455.0f - precipitation, biome.PrecipitationRange);

			if(inverseTemperatureInRange && inversePrecipitationInRange && correctTerrainType)
				return biome;
		}

		return (DefaultBiome is not null && DefaultBiome.RequiredTerrainType == terrainType)
			? DefaultBiome : null;
	}
}