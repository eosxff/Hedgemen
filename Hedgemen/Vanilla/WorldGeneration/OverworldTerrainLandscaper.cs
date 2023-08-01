using System;
using Hgm.Components;
using Hgm.WorldGeneration;
using Microsoft.Xna.Framework;
using Petal.Framework.EC;
using Petal.Framework.Util;

namespace Hgm.Vanilla.WorldGeneration;

public sealed class OverworldTerrainLandscaper : ILandscaper
{
	public void PerformGenerationStep(Map<MapCell> cells, CartographyOptions options)
	{
		var gen = new FastNoiseLite(options.Seed);
		cells.Iterate((e, pos) => GetPerlinGeneration(e, pos, gen, options));
	}
	public bool ShouldPerformGenerationStep(Map<MapCell> cells, CartographyOptions options)
	{
		return true;
	}

	private void GetPerlinGeneration(MapCell cell, Vector2Int position, FastNoiseLite gen, CartographyOptions options)
	{
		var perlin = new PerlinGeneration();
		var cylinderCoords = ToCylinderCoordinates(position, options);
		perlin.Height = gen.GetNoise(cylinderCoords.X, cylinderCoords.Y, cylinderCoords.Z);

		cell.AddComponent(perlin);
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
