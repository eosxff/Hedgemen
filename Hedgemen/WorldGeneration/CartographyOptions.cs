using Microsoft.Xna.Framework;

namespace Hgm.WorldGeneration;

public readonly struct CartographyOptions
{
	public required Vector2Int MapDimensions
	{
		get;
		init;
	}

	public required int Seed
	{
		get;
		init;
	}

	
}