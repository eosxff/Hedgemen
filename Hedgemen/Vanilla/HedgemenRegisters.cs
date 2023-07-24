using System;
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
	public static readonly NamespacedString AssetsRegisterName = new("hgm:assets");
	public static readonly NamespacedString EntityComponentsRegisterName = new("hgm:entity_components");
	public static readonly NamespacedString CellComponentsRegisterName = new("hgm:cell_components");

	private Register<object>? _assets;

	/// <summary>
	/// Game art assets register. Will throw an exception if the underlying field is null.
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
	/// Game entity component register. Will throw an exception if the underlying field is null.
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
	/// Game cell component register. Will throw an exception if the underlying field is null.
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
	/// Initializes the registers to non-null values.
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
		_assets = new Register<object>(
			AssetsRegisterName,
			HedgemenVanilla.ModID,
			registry);

		AddRegisterToRegistry(_assets, registry);

		var assetLoader = Hedgemen.Instance.Assets;

		var assetManifest = AssetManifest.FromFile("asset_manifest.json");
		assetManifest.ForwardToRegister(Assets, assetLoader);
	}

	private void SetupEntityComponentsRegister(Registry registry)
	{
		_entityComponents = new Register<ContentSupplier<EntityComponent>>(
			EntityComponentsRegisterName,
			HedgemenVanilla.ModID,
			registry);

		AddRegisterToRegistry(_entityComponents, registry);

		EntityComponents.AddKey("hgm:character_sheet", () => new CharacterSheet());
		EntityComponents.AddKey("hgm:character_race", () => new CharacterRace());
	}

	private void SetupCellComponentsRegister(Registry registry)
	{
		_cellComponents = new Register<ContentSupplier<CellComponent>>(
			CellComponentsRegisterName,
			HedgemenVanilla.ModID,
			registry);

		AddRegisterToRegistry(_cellComponents, registry);
	}

	/// <summary>
	/// Adds the <paramref name="register"/> to the <paramref name="registry"/>.
	/// If this operation fails, an exception will be thrown.
	/// </summary>
	/// <param name="register">the register.</param>
	/// <param name="registry">the register.</param>
	/// <exception cref="InvalidOperationException">if this operation failed. The logger should give details.
	/// </exception>
	private void AddRegisterToRegistry(
		IRegister register,
		Registry registry)
	{
		bool success = registry.AddRegister(register);

		if (!success)
			throw new InvalidOperationException(
				$"'{register.RegistryName}' could not be added to {nameof(Registry)}");
	}
}
