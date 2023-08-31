using System;
using Petal.Framework;
using Petal.Framework.EC;
using Petal.Framework.Persistence;
using Petal.Framework.Util;

namespace Hgm.WorldGeneration;

public class WorldMap : IPersistent
{
	public Guid UniverseGuid
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
	}

	public WorldMap(Map<MapCell> cells)
	{
		Cells = cells;
	}

	public void Delete()
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
