using Petal.Framework.EC;
using Petal.Framework.Persistence;

namespace Hgm.Components;

/// <summary>
///     also a dummy class
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
		record.SyncDataAdd("hgm:race_name", RaceName);

		return record;
	}

	public override void ReadStorage(DataStorage storage)
	{
		RaceName = storage.SyncDataGet<string>("hgm:race_name", "human");
	}
}