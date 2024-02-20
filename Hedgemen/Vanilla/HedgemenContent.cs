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

	public RegistryObject<Supplier<CellComponent>> OverworldTallMountain
	{
		get;
		private set;
	}

	public RegistryObject<Supplier<CellComponent>> OverworldTundra
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
		var overworldTallMountainName = new NamespacedString("hgm:overworld_tall_mountain");

		var overworldTundraName = new NamespacedString("hgm:overworld_tundra");

		register.AddKey(perlinGenerationName, () => new PerlinGeneration());
		register.AddKey(overworldDeepWaterName, () => new OverworldDeepWater());
		register.AddKey(overworldShallowWaterName,() => new OverworldShallowWater());
		register.AddKey(overworldLandName, () => new OverworldLand());
		register.AddKey(overworldMountainName, () => new OverworldMountain());
		register.AddKey(overworldTallMountainName, () => new OverworldTallMountain());

		register.AddKey(overworldTundraName, () => new OverworldTundra
		{
			Details = WorldGeneration.OverworldTundra.BiomeDetails
		});

		PerlinGeneration = register.MakeReference(perlinGenerationName);

		OverworldDeepWater = register.MakeReference(overworldDeepWaterName);
		OverworldShallowWater = register.MakeReference(overworldShallowWaterName);
		OverworldLand = register.MakeReference(overworldLandName);
		OverworldMountain = register.MakeReference(overworldMountainName);
		OverworldTallMountain = register.MakeReference(overworldTallMountainName);

		OverworldTundra = register.MakeReference(overworldTundraName);
	}

	private void RegisterGenerationPasses(HedgemenRegisters registers)
	{
		var register = registers.GenerationPasses;

		var overworldTerrainGenerationPassName = new NamespacedString("hgm:overworld_terrain");
		var overworldBiomeGenerationPassName = new NamespacedString("hgm:overworld_biome");

		register.AddKey(overworldTerrainGenerationPassName, () => new OverworldTerrainGenerationPass
		{
			DeepWater = OverworldDeepWater.Get(),
			DeepWaterHeight = 0.5f,

			ShallowWater = OverworldShallowWater.Get(),
			ShallowWaterHeight = 0.58f,

			Land = OverworldLand.Get(),
			LandHeight = 0.75f,

			Mountain = OverworldMountain.Get(),
			MountainHeight = 0.85f,

			TallMountain = OverworldTallMountain.Get(),
			TallMountainHeight = 1.0f,
		});

		register.AddKey(overworldBiomeGenerationPassName, () => new OverworldBiomeGenerationPass
		{
			Biomes = [ WorldGeneration.OverworldTundra.BiomeDetails ],
			DefaultBiome = WorldGeneration.OverworldTundra.BiomeDetails
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
				Frequency = 2.5f,
				Lacunarity = 2.75f,
				Offset = new Vector2Int(0, 0),
				FalloffModifier = 0.0f
			},
			GenerationPasses = { OverworldTerrainGenerationPass.Get(), OverworldBiomeGenerationPass.Get() }
		};

		register.AddKey(overworldCartographerName, overworld);

		OverworldCartographer = register.MakeReference(overworldCartographerName);
	}
}
