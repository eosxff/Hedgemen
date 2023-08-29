using Microsoft.Xna.Framework;
using Petal.Framework.EC;

namespace Hgm.Vanilla.WorldGeneration;

public sealed class QueryMapPixelColorEvent : CellEvent
{
	public Color MapPixelColor
	{
		get;
		set;
	} = Color.Black;
}
