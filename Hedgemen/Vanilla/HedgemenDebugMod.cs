using System.Collections.Generic;
using System.IO;
using System.Text;
using Hgm.Components;
using Petal.Framework;
using Petal.Framework.EC;
using Petal.Framework.IO;
using Petal.Framework.Modding;

namespace Hgm.Vanilla;

public sealed class HedgemenDebugMod : PetalEmbeddedMod
{
	public static readonly NamespacedString ModID = new("hgm_debug:mod");

	protected override void Setup(ModLoaderSetupContext context)
	{
		Test();
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
		bool entityCloneCreated = data.ReadData<Entity>(out var entityClone);

		Game.Logger.Critical(entityCloneCreated.ToString());

		logger.Info($"Test entity responds to {nameof(ChangeStatEvent)}: " +
		             $"{entityClone.WillRespondToEvent<ChangeStatEvent>()}");

		entityClone.RemoveComponent<CharacterSheet>();

		logger.Info(
			$"Test does entity respond to {nameof(ChangeStatEvent)} after removing all referenced components: " +
			$"{entityClone.WillRespondToEvent<ChangeStatEvent>()}");

		entity.RemoveComponent<CharacterSheet>();
	}

	public override PetalModManifest GetEmbeddedManifest()
	{
		return new PetalModManifest
		{
			SchemaVersion = 1,
			ModID = ModID,
			Name = "Hedgemen Debug",
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
