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

	protected override void RegisterEvents()
	{
		RegisterEvent<ChangeStatEvent>(ChangeStat);
	}

	private void ChangeStat(ChangeStatEvent args)
	{
		if (args.StatName == "strength")
			Strength += args.ChangeAmount;
	}

	public override PersistentData WriteData()
	{
		var storage = base.WriteData();
		storage.WriteField("hgm:strength", Strength);
		storage.WriteField("hgm:dexterity", Dexterity);
		storage.WriteField("hgm:constitution", Constitution);
		storage.WriteField("hgm:intelligence", Intelligence);
		storage.WriteField("hgm:wisdom", Wisdom);
		storage.WriteField("hgm:charisma", Charisma);

		return storage;
	}

	public override void ReadData(PersistentData data)
	{
		Strength = data.ReadField<int>("hgm:strength");
		Dexterity = data.ReadField<int>("hgm:dexterity");
		Constitution = data.ReadField<int>("hgm:constitution");
		Intelligence = data.ReadField<int>("hgm:intelligence");
		Wisdom = data.ReadField<int>("hgm:wisdom");
		Charisma = data.ReadField<int>("hgm:charisma");
	}
}
