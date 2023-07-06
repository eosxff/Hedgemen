using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Hgm.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework.EC;
using Petal.Framework.Graphics;
using Petal.Framework.IO;
using Petal.Framework.Modding;
using Petal.Framework.Persistence;
using Petal.Framework.Scenery;
using Petal.Framework.Scenery.Nodes;

namespace Hgm.Vanilla;

public class HedgemenVanilla : PetalMod
{
	private static Hedgemen Game
		=> Hedgemen.Instance;

	protected override void OnLoadedToPetalModLoader()
	{
		Game.Logger.Debug($"Loaded {nameof(HedgemenVanilla)}");
	}

	protected override void PrePetalModLoaderModSetupPhase(ModLoaderSetupContext context)
	{
		
	}

	protected override void Setup(ModLoaderSetupContext context)
	{
		Game.Logger.Debug($"Registering content for vanilla!");

		Game.ContentRegistry.Register(
			"hgm:ui/skin/button_hover_texture",
			Game.Assets.LoadAsset<Texture2D>(new FileInfo("button_hover.png").Open(FileMode.Open)));
		
		Game.ContentRegistry.Register(
			"hgm:ui/skin/button_normal_texture",
			Game.Assets.LoadAsset<Texture2D>(new FileInfo("button_normal.png").Open(FileMode.Open)));
		
		Game.ContentRegistry.Register(
			"hgm:ui/skin/button_input_texture",
			Game.Assets.LoadAsset<Texture2D>(new FileInfo("button_input.png").Open(FileMode.Open)));
		
		Game.ContentRegistry.Register(
			"hgm:ui/small_font", Game.Assets.LoadAsset<SpriteFont>("pixelade_regular"));
		
		Game.ContentRegistry.Register(
			"hgm:ui/medium_font", Game.Assets.LoadAsset<SpriteFont>("pixelade_regular_32"));
		
		Game.ContentRegistry.Register(
			"hgm:ui/large_font", Game.Assets.LoadAsset<SpriteFont>("pixelade_regular_64"));
		
		var skin = Skin.FromJson(new FileInfo("skin.json").ReadString(Encoding.UTF8), Game.ContentRegistry);
		
		var scene = new Scene(
			new Stage(), skin)
		{
			BackgroundColor = new Color(232, 190, 198, 255),
			ViewportAdapter = new BoxingViewportAdapter(
				Game.GraphicsDevice,
				Game.Window, 
				new Vector2Int(640, 360))
		};

		Game.Logger.Critical(
			$"{scene.Skin.ContentRegistry.Get<Texture2D>("hgm:ui/skin/button_input_texture").HasItem}");
		Game.Logger.Critical($"{scene.Skin.Button.InputTexture.HasItem}");

		Game.ChangeScenes(scene);

		Test();
	}

	private void Test()
	{
		var logger = Game.Logger;
		
		/*logger.Debug($"Just testing some garbage.");
		
		var entity = new Entity();
		entity.AddComponent(new CharacterSheet());
		entity.AddComponent(new CharacterRace
		{
			RaceName = "high elf"
		});

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
		
		logger.Error($"Testing if error applies in {(Game.IsDebug ? "Debug" : logger.LogLevel.ToString())}");
		logger.Critical($"Testing if critical applies in {(Game.IsDebug ? "Debug" : logger.LogLevel.ToString())}");

		entity.RemoveComponent<CharacterSheet>();*/
		
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
	}

	protected override void PostPetalModLoaderSetupPhase(ModLoaderSetupContext context)
	{
		var scene = Game.Scene;

		if (scene is null)
			return;

		scene.Root.Add(new Text
		{
			Font = scene.Skin.Font.LargeFont,
			Bounds = new Rectangle(32, 32, 64, 24),
			Color = Color.White,
			Message = "Hedgemen!",
			Scale = 0.75f
		});

		Game.Logger.Critical($"{scene.Skin.Button.HoverTexture.ContentID}");
		var startButton = scene.Root.Add(new Button(scene.Skin)
		{
			Anchor = Anchor.CenterLeft,
			Bounds = new Rectangle(32, -56, 128, 40)
		});

		startButton.Add(new Text
		{
			Anchor = Anchor.Center,
			Font = scene.Skin.Font.MediumFont,
			Message = "Singleplayer",
			Scale = 0.5f
		});
		
		var exitButton = scene.Root.Add(new Button(scene.Skin)
		{
			Anchor = Anchor.CenterLeft,
			Bounds = new Rectangle(32, -8, 128, 40)
		});

		exitButton.OnMousePressed += (sender, args) =>
		{
			Game.Exit();
		};
		
		exitButton.Add(new Text
		{
			Anchor = Anchor.Center,
			Font = scene.Skin.Font.MediumFont,
			Message = "Exit",
			Scale = 0.5f
		});
	}

	public override PetalModManifest GetEmbeddedManifest()
	{
		return new PetalModManifest
		{
			SchemaVersion = 1,
			NamespacedID = "hgm:mod",
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
				IncompatibleMods = new List<string>(),
				Mods = new List<string>()
			},
			
			ModFileDll = string.Empty,
			ModMain = string.Empty,
			IsOverhaul = false
		};
	}
}