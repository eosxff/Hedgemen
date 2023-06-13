using Petal.Framework.EC;
using Petal.Framework.Persistence;

namespace Hgm.Components;

/// <summary>
///     also a dummy class
/// </summary>
public class CharacterRace : EntityComponent
{
	public string RaceName { get; set; } = "human";

	public override SerializedData WriteObjectState()
	{
		var record = base.WriteObjectState();
		record.AddField("hedgemen:race_name", RaceName);

		return record;
	}

	public override void ReadObjectState(SerializedData data)
	{
		RaceName = data.GetField<string>("hedgemen:race_name", "human");
	}
}