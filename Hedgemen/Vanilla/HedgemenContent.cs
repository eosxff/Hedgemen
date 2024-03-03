using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Hgm.Game.Campaigning;
using Hgm.Game.CellComponents;
using Hgm.Vanilla.WorldGeneration;
using Hgm.Game.WorldGeneration;
using Hgm.Vanilla.EntityComponents;
using Microsoft.Xna.Framework;
using Petal.Framework;
using Petal.Framework.Assets;
using Petal.Framework.Content;
using Petal.Framework.EC;
using Petal.Framework.IO;
using Petal.Framework.Persistence;
using Petal.Framework.Util;

namespace Hgm.Vanilla;

public sealed class HedgemenContent
{
	public RegistryObject<Supplier<EntityComponent>> CharacterSheet
	{
		get;
		private set;
	}

	public RegistryObject<Supplier<EntityComponent>> CharacterRace
	{
		get;
		private set;
	}

	public RegistryObject<Supplier<IGenerationPass>> OverworldTerrainGenerationPass
	{
		get;
		private set;
	}

	public RegistryObject<Supplier<IGenerationPass>> OverworldBiomeGenerationPass
	{
		get;
		private set;
	}

	public RegistryObject<Supplier<CellComponent>> OverworldDeepWater
	{
		get;
		private set;
	}

	public RegistryObject<Supplier<CellComponent>> OverworldShallowWater
	{
		get;
		private set;
	}

	public RegistryObject<Supplier<CellComponent>> PerlinGeneration
	{
		get;
		private set;
	}

	public RegistryObject<Supplier<CellComponent>> OverworldLand
	{
		get;
		private set;
	}

	public RegistryObject<Supplier<CellComponent>> OverworldMountain
	{
		get;
		private set;
	}

	public RegistryObject<Supplier<CellComponent>> OverworldMountainPeak
	{
		get;
		private set;
	}

	public RegistryObject<Supplier<CellComponent>> OverworldTundra
	{
		get;
		private set;
	}

	public RegistryObject<Supplier<CellComponent>> OverworldTaiga
	{
		get;
		private set;
	}

	public RegistryObject<Supplier<CellComponent>> OverworldTemperateRainforest
	{
		get;
		private set;
	}

	public RegistryObject<Supplier<CellComponent>> OverworldDesert
	{
		get;
		private set;
	}

	public RegistryObject<Supplier<CellComponent>> OverworldForest
	{
		get;
		private set;
	}

	public RegistryObject<Supplier<CellComponent>> OverworldShrubland
	{
		get;
		private set;
	}

	public RegistryObject<Supplier<CellComponent>> OverworldTropicalRainforest
	{
		get;
		private set;
	}

	public RegistryObject<Supplier<CellComponent>> OverworldGrassland
	{
		get;
		private set;
	}

	public RegistryObject<Cartographer> OverworldCartographer
	{
		get;
		private set;
	}

	public RegistryObject<Supplier<Campaign>> HedgemenCampaign
	{
		get;
		private set;
	}

	/// <summary>
	/// Loads all the content in vanilla Hedgemen in this order: assets > entity components > cell components >
	/// landscapers > cartographers.
	/// </summary>
	/// <param name="registers">the vanilla registers.</param>
	public void Setup(HedgemenRegisters registers)
	{
		RegisterAssets(registers);
		RegisterEntityComponents(registers);
		RegisterCellComponents(registers);
		RegisterGenerationPasses(registers);
		RegisterCartographers(registers);
	}

	private static void RegisterAssets(HedgemenRegisters registers)
	{
		var assetLoader = Hedgemen.InstanceOrThrow.Assets;

		var file = new FileInfo("asset_manifest.json");

		var manifestStorage = JsonSerializer.Deserialize(
			file.ReadString(Encoding.UTF8),
			PersistentData.JsonTypeInfo);

		var manifest = new AssetManifest(manifestStorage);
		manifest.ForwardToRegister(registers.Assets, assetLoader);
	}

