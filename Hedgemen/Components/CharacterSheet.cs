using System;
using Petal.Framework.EntityComponent;
using Petal.Framework.EntityComponent.Persistence;

namespace Hgm.Components;

/// <summary>
/// dummy class
/// </summary>
public sealed class CharacterSheet : Component
{
	public static readonly ComponentInfo ComponentInfo = new()
	{
		ContentIdentifier = "hedgemen:character_sheet",
		ComponentType = typeof(CharacterSheet)
	};

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
	}
}