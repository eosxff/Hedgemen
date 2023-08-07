using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
using Petal.Framework.Util;
using Petal.Framework.Util.Coroutines;

namespace Hgm.Vanilla;

public class HedgemenVanilla : PetalEmbeddedMod
{
	public static readonly NamespacedString ModID = new("hgm:mod");

	private static HedgemenVanilla _instance;

	public static HedgemenVanilla Instance
	{
		get
		{
			PetalExceptions.ThrowIfNull(_instance);
			return _instance;
		}

		private set => _instance = value;
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
		};

		var scene = SplashSceneFactory.NewScene(
			Game,
			new FileInfo("splash.png").Open(FileMode.Open));
		Game.ChangeScenes(scene);

		var cartographer = Content.OverworldCartographer.Get();
		var map = cartographer.Generate(new CartographyOptions
		{
			MapDimensions = new Vector2Int(128, 128),
			Seed = 1337
		});

		Test();
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

	private void Test()
	{
		var logger = Game.Logger;

		var entity = new Entity();
		entity.AddComponent(new CharacterSheet());
		entity.AddComponent(new CharacterRace
		{
			RaceName = "high elf"
		});

		var file = new FileInfo("sentient_apple_pie.json");
		string entityManifestJson = file.ReadString(Encoding.UTF8);
		var entityManifest = EntityManifest.FromJson(entityManifestJson);

		if (entityManifest is not null)
		{
			logger.Info($"Entity manifest component count: {entityManifest.Components.Count}");
			logger.Info($"Entity component: " +
			             $"{entityManifest.Components["hgm:character_sheet"].ReadData<int>("hgm:strength")}.");
			logger.Info($"Entity component: {
				entityManifest.Components["hgm:character_race"]
					.ReadData<string>("hgm:race_name")}.");
		}
		else
		{
			logger.Warn($"File '{file.FullName}' failed to create valid {nameof(EntityManifest)}.");
		}

		for (int i = 0; i < 1; ++i)
		{
			if (entity.WillRespondToEvent<ChangeStatEvent>())
			{
				entity.PropagateEvent(new ChangeStatEvent
				{
					Sender = entity,
					ChangeAmount = 1015,
					StatName = "strength"
				});
			}

			entity.PropagateEventIfResponsive(new ChangeStatEvent
			{
				Sender = entity,
				ChangeAmount = 10,
				StatName = "constitution"
			});
		}

		if (entity.WillRespondToEvent<ChangeStatEvent>())
		{
			var task = entity.PropagateEventAsync(new ChangeStatEvent
			{
				Sender = entity,
				ChangeAmount = 1015,
				StatName = "strength"
			});

			task.ContinueWith(_ =>
			{
				logger.Info($"New entity strength: {entity.GetComponent<CharacterSheet>()?.Strength}");
			});
		}

		var data = entity.WriteStorage();
		var entityClone = data.ReadData<Entity>();

		logger.Info($"Test entity responds to {nameof(ChangeStatEvent)}: " +
		             $"{entityClone.WillRespondToEvent<ChangeStatEvent>()}");

		entityClone.RemoveComponent<CharacterSheet>();

		logger.Info(
			$"Test does entity respond to {nameof(ChangeStatEvent)} after removing all referenced components: " +
			$"{entityClone.WillRespondToEvent<ChangeStatEvent>()}");

		entity.RemoveComponent<CharacterSheet>();
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
