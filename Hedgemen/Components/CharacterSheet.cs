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

	protected override void RegisterEvents()
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
		data.WriteData("hgm:strength", Strength);
		data.WriteData("hgm:dexterity", Dexterity);
		data.WriteData("hgm:constitution", Constitution);
		data.WriteData("hgm:intelligence", Intelligence);
		data.WriteData("hgm:wisdom", Wisdom);
		data.WriteData("hgm:charisma", Charisma);

		return data;
	}

	public override void ReadStorage(DataStorage storage)
	{
		Strength = storage.ReadData<int>("hgm:strength");
		Dexterity = storage.ReadData<int>("hgm:dexterity");
		Constitution = storage.ReadData<int>("hgm:constitution");
		Intelligence = storage.ReadData<int>("hgm:intelligence");
		Wisdom = storage.ReadData<int>("hgm:wisdom");
		Charisma = storage.ReadData<int>("hgm:charisma");
	}
}