using System;
using Hgm.Game.CellComponents;
using Hgm.Game.WorldGeneration;
using Microsoft.Xna.Framework;
using Petal.Framework;
using Petal.Framework.EC;
using Petal.Framework.Util;
using Petal.Framework.Vendor;

namespace Hgm.Vanilla.WorldGeneration;

public sealed class OverworldTerrainLandscaper : ILandscaper
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

	public required float Scale; // currently does nothing
	public required int Octaves;
	public required float Frequency;
	public required float Lacunarity;
	public required Vector2Int Offset;
	public required float FalloffModifier;

	public void PerformGenerationStep(Map<MapCell> cells, CartographyOptions options)
	{
		var noiseMap = GenerateNoiseMap(options);
		NormalizeNoiseMap(noiseMap, options);

		// a falloffmodifier of 0 would be pointless. a falloffmodifier of <0 would be fucking shitfucked stupid
		if(FalloffModifier > 0.0f)
		{
			var falloffMap = GenerateFalloffMap(options);
			ApplyFalloffMap(noiseMap, falloffMap, options);
			//NormalizeNoiseMap(noiseMap, options);
		}

		cells.Populate(() => new MapCell());
		AddPerlinComponents(cells, noiseMap, options);
		AddTerrainComponents(cells, noiseMap, options);
	}

	private void AddPerlinComponents(Map<MapCell> cells, Map<float> noiseMap, CartographyOptions options)
	{
		int width = options.MapDimensions.X;
		int height = options.MapDimensions.Y;

		for (int y = 0; y < height; ++y)
		{
			for (int x = 0; x < width; ++x)
			{
				var perlin = new PerlinGeneration
				{
					Height = noiseMap[x, y]
				};

				if (!cells[x, y].AddComponent(perlin))
				{
					Console.WriteLine("Error!");
					throw new Exception();
				}
			}
		}
	}

	private void AddTerrainComponents(Map<MapCell> cells, Map<float> noiseMap, CartographyOptions options)
	{
		int width = options.MapDimensions.X;
		int height = options.MapDimensions.Y;

		for (int y = 0; y < height; ++y)
		{
			for (int x = 0; x < width; ++x)
			{
				var terrain = GetTerrain(noiseMap[x, y]);

				if (!cells[x, y].AddComponent(terrain))
				{
					Console.WriteLine("Error!");
					throw new Exception();
				}
			}
		}
	}

	private Map<float> GenerateNoiseMap(CartographyOptions options)
	{
		int width = options.MapDimensions.X;
		int height = options.MapDimensions.Y;

		var noiseMap = new Map<float>(width, height);

		var noiseGen = new FastNoiseLite(options.Seed);
		noiseGen.SetFractalType(FastNoiseLite.FractalType.FBm);
		noiseGen.SetFractalLacunarity(Lacunarity);
		noiseGen.SetFractalOctaves(Octaves);
		noiseGen.SetFrequency(Frequency);

		for (int y = 0; y < height; ++y)
		{
			for (int x = 0; x < width; ++x)
			{
				noiseMap[x, y] = GetNoise(x, y, noiseGen, options);
			}
		}

		return noiseMap;
	}

	private void NormalizeNoiseMap(Map<float> noiseMap, CartographyOptions options)
	{
		int width = options.MapDimensions.X;
		int height = options.MapDimensions.Y;

		float minHeight = float.MaxValue;
		float maxHeight = float.MinValue;

		for (int y = 0; y < height; ++y)
		{
			for (int x = 0; x < width; ++x)
			{
				float noise = noiseMap[x, y];

				if (noise > maxHeight)
					maxHeight = noise;
				else if (noise < minHeight)
					minHeight = noise;
			}
		}

		for (int y = 0; y < height; ++y)
		{
			for (int x = 0; x < width; ++x)
			{
				float noise = noiseMap[x, y];
				noiseMap[x, y] = 1.0f - Mathf.Lerp(noise, minHeight, maxHeight);
			}
		}
	}

	private Map<float> GenerateFalloffMap(CartographyOptions options)
	{
		int width = options.MapDimensions.X;
		int height = options.MapDimensions.Y;

		var falloffMap = new Map<float>(width, height);

		for (int y = 0; y < height; ++y)
		{
			for (int x = 0; x < width; ++x)
			{
				float xx = x / (float)width * 2 - 1;
				float yy = y / (float)height * 2 - 1;

				float value = Math.Max(Math.Abs(xx), Math.Abs(yy));

				falloffMap[x, y] = value * FalloffModifier;
			}
		}

		return falloffMap;
	}

	private void ApplyFalloffMap(Map<float> noiseMap, Map<float> falloffMap, CartographyOptions options)
	{
		int width = options.MapDimensions.X;
		int height = options.MapDimensions.Y;

		for (int y = 0; y < height; ++y)
		{
			for (int x = 0; x < width; ++x)
			{
				noiseMap[x, y] -= falloffMap[x, y];
			}
		}
	}

	private float GetNoise(int x, int y, FastNoiseLite noiseGen, CartographyOptions options)
	{
		float noise = 0.0f;

		for (int i = 0; i < Octaves; ++i)
		{
			var sample = ToCylinderCoordinates(new Vector2(x + Offset.X, y + Offset.Y), options);
			noise += noiseGen.GetNoise(sample.X, sample.Y, sample.Z) * Scale; // scale gets normalized anyways
		}

		return noise;
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

	public bool ShouldPerformGenerationStep(Map<MapCell> cells, CartographyOptions options)
	{
		return true;
	}

	private static Vector3 ToCylinderCoordinates(Vector2 position, CartographyOptions options)
	{
		float x1 = 0, x2 = 1;
		float y1 = 0, y2 = 1;
		float dx = x2 - x1;
		float dy = y2 - y1;

		float s = position.X / options.MapDimensions.X;
		float t = position.Y / options.MapDimensions.Y;

		float nx = (float)(x1 + Math.Cos (s * 2 * Math.PI) * dx / (2 * Math.PI));
		float ny = (float)(x1 + Math.Sin (s * 2 * Math.PI) * dx / (2 * Math.PI));
		float nz = t;

		return new Vector3(nx, ny, nz);
	}
}
