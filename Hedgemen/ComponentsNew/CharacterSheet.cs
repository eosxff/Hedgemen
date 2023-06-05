using System;
using Petal.Framework.ECS;

namespace Hgm.ComponentsNew;

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

	public override void RegisterEvents()
	{
		RegisterEvent<ChangeStatEvent>(ChangeStat);
	}

	private void ChangeStat(ChangeStatEvent args)
	{
		Console.WriteLine("Hi");
	}
}