<<<<<<< Updated upstream
﻿using Petal.Framework.EntityComponent;
using Petal.Framework.EntityComponent.Persistence;

namespace Hgm.Components;

/// <summary>
/// also a dummy class
/// </summary>
public class CharacterRace : Component
{
	public static readonly ComponentInfo ComponentInfo = new()
	{
		ContentIdentifier = "hedgemen:character_race",
		ComponentType = typeof(CharacterRace)
	};
	
	public string RaceName
	{
		get;
		set;
	} = "human";

	public override ComponentInfo GetComponentInfo()
		=> ComponentInfo;

	public override SerializedRecord WriteObjectState()
	{
		var record = base.WriteObjectState();
		record.AddField("hedgemen:race_name", RaceName);
		
		return record;
	}

	public override void ReadObjectState(SerializedRecord record)
	{
		RaceName = record.GetField<string>("hedgemen:race_name", "human");
=======
﻿using Petal.Framework.EC;
using Petal.Framework.Persistence;

namespace Hgm.Components;

public class CharacterRace : EntityComponent
{
	public required string Name
	{
		get;
		set;
	}
	
	public override void RegisterEvents()
	{
		
	}

	public override SerializedData WriteObjectState()
	{
		return base.WriteObjectState();
	}

	public override void ReadObjectState(SerializedData data)
	{
		base.ReadObjectState(data);
>>>>>>> Stashed changes
	}
}