	private void RegisterEntityComponents(HedgemenRegisters registers)
	{
		var register = registers.EntityComponents;

		var characterSheetName = new NamespacedString("hgm:character_sheet");
		var characterRaceName = new NamespacedString("hgm:character_race");

		register.AddKey(characterSheetName, () => new CharacterSheet());
		register.AddKey(characterRaceName, () => new CharacterRace());

		CharacterSheet = register.MakeReference(characterSheetName);
		CharacterRace = register.MakeReference(characterRaceName);
	}

	private void RegisterCellComponents(HedgemenRegisters registers)
	{
		var register = registers.CellComponents;

		var perlinGenerationName = new NamespacedString("hgm:perlin_generation");
		var overworldDeepWaterName = new NamespacedString("hgm:overworld_deep_water");
		var overworldShallowWaterName = new NamespacedString("hgm:overworld_shallow_water");
		var overworldLandName = new NamespacedString("hgm:overworld_land");
		var overworldMountainName = new NamespacedString("hgm:overworld_mountain");
		var overworldMountainPeakName = new NamespacedString("hgm:overworld_mountain_peak");

		var overworldTundraName = new NamespacedString("hgm:overworld_tundra");
		var overworldTaigaName = new NamespacedString("hgm:overworld_taiga");
		var overworldTemperateRainforestName = new NamespacedString("hgm:overworld_temperate_rainforest");
		var overworldTropicalRainforestName = new NamespacedString("hgm:overworld_tropical_rainforest");
		var overworldDesert = new NamespacedString("hgm:overworld_desert");
		var overworldForest = new NamespacedString("hgm:overworld_forest");
		var overworldShrubland = new NamespacedString("hgm:overworld_shrubland");
		var overworldGrasslandName = new NamespacedString("hgm:overworld_grassland");

		register.AddKey(perlinGenerationName, () => new PerlinGeneration());
		register.AddKey(overworldDeepWaterName, () => new OverworldDeepWater());
		register.AddKey(overworldShallowWaterName,() => new OverworldShallowWater());
		register.AddKey(overworldLandName, () => new OverworldLand());
		register.AddKey(overworldMountainName, () => new OverworldMountain());
		register.AddKey(overworldMountainPeakName, () => new OverworldMountainPeak());

		register.AddKey(overworldTundraName, () => new OverworldTundra
		{
			Details = WorldGeneration.OverworldTundra.BiomeDetails
		});

		register.AddKey(overworldTaigaName, () => new OverworldTaiga
		{
			Details = WorldGeneration.OverworldTaiga.BiomeDetails
		});

		register.AddKey(overworldTaigaName, () => new OverworldTemperateRainforest
		{
			Details = WorldGeneration.OverworldTemperateRainforest.BiomeDetails
		});

		register.AddKey(overworldTaigaName, () => new OverworldDesert
		{
			Details = WorldGeneration.OverworldDesert.BiomeDetails
		});

		register.AddKey(overworldTaigaName, () => new OverworldForest
		{
			Details = WorldGeneration.OverworldForest.BiomeDetails
		});

		register.AddKey(overworldTaigaName, () => new OverworldShrubland
		{
			Details = WorldGeneration.OverworldShrubland.BiomeDetails
		});

		register.AddKey(overworldGrasslandName, () => new OverworldGrassland
		{
			Details = WorldGeneration.OverworldGrassland.BiomeDetails
		});

		register.AddKey(overworldGrasslandName, () => new OverworldTropicalRainforest
		{
			Details = WorldGeneration.OverworldTropicalRainforest.BiomeDetails
		});

		PerlinGeneration = register.MakeReference(perlinGenerationName);

		OverworldDeepWater = register.MakeReference(overworldDeepWaterName);
		OverworldShallowWater = register.MakeReference(overworldShallowWaterName);
		OverworldLand = register.MakeReference(overworldLandName);
		OverworldMountain = register.MakeReference(overworldMountainName);
		OverworldMountainPeak = register.MakeReference(overworldMountainPeakName);

		OverworldTundra = register.MakeReference(overworldTundraName);
		OverworldTaiga = register.MakeReference(overworldTaigaName);
		OverworldForest = register.MakeReference(overworldForest);
		OverworldDesert = register.MakeReference(overworldDesert);
		OverworldShrubland = register.MakeReference(overworldShrubland);
		OverworldTemperateRainforest = register.MakeReference(overworldTemperateRainforestName);
		OverworldTropicalRainforest = register.MakeReference(overworldTropicalRainforestName);
		OverworldGrassland = register.MakeReference(overworldGrasslandName);
	}

