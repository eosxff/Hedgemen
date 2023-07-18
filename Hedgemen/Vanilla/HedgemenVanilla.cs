using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Hgm.Components;
using Hgm.Vanilla.Scenes;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework.Content;
using Petal.Framework.EC;
using Petal.Framework.IO;
using Petal.Framework.Modding;

namespace Hgm.Vanilla;

public class HedgemenVanilla : PetalEmbeddedMod
{
	private static Hedgemen Game
		=> Hedgemen.Instance;

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
		Game.Registry.AddRegister(new Register<EntityComponent>(
			"hgm:entity_components",
			Manifest.ModID,
			Game.Registry));

		Game.Registry.AddRegister(new Register<CellComponent>(
			"hgm:cell_components",
			Manifest.ModID,
			Game.Registry));

		Game.Registry.AddRegister(new Register<object>(
			"hgm:assets",
			Manifest.ModID,
			Game.Registry));

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

		var registerContent = Task.Run(async delegate
		{
			await Task.Delay(2000);
			RegisterContent();

			var mainMenuScene = MainMenuSceneFactory.NewScene(Game, assets);
			Game.ChangeScenes(mainMenuScene);
		});

		Test();
	}

	private void RegisterContent()
	{
		var logger = Game.Logger;
		var assetLoader = Game.Assets;
		var contentRegistry = Game.Registry;

		var assetsRegistryFound = Game.Registry.GetRegister("hgm:assets", out Register<object> assets);

		if (!assetsRegistryFound)
			return;

		assets.AddKey(
			"hgm:ui/skin/button_hover_texture",
			assetLoader.LoadAsset<Texture2D>(new FileInfo("button_hover.png").Open(FileMode.Open)));

		assets.AddKey(
			"hgm:ui/skin/button_normal_texture",
			assetLoader.LoadAsset<Texture2D>(new FileInfo("button_normal.png").Open(FileMode.Open)));

		assets.AddKey(
			"hgm:ui/skin/button_input_texture",
			assetLoader.LoadAsset<Texture2D>(new FileInfo("button_input.png").Open(FileMode.Open)));

		assets.AddKey(
			"hgm:ui/small_font", assetLoader.LoadAsset<SpriteFont>("pixelade_regular"));

		assets.AddKey(
			"hgm:ui/medium_font", assetLoader.LoadAsset<SpriteFont>("pixelade_regular_32"));

		assets.AddKey(
			"hgm:ui/large_font", assetLoader.LoadAsset<SpriteFont>("pixelade_regular_64"));

		assets.AddKey(
			"hgm:ui/splash_texture",
			assetLoader.LoadAsset<Texture2D>("splash.png"));
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
			ModID = "hgm:mod",
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
				ReferencedDlls = new List<string>(),
				IncompatibleMods = { },
				Mods = new List<string>()
			},

			ModFileDll = string.Empty,
			ModMain = string.Empty,
			IsOverhaul = false
		};
	}
}
