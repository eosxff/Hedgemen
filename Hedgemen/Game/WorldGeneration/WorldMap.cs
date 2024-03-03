using System;
using Petal.Framework;
using Petal.Framework.EC;
using Petal.Framework.Persistence;
using Petal.Framework.Util;

namespace Hgm.Game.WorldGeneration;

public class WorldMap(Map<MapCell> cells) : IPersistent
{
	public Guid WorldGuid
	{
		get;
		internal set;
	} = Guid.Empty;

	public NamespacedString Name
	{
		get;
		set;
	} = NamespacedString.Default;

	public Map<MapCell> Cells
	{
		get;
	} = cells;

	public void Destroy()
	{

	}

	public PersistentData WriteData()
	{
		var storage = new PersistentData(this);
		return storage;
	}

	public void ReadData(PersistentData data)
	{

	}
}
