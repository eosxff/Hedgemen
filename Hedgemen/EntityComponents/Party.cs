using System.Collections.Generic;
using Petal.Framework.EC;
using Petal.Framework.Persistence;

namespace Hgm.EntityComponents;

public sealed class Party : IPersistent
{
	private List<PartyMember> _members = new();

	//public IReadOnlyList<PartyMember> Members
	public List<PartyMember> Members
		=> _members;

	public PersistentData WriteData()
	{
		var storage = new PersistentData(this);
		storage.WriteField("hgm:members", _members.WriteStorageList());
		return storage;
	}

	public void ReadData(PersistentData data)
	{
		data.ReadField("hgm:members", out var members, new List<PersistentData>());
		_members = members.ReadStorageList<PartyMember>();
	}
}
