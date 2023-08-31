using Petal.Framework.EC;
using Petal.Framework.Persistence;

namespace Hgm.Components;

/// <summary>
/// also a dummy class
/// </summary>
public class CharacterRace : EntityComponent
{
	public string RaceName
	{
		get;
		set;
	} = "human";

	public override PersistentData WriteData()
	{
		var record = base.WriteData();
		record.WriteField("hgm:race_name", RaceName);

		return record;
	}

	public override void ReadData(PersistentData data)
	{
		RaceName = data.ReadField("hgm:race_name", "human");
	}
}
