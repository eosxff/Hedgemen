using Petal.Framework.EC;
using Petal.Framework.Persistence;

namespace Hgm.Components;

public class CharacterSheet : EntityComponent
{
	public int Strength
	{
		get;
		set;
	} = 10;

	public int Dexterity
	{
		get;
		set;
	} = 10;

	public int Constitution
	{
		get;
		set;
	} = 10;

	public int Intelligence
	{
		get;
		set;
	} = 10;

	public int Wisdom
	{
		get;
		set;
	} = 10;

	public int Charisma
	{
		get;
		set;
	} = 10;

	public CharacterSheet()
	{

	}

	public override void RegisterEvents()
	{
		RegisterEvent<ChangeStatEvent>(ChangeStat);
	}

	private void ChangeStat(ChangeStatEvent args)
	{
		if (args.StatName == "strength")
			Strength += args.ChangeAmount;
	}

	public override DataStorage WriteStorage()
	{
		var data = base.WriteStorage();
		data.SyncDataAdd("hgm:strength", Strength);
		data.SyncDataAdd("hgm:dexterity", Dexterity);
		data.SyncDataAdd("hgm:constitution", Constitution);
		data.SyncDataAdd("hgm:intelligence", Intelligence);
		data.SyncDataAdd("hgm:wisdom", Wisdom);
		data.SyncDataAdd("hgm:charisma", Charisma);

		return data;
	}

	public override void ReadStorage(DataStorage storage)
	{
		Strength = storage.SyncDataGet<int>("hgm:strength");
		Dexterity = storage.SyncDataGet<int>("hgm:dexterity");
		Constitution = storage.SyncDataGet<int>("hgm:constitution");
		Intelligence = storage.SyncDataGet<int>("hgm:intelligence");
		Wisdom = storage.SyncDataGet<int>("hgm:wisdom");
		Charisma = storage.SyncDataGet<int>("hgm:charisma");
	}
}