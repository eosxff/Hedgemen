using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework.Modding;
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
			"hgm:main_menu_font", Game.Assets.LoadAsset<SpriteFont>("pixelade_regular_32"));
	}

	protected override void PostPetalModLoaderSetupPhase(ModLoaderSetupContext context)
	{
		var scene = Game.Scene;

		scene?.Root.Add(new Text
		{
			Font = Game.ContentRegistry.Get<SpriteFont>("hgm:main_menu_font"),
			Bounds = new Rectangle(50, 50, 64, 24),
			Color = Color.White,
			Message = "Hedgemen!",
			Scale = 1.0f
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
				Dlls = new List<string>(),
				IncompatibleMods = new List<string>(),
				Mods = new List<string>()
			},
			
			ModFileDll = string.Empty,
			ModMain = string.Empty,
			IsOverhaul = false
		};
	}
}