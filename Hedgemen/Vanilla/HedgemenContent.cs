using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Hgm.Game;
using Hgm.Components;
using Hgm.Vanilla.WorldGeneration;
using Hgm.Game.WorldGeneration;
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

	public RegistryObject<Supplier<ILandscaper>> OverworldTerrainLandscaper
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

	public RegistryObject<Cartographer> OverworldCartographer
	{
		get;
		private set;
	}

	public RegistryObject<CampaignCreator> HedgemenCampaignCreator
	{
		get;
		private set;
	}

	public RegistryObject<Supplier<ICampaignBehaviour>> HedgemenCampaignBehaviour
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
		RegisterLandscapers(registers);
		RegisterCartographers(registers);
		RegisterCampaignBehaviours(registers);
		RegisterCampaignCreators(registers);
	}

	private void RegisterAssets(HedgemenRegisters registers)
	{
		var assetLoader = Hedgemen.Instance.Assets;

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

		register.AddKey(perlinGenerationName, () => new PerlinGeneration());
		register.AddKey(overworldDeepWaterName, () => new OverworldDeepWater());
		register.AddKey(overworldShallowWaterName,() => new OverworldShallowWater());
		register.AddKey(overworldLandName, () => new OverworldLand());
		register.AddKey(overworldMountainName, () => new OverworldMountain());
		register.AddKey(overworldTallMountainName, () => new OverworldTallMountain());

		PerlinGeneration = register.MakeReference(perlinGenerationName);
		OverworldDeepWater = register.MakeReference(overworldDeepWaterName);
		OverworldShallowWater = register.MakeReference(overworldShallowWaterName);
		OverworldLand = register.MakeReference(overworldLandName);
		OverworldMountain = register.MakeReference(overworldMountainName);
		OverworldTallMountain = register.MakeReference(overworldTallMountainName);
	}

	private void RegisterLandscapers(HedgemenRegisters registers)
	{
		var register = registers.Landscapers;

		var overworldTerrainLandscaperName = new NamespacedString("hgm:overworld_terrain_landscaper");

		register.AddKey(overworldTerrainLandscaperName, () => new OverworldTerrainLandscaper
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

			Scale = 50.0f,
			Octaves = 5,
			Frequency = 2.5f,
			Lacunarity = 2.75f,
			Offset = new Vector2Int(0, 0),
			FalloffModifier = 0.0f
		});

		OverworldTerrainLandscaper = register.MakeReference(overworldTerrainLandscaperName);
	}

	private void RegisterCartographers(HedgemenRegisters registers)
	{
		var register = registers.Cartographers;

		var overworldCartographerName = new NamespacedString("hgm:overworld_cartographer");

		var overworld = new Cartographer();
		overworld.Landscapers.Add(OverworldTerrainLandscaper);
		register.AddKey(overworldCartographerName, overworld);

		OverworldCartographer = register.MakeReference("hgm:overworld_cartographer");
	}

	private void RegisterCampaignCreators(HedgemenRegisters registers)
	{
		var register = registers.CampaignCreatorCreators;

		var hedgemenCampaignCreatorName = new NamespacedString("hgm:hedgemen_campaign_creator");
		var hedgemenCampaignBehaviourName = new NamespacedString("hgm:campaign_behaviour");

		register.AddKey(hedgemenCampaignCreatorName, new CampaignCreator
		{
			CampaignBehaviourName = hedgemenCampaignBehaviourName
		});

		HedgemenCampaignCreator = register.MakeReference(hedgemenCampaignCreatorName);
	}

	private void RegisterCampaignBehaviours(HedgemenRegisters registers)
	{
		var register = registers.CampaignBehaviours;

		var hedgemenCampaignBehaviourName = new NamespacedString("hgm:campaign_behaviour");

		register.AddKey(hedgemenCampaignBehaviourName, () => new HedgemenCampaignBehaviour());

		HedgemenCampaignBehaviour = register.MakeReference(hedgemenCampaignBehaviourName);
	}
}
