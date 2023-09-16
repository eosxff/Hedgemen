using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework;
using Petal.Framework.Persistence;

namespace Hgm.Game;

public abstract class Campaign : IPersistent
{
	public CampaignDirectory Directory
	{
		get;
		set;
	}

	public CampaignSettings Settings
	{
		get;
		set;
	} = new();

	public virtual PersistentData WriteData()
	{
		var data = new PersistentData(this);
		data.WriteData("hgm:campaign_settings", Settings);
		data.WriteField("hgm:campaign_directory", Directory.Info.FullName);
		return data;
	}

	public virtual void ReadData(PersistentData data)
	{
		data.ReadField("hgm:campaign_directory", out string directoryName, string.Empty);
		Directory = new CampaignDirectory(new DirectoryInfo(directoryName));
		Settings = data.ReadData("hgm:campaign_settings", new CampaignSettings());
	}

	public abstract void StartCampaign();
	public abstract void LoadCampaign(PersistentData data);
}

public sealed class CampaignSettings : IPersistent
{
	public string CampaignName
	{
		get;
		set;
	} = "New Campaign";

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

	public PersistentData WriteData()
	{
		var data = new PersistentData(this);
		data.WriteField("hgm:mods", Mods.Select(e => e.FullName).ToList());
		data.WriteField("hgm:ironman", Ironman);
		data.WriteField("hgm:name", CampaignName);
		return data;
	}

	public void ReadData(PersistentData data)
	{
		// fixme why does code analysis break when using var instead of List<string> for modsList?
		data.ReadField("hgm:mods", out List<string> modsList, new List<string>());
		Mods = new List<NamespacedString>(modsList.Count);

		foreach(string mod in modsList)
			Mods.Add(new NamespacedString(mod));

		Ironman = data.ReadField("hgm:ironman", false);
		CampaignName = data.ReadField("hgm:campaign_name", "New Campaign");
	}
}
