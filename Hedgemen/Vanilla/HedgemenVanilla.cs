using System.Collections.Generic;
using Hgm.Vanilla.Modding;

namespace Hgm.Vanilla;

public class HedgemenVanilla : ForgeMod
{
	public override ForgeModManifest GetEmbeddedManifest()
	{
		return new ForgeModManifest
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
			Contact = new HedgemenModManifestContactInfo
			{
				Homepage = "https://github.com/eosxff/Hedgemen",
				Source = "https://github.com/eosxff/Hedgemen"
			},
			Dependencies = new HedgemenModManifestDependenciesInfo
			{
				Dlls = new List<string>(),
				Hedgemen = Hedgemen.Version.ToString(),
				IncompatibleMods = new List<string>(),
				Mods = new List<string>()
			},
			
			ModFileDll = string.Empty,
			ModMain = string.Empty,
			IsOverhaul = false
		};
	}
	
}