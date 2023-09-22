﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Hgm.Game.Scenes;
using Petal.Framework;
using Petal.Framework.Content;
using Petal.Framework.Modding;
using Petal.Framework.Scenery;
using Petal.Framework.Scenery.Nodes;
using Petal.Framework.Util;

namespace Hgm.Vanilla;

public class HedgemenVanilla : PetalEmbeddedMod
{
	public static readonly NamespacedString ModID = new("hgm:mod");

	private static HedgemenVanilla HedgemenVanillaInstance;

	public static HedgemenVanilla Instance
	{
		get
		{
			PetalExceptions.ThrowIfNull(HedgemenVanillaInstance);
			return HedgemenVanillaInstance;
		}

		private set => HedgemenVanillaInstance = value;
	}

	public HedgemenVanilla()
	{
		Instance = this;
	}

	public HedgemenRegisters Registers
	{
		get;
	} = new();

	public HedgemenContent Content
	{
		get;
	} = new();

	protected override void OnLoadedToPetalModLoader()
	{
		Game.Logger.Info($"Loaded {nameof(HedgemenVanilla)}");
	}

	protected override void PrePetalModLoaderModSetupPhase(ModLoaderSetupContext context)
	{

	}

	protected override void Setup(ModLoaderSetupContext context)
	{
		var logger = Game.Logger;

		Registers.SetupRegisters(Game.Registry);
		Content.Setup(Registers);

		Game.OnSceneChanged += (sender, args) =>
		{
			logger.Info($"Changing scene.");
		};

		var scene = new StartupSplashScene(
			new Stage(),
			new Skin(Registers.Assets),
			new FileInfo("splash.png").Open(FileMode.Open));
		Game.ChangeScenes(scene);

		RegisterContentThenChangeScenes();
	}

	private /*async*/ void RegisterContentThenChangeScenes()
	{
		var logger = Game.Logger;
		var assetsRegistryFound = Game.Registry.GetRegister("hgm:assets", out Register<object> assets);

		if (!assetsRegistryFound)
			return;

		/*await Task.Run(async delegate
		{
			await Task.Delay(1000);
		});*/

		Game.ChangeScenes(new MainMenuScene(Registers.Assets));
	}

	protected override void PostPetalModLoaderSetupPhase(ModLoaderSetupContext context)
	{

	}

	public override PetalModManifest GetEmbeddedManifest()
	{
		return new PetalModManifest
		{
			SchemaVersion = 1,
			ModID = ModID,
			Name = "Hedgemen",
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

	private static Hedgemen Game
		=> Hedgemen.Instance;
}
