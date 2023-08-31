using Petal.Framework.EC;
using Petal.Framework.Persistence;

namespace Hgm.Components;

/// <summary>
/// Dummy.
/// </summary>
public sealed class PerlinGeneration : CellComponent
{
	private float _height = 0.0f;

	public float Height
	{
		get => _height;
		set => _height = value;
	}

	protected override void RegisterEvents()
	{
		RegisterEvent<SetHeightEvent>(SetHeight);
	}

	private void SetHeight(SetHeightEvent e)
	{
		Height = e.Height;
	}

	public override PersistentData WriteData()
	{
		var data = base.WriteData();
		data.WriteField("hgm:perlin_height", Height);

		return data;
	}

	public override void ReadData(PersistentData data)
	{
		data.ReadField("hgm:perlin_height", out _height, 0.0f);
	}
}
