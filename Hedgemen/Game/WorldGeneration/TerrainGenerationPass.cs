using System;
using Microsoft.Xna.Framework;
using Petal.Framework;
using Petal.Framework.EC;
using Petal.Framework.Util;
using Petal.Framework.Vendor;

namespace Hgm.Game.WorldGeneration;

public abstract class TerrainGenerationPass : IGenerationPass
{
	protected FastNoiseLite NoiseGen
	{
		get;
		private set;
	}

	public abstract bool ShouldPerformGenerationStep(WorldGenerationInfo genInfo);

	public void PerformGenerationStep(WorldGenerationInfo genInfo)
	{
		NoiseGen = new FastNoiseLite(genInfo.NoiseGenArgs.Seed);
		PrepareNoiseGen(genInfo);

		genInfo.Cells ??= GenerateEmptyCellMap(genInfo);
		var noiseHeightMap = GenerateNoiseHeightMap(genInfo);

		NormalizeNoiseMap(noiseHeightMap, genInfo);
		ApplyFalloffValuesToMap(noiseHeightMap, genInfo);

		AddComponentsToMapCells(noiseHeightMap, genInfo);
	}

	protected virtual void PrepareNoiseGen(WorldGenerationInfo genInfo)
	{
		NoiseGen.SetFractalType(FastNoiseLite.FractalType.FBm);
		NoiseGen.SetFractalLacunarity(genInfo.NoiseGenArgs.Lacunarity);
		NoiseGen.SetFractalOctaves(genInfo.NoiseGenArgs.Octaves);
		NoiseGen.SetFrequency(genInfo.NoiseGenArgs.Frequency);
	}

	protected virtual void AddComponentsToMapCells(Map<float> noiseHeightMap, WorldGenerationInfo genInfo)
	{
		PetalExceptions.ThrowIfNull(genInfo.Cells);

		int width = noiseHeightMap.Width;
		int height = noiseHeightMap.Height;

		for (int y = 0; y < height; ++y)
		{
			for (int x = 0; x < width; ++x)
			{
				float heightValue = noiseHeightMap[x, y];
				var cell = genInfo.Cells[x, y];
				AddComponentsToEmptyMapCell(cell, heightValue, genInfo);
			}
		}
	}

	protected abstract void AddComponentsToEmptyMapCell(MapCell cell, float heightValue, WorldGenerationInfo genInfo);

	protected float SampleLayeredNoiseAtCoordinate(int x, int y, int octaves, WorldGenerationInfo genInfo)
	{
		float noise = 0.0f;

		for (int i = 0; i < octaves; ++i)
			noise += SampleNoiseAtCoordinate(x, y, genInfo);

		return noise;
	}

	protected virtual float SampleNoiseAtCoordinate(int x, int y, WorldGenerationInfo genInfo)
	{
		float scale = genInfo.NoiseGenArgs.Scale;
		var dimensions = genInfo.NoiseGenArgs.Dimensions;
		var offset = genInfo.NoiseGenArgs.Offset;
		var sample = Mathf.Torus(
			new Vector2(x + offset.X, y + offset.Y),
			dimensions.X,
			dimensions.Y);

		return NoiseGen.GetNoise(sample.X, sample.Y, sample.Z) * scale; // scale gets normalized anyways
	}

	protected virtual float SampleFalloffAtCoordinate(int x, int y, WorldGenerationInfo genInfo)
	{
		float falloffModifier = genInfo.NoiseGenArgs.FalloffModifier;

		var dimensions = genInfo.NoiseGenArgs.Dimensions;
		int width = dimensions.X;
		int height = dimensions.Y;

		float xx = x / (float)width * 2 - 1;
		float yy = y / (float)height * 2 - 1;

		float value = Math.Max(Math.Abs(xx), Math.Abs(yy));
		return value * falloffModifier;
	}

	protected Map<MapCell> GenerateEmptyCellMap(WorldGenerationInfo genInfo)
	{
		var dimensions = genInfo.NoiseGenArgs.Dimensions;
		var map = new Map<MapCell>(dimensions);
		map.Populate(() => new MapCell());

		return map;
	}

	protected Map<float> GenerateNoiseHeightMap(WorldGenerationInfo genInfo)
	{
		var dimensions = genInfo.NoiseGenArgs.Dimensions;
		int octaves = genInfo.NoiseGenArgs.Octaves;
		int width = dimensions.X;
		int height = dimensions.Y;
		var map = new Map<float>(width, height);

		for (int y = 0; y < height; ++y)
		{
			for (int x = 0; x < width; ++x)
			{
				map[x, y] = SampleLayeredNoiseAtCoordinate(x, y, octaves, genInfo);
			}
		}

		return map;
	}

	protected void NormalizeNoiseMap(Map<float> noiseMap, WorldGenerationInfo genInfo)
	{
		var dimensions = genInfo.NoiseGenArgs.Dimensions;
		int width = dimensions.X;
		int height = dimensions.Y;

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

	protected void ApplyFalloffValuesToMap(Map<float> noiseMap, WorldGenerationInfo genInfo)
	{
		var dimensions = genInfo.NoiseGenArgs.Dimensions;
		int width = dimensions.X;
		int height = dimensions.Y;

		for (int y = 0; y < height; ++y)
		{
			for (int x = 0; x < width; ++x)
				noiseMap[x, y] -= SampleFalloffAtCoordinate(x, y, genInfo);
		}
	}
}
