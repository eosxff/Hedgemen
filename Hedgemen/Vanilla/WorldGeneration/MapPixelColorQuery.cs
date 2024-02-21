using Hgm.Game.WorldGeneration;
using Microsoft.Xna.Framework;
using Petal.Framework.EC;
using Petal.Framework.Util;

namespace Hgm.Vanilla.WorldGeneration;

public sealed class MapPixelColorQuery : CellEvent, IResettable
{
	public Color MapPixelColor
	{
		get;
		set;
	} = Color.LightGray;

	public DisplayPriority Priority
	{
		get;
		set;
	} = DisplayPriority.Noise;

	public void Reset()
	{
		Priority = DisplayPriority.Noise;
		MapPixelColor = Color.LightGray;
	}

	public enum DisplayPriority // todo maybe move to MapPixelColorQuery
	{
		Noise,
		Terrain,
		Biome,
		Place
	}
}
