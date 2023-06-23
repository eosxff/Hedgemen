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

	public override SerializedData WriteObjectState()
	{
		var data = base.WriteObjectState();
		data.AddField("hedgemen:strength", Strength);
		data.AddField("hedgemen:dexterity", Dexterity);
		data.AddField("hedgemen:constitution", Constitution);
		data.AddField("hedgemen:intelligence", Intelligence);
		data.AddField("hedgemen:wisdom", Wisdom);
		data.AddField("hedgemen:charisma", Charisma);

		return data;
	}

	public override void ReadObjectState(SerializedData data)
	{
		Strength = data.GetField<int>("hedgemen:strength");
		Dexterity = data.GetField<int>("hedgemen:dexterity");
		Constitution = data.GetField<int>("hedgemen:constitution");
		Intelligence = data.GetField<int>("hedgemen:intelligence");
		Wisdom = data.GetField<int>("hedgemen:wisdom");
		Charisma = data.GetField<int>("hedgemen:charisma");
	}
}