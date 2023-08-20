using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Hgm.Components;
using Hgm.WorldGeneration;
using Microsoft.Xna.Framework;
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
		var cartographer = HedgemenVanilla.Instance.Content.OverworldCartographer.Get();

		if (cartographer is not null)
		{
			Task.Run(() => GenerateMap(cartographer));
		}

		else
		{
			Game.Logger.Debug("Could not get overworld cartographer!");
		}
	}

	private void GenerateMap(Cartographer cartographer)
	{
		var mapDimensions = new Vector2Int(1024, 1024);
		Game.Logger.Debug($"Generating overworld {mapDimensions.X}x{mapDimensions.Y}!");

		var stopwatch = new Stopwatch();
		stopwatch.Start();

		cartographer.Generate(new CartographyOptions
		{
			MapDimensions = mapDimensions,
			Seed = 1025
		});

		stopwatch.Stop();

		Game.Logger.Debug($"Finished generating overworld ({Math.Round(stopwatch.Elapsed.TotalMilliseconds)}ms)");
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
