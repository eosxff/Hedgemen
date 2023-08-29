using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Hgm.Components;
using Hgm.EntityComponents;
using Hgm.Vanilla.WorldGeneration;
using Hgm.WorldGeneration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework;
using Petal.Framework.EC;
using Petal.Framework.IO;
using Petal.Framework.Modding;
using Petal.Framework.Scenery;
using Petal.Framework.Scenery.Nodes;
using Petal.Framework.Util.Coroutines;

namespace Hgm.Vanilla;

public sealed class HedgemenDebugMod : PetalEmbeddedMod
{
	public static readonly NamespacedString ModID = new("hgm_debug:mod");

	protected override void PrePetalModLoaderModSetupPhase(ModLoaderSetupContext context)
	{
		var logger = context.Game.Logger;

		logger.Debug("I");
		logger.Info("Love");
		logger.Warn("All");
		logger.Error("These");
		logger.Critical("Colours");
	}

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

		var party = new Party();
		party.Members.Add(new PartyMember());

		var partyStorage = party.WriteStorage();

		var newParty = new Party();
		newParty.ReadStorage(partyStorage);
		Game.Logger.Debug($"New party member: {newParty.Members[0]}");
	}

	private void GenerateMap(Cartographer cartographer)
	{
		var mapDimensions = new Vector2Int(384, 384);
		Game.Logger.Debug($"Generating overworld {mapDimensions.X}x{mapDimensions.Y}!");

		var stopwatch = new Stopwatch();
		stopwatch.Start();

		var map = cartographer.Generate(new CartographyOptions
		{
			MapDimensions = mapDimensions,
			Seed = new Random().Next(int.MaxValue)
			//Seed = 1025
		});

		stopwatch.Stop();

		Game.Logger.Debug($"Finished generating overworld! ({Math.Round(stopwatch.Elapsed.TotalMilliseconds)}ms)");

		var colorQuery = new QueryMapPixelColorEvent
		{
			Sender = null
		};

		var colorMap = new Color[mapDimensions.X * mapDimensions.Y];

		for (int y = 0; y < mapDimensions.Y; ++y)
		{
			for (int x = 0; x < mapDimensions.X; ++x)
			{
				var cell = map.Cells[x, y];

				cell.PropagateEvent(colorQuery);

				colorMap[y * mapDimensions.X + x] = colorQuery.MapPixelColor;
			}
		}

		var mapTexture = new Texture2D(Game.GraphicsDevice, mapDimensions.X, mapDimensions.Y);
		mapTexture.SetData(colorMap);

		mapTexture.SaveAsPng(
			new FileInfo($"map-{DateTime.Now:yyyy-MM-dd-hh:mm:ss}.png").Open(FileMode.OpenOrCreate),
			mapDimensions.X,
			mapDimensions.Y);

		_mapTexture = mapTexture;

		Game.Coroutines.EnqueueCoroutine(AddMapTextureCoroutine());
	}

	private IEnumerator AddMapTextureCoroutine()
	{
		yield return new WaitForSeconds(1.0f);

		if (_mapTexture != null && Game.Scene != null)
		{
			Game.Scene.Root.Add(new Image
			{
				Texture = _mapTexture,
				Bounds = new Rectangle(0, 0, 180, 180),
				Anchor = Anchor.TopRight
			});
		}
	}

	private Texture2D? _mapTexture = null;

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