	private void RegisterGenerationPasses(HedgemenRegisters registers)
	{
		var register = registers.GenerationPasses;

		var overworldTerrainGenerationPassName = new NamespacedString("hgm:overworld_terrain");
		var overworldBiomeGenerationPassName = new NamespacedString("hgm:overworld_biome");

		register.AddKey(overworldTerrainGenerationPassName, () => new OverworldTerrainGenerationPass
		{
			/*DeepWater = OverworldDeepWater.Get(),
			DeepWaterHeight = 0.5f,

			ShallowWater = OverworldShallowWater.Get(),
			ShallowWaterHeight = 0.58f,

			Land = OverworldLand.Get(),
			LandHeight = 0.75f,

			Mountain = OverworldMountain.Get(),
			MountainHeight = 0.85f,

			TallMountain = OverworldTallMountain.Get(),
			TallMountainHeight = 1.0f,*/

			DeepWater = OverworldDeepWater.Get(),
			DeepWaterHeight = 0.45f,

			ShallowWater = OverworldShallowWater.Get(),
			ShallowWaterHeight = 0.53f,

			Land = OverworldLand.Get(),
			LandHeight = 0.7f,

			Mountain = OverworldMountain.Get(),
			MountainHeight = 0.8f,

			TallMountain = OverworldMountainPeak.Get(),
			TallMountainHeight = 1.0f,
		});

		register.AddKey(overworldBiomeGenerationPassName, () => new OverworldBiomeGenerationPass
		{
			Biomes =
			[
				WorldGeneration.OverworldDesert.BiomeDetails,
				WorldGeneration.OverworldTundra.BiomeDetails,
				WorldGeneration.OverworldTaiga.BiomeDetails,
				WorldGeneration.OverworldForest.BiomeDetails,
				WorldGeneration.OverworldShrubland.BiomeDetails,
				WorldGeneration.OverworldTropicalRainforest.BiomeDetails,
				WorldGeneration.OverworldTemperateRainforest.BiomeDetails
			],
			DefaultBiome = WorldGeneration.OverworldGrassland.BiomeDetails
		});

		OverworldTerrainGenerationPass = register.MakeReference(overworldTerrainGenerationPassName);
		OverworldBiomeGenerationPass = register.MakeReference(overworldBiomeGenerationPassName);
	}

	private void RegisterCartographers(HedgemenRegisters registers)
	{
		var register = registers.Cartographers;

		var overworldCartographerName = new NamespacedString("hgm:overworld");

		var overworld = new Cartographer
		{
			NoiseGenerationArgs = new NoiseArgs
			{
				Seed = new Random().Next(int.MinValue, int.MaxValue),
				Dimensions = new Vector2Int(512, 512),
				Scale = 50.0f,
				Octaves = 5,
				Frequency = 2.0f,
				Lacunarity = 2.65f, /*2.75f,*/
				Offset = new Vector2Int(0, 0),
				FalloffModifier = 0.0f
			},
			GenerationPasses = { OverworldTerrainGenerationPass.Get(), OverworldBiomeGenerationPass.Get() }
		};

		register.AddKey(overworldCartographerName, overworld);

		OverworldCartographer = register.MakeReference(overworldCartographerName);
	}
}
