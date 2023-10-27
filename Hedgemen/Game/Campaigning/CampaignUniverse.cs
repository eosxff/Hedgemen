using System;
using System.Collections.Generic;
using Hgm.Game.WorldGeneration;
using Petal.Framework.Persistence;

namespace Hgm.Game.Campaigning;

public sealed class CampaignUniverse : IPersistent
{
	public Campaign Campaign
	{
		get;
	}

	private Dictionary<Guid, WorldMap> _loadedWorlds = new();

	public CampaignUniverse(Campaign campaign)
	{
		Campaign = campaign;
	}

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
