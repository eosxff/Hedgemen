using Petal.Framework.EC;
using Petal.Framework.Persistence;

namespace Hgm.Components;

/// <summary>
/// Dummy.
/// </summary>
public sealed class PerlinGeneration : CellComponent
{
	public float Height
	{
		get;
		set;
	} = 0.0f;

	public override void RegisterEvents()
	{
		RegisterEvent<SetHeightEvent>(SetHeight);
	}
	
	private void SetHeight(SetHeightEvent e)
	{
		Height = e.Height;
	}

	public override DataStorage WriteStorage()
	{
		var data = base.WriteStorage();
		data.WriteData("hgm:perlin_height", Height);

		return data;
	}

	public override void ReadStorage(DataStorage storage)
	{
		Height = storage.ReadData("hgm:perlin_height", 0.0f);
	}
}