using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Hgm.Components;
using Hgm.Vanilla.WorldGeneration;
using Hgm.WorldGeneration;
using Petal.Framework.Assets;
using Petal.Framework.Content;
using Petal.Framework.IO;
using Petal.Framework.Persistence;

namespace Hgm.Vanilla;

public sealed class HedgemenContent
{
	public RegistryObject<ContentSupplier<CharacterSheet>> CharacterSheet
	{
		get;
		private set;
	}

	public RegistryObject<ContentSupplier<CharacterRace>> CharacterRace
	{
		get;
		private set;
	}

	public RegistryObject<ContentSupplier<ILandscaper>> OverworldTerrainLandscaper
	{
		get;
		private set;
	}

	public RegistryObject<Cartographer> OverworldCartographer
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
	}

	private void RegisterAssets(HedgemenRegisters registers)
	{
		var assetLoader = Hedgemen.Instance.Assets;

		var file = new FileInfo("asset_manifest.json");

		var manifestStorage = JsonSerializer.Deserialize(
			file.ReadString(Encoding.UTF8),
			DataStorageJsonTypeInfo.Default.DataStorage);

		var manifest = new AssetManifest(manifestStorage);
		manifest.ForwardToRegister(registers.Assets, assetLoader);
	}

	private void RegisterEntityComponents(HedgemenRegisters registers)
	{
		var register = registers.EntityComponents;

		register.AddKey("hgm:character_sheet", () => new CharacterSheet());
		register.AddKey("hgm:character_race", () => new CharacterRace());

		CharacterSheet = register.MakeDerivedReference<ContentSupplier<CharacterSheet>>("hgm:character_sheet");
		CharacterRace = register.MakeDerivedReference<ContentSupplier<CharacterRace>>("hgm:character_race");
	}

	private void RegisterCellComponents(HedgemenRegisters registers)
	{
		var register = registers.CellComponents;

		register.AddKey("hgm:perlin_generation", () => new PerlinGeneration());
	}

	private void RegisterLandscapers(HedgemenRegisters registers)
	{
		var register = registers.Landscapers;

		register.AddKey("hgm:overworld_terrain_landscaper", () => new OverworldTerrainLandscaper());

		OverworldTerrainLandscaper = register.MakeReference("hgm:overworld_terrain_landscaper");
	}

	private void RegisterCartographers(HedgemenRegisters registers)
	{
		var register = registers.Cartographers;

		var overworld = new Cartographer();
		overworld.Landscapers.Add(OverworldTerrainLandscaper);
		register.AddKey("hgm:overworld_cartographer", overworld);

		OverworldCartographer = register.MakeReference("hgm:overworld_cartographer");
	}
}
