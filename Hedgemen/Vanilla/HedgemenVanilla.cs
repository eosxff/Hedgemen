using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Hgm.Components;
using Hgm.Vanilla.Scenes;
using Hgm.WorldGeneration;
using Microsoft.Xna.Framework;
using Petal.Framework;
using Petal.Framework.Content;
using Petal.Framework.EC;
using Petal.Framework.IO;
using Petal.Framework.Modding;
using Petal.Framework.Persistence;
using Petal.Framework.Util;
using Petal.Framework.Util.Coroutines;

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
		var logger = Game.Logger;

		logger.Debug("I");
		logger.Info("Love");
		logger.Warn("All");
		logger.Error("These");
		logger.Critical("Colours");
	}

	protected override void Setup(ModLoaderSetupContext context)
	{
		var logger = Game.Logger;

		Registers.SetupRegisters(Game.Registry);
		Content.Setup(Registers);

		Game.OnSceneChanged += (sender, args) =>
		{
			logger.Info($"Changing scene.");

			args.NewScene.AfterUpdate += (sender1, args1) =>
			{
				Game.Logger.Debug("sdkg");
			};
		};

		var scene = SplashSceneFactory.NewScene(
			Game,
			new FileInfo("splash.png").Open(FileMode.Open));
		Game.ChangeScenes(scene);

		RegisterContentThenChangeScenes();
	}

	private async void RegisterContentThenChangeScenes()
	{
		var logger = Game.Logger;
		var assetsRegistryFound = Game.Registry.GetRegister("hgm:assets", out Register<object> assets);

		if (!assetsRegistryFound)
			return;

		await Task.Run(async delegate
		{
			await Task.Delay(1000);
		});

		var mainMenuScene = MainMenuSceneFactory.NewScene(Game, assets);
		Game.ChangeScenes(mainMenuScene);
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
			Version = Hedgemen.HedgemenVersion.ToString(),
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
