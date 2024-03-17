using System;
using System.Collections.Generic;
using System.IO;
using Petal.Framework;
using Petal.Framework.Modding;
using Petal.Framework.Modding.New;
using Petal.Framework.Persistence;

namespace Hgm.Vanilla;

public sealed class HedgemenDebugMod : PetalEmbeddedMod
{
	public static readonly NamespacedString ModID = new("hgm_debug:mod");

	protected override void PrePetalModLoaderModSetupPhase(ModLoaderSetupContext context)
	{
		var logger = context.Game.Logger;

		logger.Debug("I");
		logger.Info("Love");
		logger.Warn("All");
		logger.Error("These");
		logger.Critical("Colours");
	}

	protected override void Setup(ModLoaderSetupContext context)
	{
		// ...

		/*var proxyModLoader = new PetalModProxyLoader(context.Game.Logger);

		var mods = proxyModLoader.LoadModProxies(new PetalModProxyLoadArgs
		{
			FirstPartyProxies = [ ],
			Game = Game,
			LoadFirstPartyProxiesOnly = false,
			ProxyDirectory = new DirectoryInfo("mods"),
			ProxyManifestFileName = "proxy.json"
		});*/
		var typeInfo = JsonManifest.SupportedTypes;

		var jsonManifest = new JsonManifest();
		jsonManifest.WriteType("hgm:strength", 10, typeInfo.Int32);
		jsonManifest.WriteType("hgm:dexterity", 1025, typeInfo.Int32);

		Console.WriteLine(jsonManifest.ReadType("hgm:strength", typeInfo.Int32));

		PetalGame.Petal.Logger.Debug($"JsonManifest: {jsonManifest}.");
	}

	public override PetalModManifest GetEmbeddedManifest()
	{
		return new PetalModManifest
		{
			SchemaVersion = 1,
			ModID = ModID,
			Name = "Hedgemen Debug",
			Version = Hedgemen.Version,
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
				ReferencedDlls = new List<string>(),
				IncompatibleMods = new List<NamespacedString>(),
				Mods = new List<NamespacedString>()
			},
			ModFileDll = string.Empty,
			ModMain = string.Empty,
			IsOverhaul = false
		};
	}
}
