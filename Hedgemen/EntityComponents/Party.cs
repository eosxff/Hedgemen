using System.Collections.Generic;
using Petal.Framework.EC;
using Petal.Framework.Persistence;

namespace Hgm.EntityComponents;

public sealed class Party : IDataStorageHandler
{
	private List<PartyMember> _members = new();

	//public IReadOnlyList<PartyMember> Members
	public List<PartyMember> Members
		=> _members;

	public DataStorage WriteStorage()
	{
		var storage = new DataStorage(this);
		storage.WriteData("hgm:members", _members.WriteStorageList());
		return storage;
	}

	public void ReadStorage(DataStorage storage)
	{
		storage.ReadData("hgm:members", out var members, new List<DataStorage>());
		_members = members.ReadStorageList<PartyMember>();
	}
}
