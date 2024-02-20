using System;
using Microsoft.Xna.Framework;
using Petal.Framework.EC;
using Petal.Framework.Util;

namespace Hgm.Game.WorldGeneration;

public struct NoiseArgs
{
	public static NoiseArgs New()
		=> new()
		{
			Seed = 1025,
			Dimensions = new Vector2Int(384, 384),
			Scale = 50.0f,
			Octaves = 5,
			Frequency = 2.5f,
			Lacunarity = 2.75f,
			Offset = new Vector2Int(0, 0),
			FalloffModifier = 0.0f
		};

	public required int Seed;
	public required float Scale;
	public required int Octaves;
	public required float Frequency;
	public required float Lacunarity;
	public required Vector2Int Offset;
	public required float FalloffModifier;
	public required Vector2Int Dimensions;
}

public sealed class WorldGenerationInfo(NoiseArgs noiseGenArgs)
{
	public NoiseArgs NoiseGenArgs
	{
		get;
	} = noiseGenArgs;

	public Map<MapCell>? Cells
	{
		get;
		set;
	}
}
