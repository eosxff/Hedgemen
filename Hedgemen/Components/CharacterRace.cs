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

	public override DataStorage WriteStorage()
	{
		var record = base.WriteStorage();
		record.WriteData("hgm:race_name", RaceName);

		return record;
	}

	public override void ReadStorage(DataStorage storage)
	{
		RaceName = storage.ReadData("hgm:race_name", "human");
	}
}