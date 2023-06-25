using Petal.Framework.EC;
using Petal.Framework.Persistence;

namespace Hgm.Components;

/// <summary>
/// Dummy
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

	public override SerializedData WriteObjectState()
	{
		var data = base.WriteObjectState();
		data.AddField("hedgemen:perlin_height", Height);

		return data;
	}

	public override void ReadObjectState(SerializedData data)
	{
		Height = data.GetField("hedgemen:perlin_height", 0.0f);
	}
}