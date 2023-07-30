using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Hgm.Components;
using Hgm.Vanilla.Scenes;
using Petal.Framework;
using Petal.Framework.Content;
using Petal.Framework.EC;
using Petal.Framework.IO;
using Petal.Framework.Modding;
using Petal.Framework.Util;

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

	protected override void OnLoadedToPetalModLoader()
	{
		Game.Logger.Debug($"Loaded {nameof(HedgemenVanilla)}");
	}

	protected override void PrePetalModLoaderModSetupPhase(ModLoaderSetupContext context)
	{
		var logger = Game.Logger;

		logger.Debug("I");
		logger.Warn("Love");
		logger.Error("These");
		logger.Critical("Colours");
	}

	protected override void Setup(ModLoaderSetupContext context)
	{
		Registers.SetupRegisters(Game.Registry);

		var logger = Game.Logger;
		var assetsRegistryFound = Game.Registry.GetRegister("hgm:assets", out Register<object> assets);
		var assetLoader = Game.Assets;

		Game.OnSceneChanged += (sender, args) =>
		{
			logger.Debug($"Changing scene.");
		};

		var scene = SplashSceneFactory.NewScene(
			Game,
			new FileInfo("splash.png").Open(FileMode.Open));
		Game.ChangeScenes(scene);

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
			RegisterContent();
		});

		var mainMenuScene = MainMenuSceneFactory.NewScene(Game, assets);
		Game.ChangeScenes(mainMenuScene);
	}

	private void RegisterContent()
	{
		var logger = Game.Logger;
		var assetLoader = Game.Assets;
		var registry = Game.Registry;

		var assetsRegisterFound = Game.Registry.GetRegister("hgm:assets", out Register<object> assets);

		if (!assetsRegisterFound)
			return;
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
		var entityManifestJson = file.ReadString(Encoding.UTF8);
		var entityManifest = EntityManifest.FromJson(entityManifestJson);

		if (entityManifest is not null)
		{
			logger.Debug($"Entity manifest component count: {entityManifest.Components.Count}");
			logger.Debug($"Entity component: " +
			             $"{entityManifest.Components["hgm:character_sheet"].ReadData<int>("hgm:strength")}.");
			logger.Debug($"Entity component: {
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
				lock (entity)
				{
					logger.Debug($"New entity strength: {entity.GetComponent<CharacterSheet>().Strength}");
				}
			});
		}

		var data = entity.WriteStorage();
		var entityClone = data.ReadData<Entity>();

		logger.Debug($"Test entity responds to {nameof(ChangeStatEvent)}: " +
		             $"{entityClone.WillRespondToEvent<ChangeStatEvent>()}");

		entityClone.RemoveComponent<CharacterSheet>();

		logger.Debug(
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
				IncompatibleMods = { },
				Mods = new List<string>()
			},

			ModFileDll = string.Empty,
			ModMain = string.Empty,
			IsOverhaul = false
		};
	}

	private static Hedgemen Game
		=> Hedgemen.Instance;
}
