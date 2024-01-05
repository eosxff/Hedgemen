using Hgm.Vanilla.WorldGeneration;
using Microsoft.Xna.Framework;
using Petal.Framework.EC;

namespace Hgm.Game.WorldGeneration;

public abstract class Terrain : CellComponent
{
	public abstract Color GetMapPixelColor();

	protected override void RegisterEvents()
	{
		base.RegisterEvents();
		RegisterEvent<QueryMapPixelColorEvent>(QueryMapPixelColor);
	}

	private void QueryMapPixelColor(QueryMapPixelColorEvent e)
	{
		e.MapPixelColor = GetMapPixelColor();
	}
}
