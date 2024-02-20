using System;
using System.Collections.Generic;
using Hgm.Game.WorldGeneration;
using Petal.Framework.Persistence;

namespace Hgm.Game.Campaigning;

public sealed class CampaignUniverse(Campaign campaign) : IPersistent
{
	public Campaign Campaign
	{
		get;
	} = campaign;

	private readonly Dictionary<Guid, WorldMap> _loadedWorlds = [];

	public void AddWorld(WorldMap world)
	{
		_loadedWorlds.Add(GetUniqueGuid(), world);
	}

	public PersistentData WriteData()
	{
		var data = new PersistentData(this);
		return data;
	}

	public void ReadData(PersistentData data)
	{

	}

	private Guid GetUniqueGuid()
	{
		Guid guid;

		do
		{
			guid = Guid.NewGuid();
		} while (_loadedWorlds.ContainsKey(guid));

		return guid;
	}
}
