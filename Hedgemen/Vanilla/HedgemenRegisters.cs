﻿using System;
using Hgm.Game;
using Hgm.Game.Campaigning;
using Hgm.Game.WorldGeneration;
using Petal.Framework;
using Petal.Framework.Content;
using Petal.Framework.EC;
using Petal.Framework.Util;

namespace Hgm.Vanilla;

/// <summary>
/// Registers only non-null when <see cref="HedgemenVanilla"/> is finished <see cref="HedgemenVanilla.Setup"/>.
/// Attempting to access these registers before they are initialized will result in an exception thrown.
/// </summary>
public sealed class HedgemenRegisters
{
	public static readonly NamespacedString AssetsRegisterName = new("hgm:assets");
	public static readonly NamespacedString EntityComponentsRegisterName = new("hgm:entity_components");
	public static readonly NamespacedString CellComponentsRegisterName = new("hgm:cell_components");
	public static readonly NamespacedString GenerationPassesRegisterName = new("hgm:generation_passes");
	public static readonly NamespacedString CartographersRegisterName = new("hgm:cartographers");
	public static readonly NamespacedString CampaignsRegisterName = new("hgm:campaigns");


	private Register<object>? _assets;

	public Register<object> Assets
	{
		get
		{
			PetalExceptions.ThrowIfNull(_assets);
			return _assets;
		}
	}

	private Register<Supplier<EntityComponent>>? _entityComponents;

	public Register<Supplier<EntityComponent>> EntityComponents
	{
		get
		{
			PetalExceptions.ThrowIfNull(_entityComponents);
			return _entityComponents;
		}
	}

	private Register<Supplier<CellComponent>>? _cellComponents;

	public Register<Supplier<CellComponent>> CellComponents
	{
		get
		{
			PetalExceptions.ThrowIfNull(_cellComponents);
			return _cellComponents;
		}
	}

	private Register<Supplier<IGenerationPass>>? _generationPasses;

	public Register<Supplier<IGenerationPass>> GenerationPasses
	{
		get
		{
			PetalExceptions.ThrowIfNull(_generationPasses);
			return _generationPasses;
		}
	}

	private Register<Cartographer>? _cartographers;

	public Register<Cartographer> Cartographers
	{
		get
		{
			PetalExceptions.ThrowIfNull(_cartographers);
			return _cartographers;
		}
	}

	private Register<Supplier<Campaign>> _campaigns;

	public Register<Supplier<Campaign>> Campaigns
	{
		get
		{
			PetalExceptions.ThrowIfNull(_campaigns);
			return _campaigns;
		}
	}


	/// <summary>
	/// Initializes the registers to non-null values. Note that this does not add the content.
	/// </summary>
	/// <param name="registry">the registry to be registered to</param>
	public void SetupRegisters(Registry registry)
	{
		SetupAssetsRegister(registry);
		SetupEntityComponentsRegister(registry);
		SetupCellComponentsRegister(registry);
		SetupGenerationPassesRegister(registry);
		SetupCartographersRegister(registry);
		SetupCampaignsRegister(registry);
	}

	private void SetupAssetsRegister(Registry registry)
	{
		_assets = new Register<object>(
			AssetsRegisterName,
			HedgemenVanilla.ModID,
			registry);

		AddRegisterToRegistry(_assets, registry);
	}

	private void SetupEntityComponentsRegister(Registry registry)
	{
		_entityComponents = new Register<Supplier<EntityComponent>>(
			EntityComponentsRegisterName,
			HedgemenVanilla.ModID,
			registry);

		AddRegisterToRegistry(_entityComponents, registry);
	}

	private void SetupCellComponentsRegister(Registry registry)
	{
		_cellComponents = new Register<Supplier<CellComponent>>(
			CellComponentsRegisterName,
			HedgemenVanilla.ModID,
			registry);

		AddRegisterToRegistry(_cellComponents, registry);
	}

	private void SetupGenerationPassesRegister(Registry registry)
	{
		_generationPasses = new Register<Supplier<IGenerationPass>>(
			GenerationPassesRegisterName,
			HedgemenVanilla.ModID,
			registry);

		AddRegisterToRegistry(_generationPasses, registry);
	}

	private void SetupCartographersRegister(Registry registry)
	{
		_cartographers = new Register<Cartographer>(
			CartographersRegisterName,
			HedgemenVanilla.ModID,
			registry);

		AddRegisterToRegistry(_cartographers, registry);
	}

	private void SetupCampaignsRegister(Registry registry)
	{
		_campaigns = new Register<Supplier<Campaign>>(
			CampaignsRegisterName,
			HedgemenVanilla.ModID,
			registry);

		AddRegisterToRegistry(_campaigns, registry);
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
		{
			string message = $"'{register.RegistryName}' could not be added to {nameof(Registry)}";
			throw new InvalidOperationException(message);
		}
	}
}
