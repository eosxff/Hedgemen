using System;
<<<<<<< Updated upstream
using Petal.Framework.EntityComponent;
using Petal.Framework.EntityComponent.Persistence;
=======
using Petal.Framework.EC;
using Petal.Framework.Persistence;
>>>>>>> Stashed changes

namespace Hgm.Components;

public class CharacterSheet : EntityComponent
{
<<<<<<< Updated upstream
	public static readonly ComponentInfo ComponentInfo = new()
	{
		ContentIdentifier = "hedgemen:character_sheet",
		ComponentType = typeof(CharacterSheet)
	};

	public int Strength
=======
	public int Strength
	{
		get;
		set;
	} = 10;

	public int Dexterity
>>>>>>> Stashed changes
	{
		get;
		set;
	} = 10;

<<<<<<< Updated upstream
	public int Dexterity
=======
	public int Constitution
>>>>>>> Stashed changes
	{
		get;
		set;
	} = 10;

<<<<<<< Updated upstream
	public int Constitution
=======
	public int Intelligence
>>>>>>> Stashed changes
	{
		get;
		set;
	} = 10;

<<<<<<< Updated upstream
	public int Intelligence
=======
	public int Wisdom
>>>>>>> Stashed changes
	{
		get;
		set;
	} = 10;

<<<<<<< Updated upstream
	public int Wisdom
	{
		get;
		set;
	} = 10;

=======
>>>>>>> Stashed changes
	public int Charisma
	{
		get;
		set;
	} = 10;

<<<<<<< Updated upstream
	public CharacterSheet()
	{
	}

	public override void PropagateEvent(EntityEvent e)
	{
		switch (e)
		{
			case StatChangeEvent args:
				ChangeStat(args.StatName, args.Amount);
				break;
		}
	}

	public void ChangeStat(string statName, int amount)
	{
		switch (statName)
		{
			case "strength":
				Strength += amount;
				break;
			case "dexterity":
				Dexterity += amount;
				break;
			case "constitution":
				Constitution += amount;
				break;
			case "intelligence":
				Intelligence += amount;
				break;
			case "wisdom":
				Wisdom += amount;
				break;
			case "charisma":
				Charisma += amount;
				break;
			default:
				return;
		}
	}

	public override ComponentInfo GetComponentInfo()
	{
		return ComponentInfo;
	}

	public override SerializedRecord WriteObjectState()
	{
		var record = base.WriteObjectState();
		record.AddField("hedgemen:strength", Strength);
		record.AddField("hedgemen:dexterity", Dexterity);
		record.AddField("hedgemen:constitution", Constitution);
		record.AddField("hedgemen:intelligence", Intelligence);
		record.AddField("hedgemen:wisdom", Wisdom);
		record.AddField("hedgemen:charisma", Charisma);

		return record;
	}

	public override void ReadObjectState(SerializedRecord record)
	{
		Strength = record.GetField<int>("hedgemen:strength");
		Dexterity = record.GetField<int>("hedgemen:dexterity");
		Constitution = record.GetField<int>("hedgemen:constitution");
		Intelligence = record.GetField<int>("hedgemen:intelligence");
		Wisdom = record.GetField<int>("hedgemen:wisdom");
		Charisma = record.GetField<int>("hedgemen:charisma");
=======
	public override void RegisterEvents()
	{
		RegisterEvent<ChangeStatEvent>(ChangeStat);
	}

	public override SerializedData WriteObjectState()
	{
		var data = base.WriteObjectState();
		data.AddField("hedgemen:strength", Strength);

		return data;
	}

	public override void ReadObjectState(SerializedData data)
	{
		Strength = data.GetField("hedgemen:strength", 10);
		Console.WriteLine($"Strength: {Strength}");
	}

	private void ChangeStat(ChangeStatEvent args)
	{
		if (args.StatName == "strength")
			Strength += args.ChangeAmount;
>>>>>>> Stashed changes
	}
}