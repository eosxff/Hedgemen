using System.Collections.Generic;
using Petal.Framework.Modding;

namespace Hgm.Vanilla;

public class HedgemenVanilla : PetalMod
{
	public override PetalModManifest GetEmbeddedManifest()
	{
		return new PetalModManifest
		{
			SchemaVersion = 1,
			NamespacedID = "hedgemen:mod",
			Name = "Hedgemen",
			Version = Hedgemen.Version.ToString(),
			Description = "Open world roguelike sidescroller. It's not even a game yet lol.",
			Authors = new List<string>
			{
				"eosxff"
			},
			Contact = new PetalModManifestContactInfo
			{
				Homepage = "https://github.com/eosxff/Hedgemen",
				Source = "https://github.com/eosxff/Hedgemen"
			},
			Dependencies = new PetalModManifestDependenciesInfo
			{
				Dlls = new List<string>(),
				IncompatibleMods = new List<string>(),
				Mods = new List<string>()
			},
			
			ModFileDll = string.Empty,
			ModMain = string.Empty,
			IsOverhaul = false
		};
	}
	
}