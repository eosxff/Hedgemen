using System;
using Hgm.Components;
using Hgm.WorldGeneration;
using Microsoft.Xna.Framework;
using Petal.Framework.Content;
using Petal.Framework.EC;
using Petal.Framework.Util;
using Petal.Framework.Vendor;

namespace Hgm.Vanilla.WorldGeneration;

public sealed class OverworldTerrainLandscaper : ILandscaper
{
	public Supplier<CellComponent> DeepWater;
	public Supplier<CellComponent> ShallowWater;
	public Supplier<CellComponent> Land;
	public Supplier<CellComponent> Mountain;
	public Supplier<CellComponent> TallMountain;

	public void PerformGenerationStep(Map<MapCell> cells, CartographyOptions options)
	{
		GeneratePerlin(cells, options);
		GenerateTerrain(cells, options);
	}

	public bool ShouldPerformGenerationStep(Map<MapCell> cells, CartographyOptions options)
	{
		return true;
	}

	private void GeneratePerlin(Map<MapCell> cells, CartographyOptions options)
	{
		int width = options.MapDimensions.X;
		int height = options.MapDimensions.Y;
		var noiseGen = new FastNoiseLite(options.Seed);

		for (int y = 0; y < height; ++y)
		{
			for (int x = 0; x < width; ++x)
			{
				var noiseCoordinates = ToCylinderCoordinates(new Vector2Int(x, y), options);
				float noise = noiseGen.GetNoise(noiseCoordinates.X, noiseCoordinates.Y, noiseCoordinates.Z);

				var cell = new MapCell();

				var perlinComponent = new PerlinGeneration
				{
					Height = noise,
				};

				if (!cell.AddComponent(perlinComponent))
				{
					throw new InvalidOperationException($"Could not add {typeof(PerlinGeneration)} to cell.");
				}

				cells[x, y] = cell;
			}
		}
	}

	private void GenerateTerrain(Map<MapCell> cells, CartographyOptions options)
	{
		int width = options.MapDimensions.X;
		int height = options.MapDimensions.Y;
		var noiseGen = new FastNoiseLite(options.Seed);

		for (int y = 0; y < height; ++y)
		{
			for (int x = 0; x < width; ++x)
			{
				var deepWater = (Terrain)DeepWater();
				var shallowWater = (Terrain)ShallowWater();
				var land = (Terrain)Land();
				var mountain = (Terrain)Mountain();
				var tallMountain = (Terrain)TallMountain();

				cells[x, y].AddComponent(deepWater);
			}
		}
	}

	private static Vector3 ToCylinderCoordinates(Vector2Int position, CartographyOptions options)
	{
		float x1 = 0, x2 = 1;
		float y1 = 0, y2 = 1;
		float dx = x2 - x1;
		float dy = y2 - y1;

		float s = position.X / (float)options.MapDimensions.X;
		float t = position.Y / (float)options.MapDimensions.Y;

		float nx = (float)(x1 + Math.Cos (s * 2 * Math.PI) * dx / (2 * Math.PI));
		float ny = (float)(x1 + Math.Sin (s * 2 * Math.PI) * dx / (2 * Math.PI));
		float nz = t;

		return new Vector3(nx, ny, nz);
	}
}
