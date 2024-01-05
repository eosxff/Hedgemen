using System;
using Hgm.Game.CellComponents;
using Hgm.Game.WorldGeneration;
using Petal.Framework.EC;
using Petal.Framework.Util;

namespace Hgm.Vanilla.WorldGeneration;

public sealed class OverworldTerrainGenerationPass : TerrainGenerationPass
{
	public required Supplier<CellComponent> DeepWater;
	public required float DeepWaterHeight;

	public required Supplier<CellComponent> ShallowWater;
	public required float ShallowWaterHeight;

	public required Supplier<CellComponent> Land;
	public required float LandHeight;

	public required Supplier<CellComponent> Mountain;
	public required float MountainHeight;

	public required Supplier<CellComponent> TallMountain;
	public required float TallMountainHeight;

	public override bool ShouldPerformGenerationStep(WorldGenerationInfo genInfo)
	{
		return true;
	}

	protected override void AddComponentsToEmptyMapCell(MapCell cell, float heightValue, WorldGenerationInfo genInfo)
	{
		var terrain = GetTerrain(heightValue);
		var perlin = new PerlinGeneration
		{
			Height = heightValue
		};

		cell.AddComponent(terrain);
		cell.AddComponent(perlin);
	}

	private Terrain GetTerrain(float height)
	{
		if (height <= DeepWaterHeight)
			return (Terrain)DeepWater();
		if (height <= ShallowWaterHeight)
			return (Terrain)ShallowWater();
		if (height <= LandHeight)
			return (Terrain)Land();
		if (height <= MountainHeight)
			return (Terrain)Mountain();
		if (height <= TallMountainHeight)
			return (Terrain)TallMountain();

		Console.WriteLine("Error!");
		throw new Exception();
	}
}
