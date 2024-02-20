using System;
using System.Collections.Generic;
using Hgm.Game.WorldGeneration;
using Microsoft.Xna.Framework;
using Optional.Unsafe;
using Petal.Framework.EC;
using Petal.Framework.Util;

namespace Hgm.Vanilla.WorldGeneration;

public sealed class OverworldBiomeGenerationPass : BiomeGenerationPass
{
	public required IReadOnlyList<BiomeDetails> Biomes
	{
		get;
		init;
	}

	public required BiomeDetails DefaultBiome
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

	protected override void AddComponentsToMapCell(
		MapCell cell,
		float temperature,
		float precipitation,
		TerrainType terrainType,
		WorldGenerationInfo genInfo)
	{
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
			temperatureMap[position] = (temperature * 40.0f) - 10.0f;
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
			if(InRange(temperature, biome.TemperatureRange) &&
				InRange(precipitation, biome.PrecipitationRange) &&
				terrainType == biome.RequiredTerrainType)
				return biome;
		}

		if(DefaultBiome.RequiredTerrainType == terrainType)
			return DefaultBiome;

		return null;
	}
}