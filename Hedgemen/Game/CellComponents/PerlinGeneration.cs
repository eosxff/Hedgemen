using Hgm.Game.WorldGeneration;
using Hgm.Vanilla.WorldGeneration;
using Microsoft.Xna.Framework;
using Petal.Framework.EC;
using Petal.Framework.Persistence;

namespace Hgm.Game.CellComponents;

/// <summary>
/// Dummy.
/// </summary>
public sealed class PerlinGeneration : CellComponent
{
	private float _height;

	public float Height
	{
		get => _height;
		set => _height = value;
	}

	protected override void RegisterEvents()
	{
		RegisterEvent<SetHeightEvent>(SetHeight);
		RegisterEvent<MapPixelColorQuery>(QueryMapPixelColor);
		RegisterEvent<WorldCellInfoQuery>(QueryWorldCellInfo);
	}

	private void SetHeight(SetHeightEvent e)
	{
		Height = e.Height;
	}

	private void QueryMapPixelColor(MapPixelColorQuery e)
	{
		if(e.Priority > MapPixelColorQuery.DisplayPriority.Noise)
            return;

		e.MapPixelColor = Color.Lerp(Color.White, Color.Black, _height);
        e.Priority = MapPixelColorQuery.DisplayPriority.Noise;
	}

	private void QueryWorldCellInfo(WorldCellInfoQuery e)
	{
		e.NoiseHeight = _height;
	}

	public override PersistentData WriteData()
	{
		var data = base.WriteData();
		data.WriteField("hgm:noise_height", Height);

		return data;
	}

	public override void ReadData(PersistentData data)
	{
		data.ReadField("hgm:noise_height", out _height);
	}
}
