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

	public Cartographer.DisplayPriority Priority
	{
		get;
		set;
	} = Cartographer.DisplayPriority.Noise;

	public void Reset()
	{
		Priority = Cartographer.DisplayPriority.Noise;
		MapPixelColor = Color.LightGray;
	}
}
