using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework;
using Petal.Framework.Persistence;

namespace Hgm;

public sealed class Campaign : IPersistent
{
	public CampaignSettings Settings
	{
		get;
		set;
	} = new();

	public PersistentData WriteData()
	{
		var data = new PersistentData(this);
		data.WriteData("hgm:campaign_settings", Settings);
		return data;
	}

	public void ReadData(PersistentData data)
	{
		Settings = data.ReadData("hgm:campaign_settings", new CampaignSettings());
	}
}

public sealed class CampaignSettings : IPersistent
{
	public List<NamespacedString> Mods
	{
		get;
		set;
	} = new();

	public bool Ironman
	{
		get;
		set;
	} = true;

	public CampaignDifficulty Difficulty
	{
		get;
		set;
	} = CampaignDifficulty.Normal;

	public PersistentData WriteData()
	{
		var modsList = Mods.Select(e => e.FullName).ToList();

		var data = new PersistentData(this);
		data.WriteField("hgm:campaign_mods", modsList);
		return data;
	}

	public void ReadData(PersistentData data)
	{
		// fixme why the fuck does code analysis break when using var instead of List<string> for modsList?
		data.ReadField("hgm:campaign_mods", out List<string> modsList, new List<string>());
		Mods = new List<NamespacedString>(modsList.Count);

		foreach(string mod in modsList)
			Mods.Add(new NamespacedString(mod));
	}
}

public enum CampaignDifficulty
{
	Easy,
	Normal,
	Hard
}
