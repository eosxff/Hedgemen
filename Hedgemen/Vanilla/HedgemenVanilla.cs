using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Hgm.Game.CampaignSystem;
using Hgm.Game.Scenes;
using Petal.Framework;
using Petal.Framework.Content;
using Petal.Framework.IO;
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

		/*var manifest =
			Petal.Framework.Persistence.Manifest.FromJson(
				new FileInfo("hedgemen_campaign_setting.json").ReadString(Encoding.UTF8));

		var campaignSetting = manifest.NewInstance<CampaignSetting>();

		Game.Logger.Debug($"Campaign setting mod count: {campaignSetting.Mods.Count}");*/

		Game.OnSceneChanged += (sender, args) =>
		{
			logger.Info($"Changing scene.");
		};
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
