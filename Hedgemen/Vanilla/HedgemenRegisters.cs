using Hgm.Components;
using Petal.Framework;
using Petal.Framework.Assets;
using Petal.Framework.Content;
using Petal.Framework.EC;
using Petal.Framework.Util;

namespace Hgm.Vanilla;

/// <summary>
/// Registers only non-null when <see cref="HedgemenVanilla"/> is finished <see cref="HedgemenVanilla.Setup"/>
/// </summary>
public sealed class HedgemenRegisters
{
	private Register<object>? _assets;

	/// <summary>
	/// Game art assets register.
	/// </summary>
	public Register<object> Assets
	{
		get
		{
			PetalExceptions.ThrowIfNull(_assets);
			return _assets;
		}
	}

	private Register<ContentSupplier<EntityComponent>>? _entityComponents;

	/// <summary>
	/// Game entity component register.
	/// </summary>
	public Register<ContentSupplier<EntityComponent>> EntityComponents
	{
		get
		{
			PetalExceptions.ThrowIfNull(_entityComponents);
			return _entityComponents;
		}
	}

	private Register<ContentSupplier<CellComponent>>? _cellComponents;

	/// <summary>
	/// Game cell component register.
	/// </summary>
	public Register<ContentSupplier<CellComponent>> CellComponents
	{
		get
		{
			PetalExceptions.ThrowIfNull(_cellComponents);
			return _cellComponents;
		}
	}

	/// <summary>
	/// Initializes the registers to (hopefully) non-null values.
	/// </summary>
	/// <param name="registry">the registry to be registered to</param>
	public void SetupRegisters(Registry registry)
	{
		SetupAssetsRegister(registry);
		SetupEntityComponentsRegister(registry);
		SetupCellComponentsRegister(registry);
	}

	private void SetupAssetsRegister(Registry registry)
	{
		var assetLoader = Hedgemen.Instance.Assets;
		SetRegister("hgm:assets", registry, ref _assets);

		var assetManifest = AssetManifest.FromFile("asset_manifest.json");
		assetManifest.ForwardToRegister(Assets, assetLoader);
	}

	private void SetupEntityComponentsRegister(Registry registry)
	{
		SetRegister("hgm:entity_components", registry, ref _entityComponents);

		EntityComponents.AddKey("hgm:character_sheet", () => new CharacterSheet());
		EntityComponents.AddKey("hgm:character_race", () => new CharacterRace());
	}

	private void SetupCellComponentsRegister(Registry registry)
	{
		SetRegister("hgm:cell_components", registry, ref _cellComponents);
	}

	private void SetRegister<TRegisterContent>(
		NamespacedString registerName,
		Registry registry,
		ref Register<TRegisterContent>? register)
	{
		bool found = registry.GetRegister(registerName, out Register<TRegisterContent> registerInRegistry);

		if (!found)
		{
			registry.Logger.Error($"Could not find '{registerName}' in registry.");
			return;
		}

		register = registerInRegistry;
	}
}
