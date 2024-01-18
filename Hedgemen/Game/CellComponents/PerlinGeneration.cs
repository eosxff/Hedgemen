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
		RegisterEvent<QueryMapPixelColorEvent>(QueryMapPixelColor);
	}

	private void SetHeight(SetHeightEvent e)
	{
		Height = e.Height;
	}

	private void QueryMapPixelColor(QueryMapPixelColorEvent e)
	{
		if (Self.GetSubscriberCountForEvent<QueryMapPixelColorEvent>() > 0) // todo lol
			return;

		e.MapPixelColor = Color.Red;
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
