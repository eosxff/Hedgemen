﻿using System;
using Hgm.WorldGeneration;
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
	public static readonly NamespacedString LandscapersRegisterName = new("hgm:landscapers");
	public static readonly NamespacedString CartographersRegisterName = new("hgm:cartographers");

	private Register<object>? _assets;

	public Register<object> Assets
	{
		get
		{
			PetalExceptions.ThrowIfNull(_assets);
			return _assets;
		}
	}

	private Register<ContentSupplier<EntityComponent>>? _entityComponents;

	public Register<ContentSupplier<EntityComponent>> EntityComponents
	{
		get
		{
			PetalExceptions.ThrowIfNull(_entityComponents);
			return _entityComponents;
		}
	}

	private Register<ContentSupplier<CellComponent>>? _cellComponents;

	public Register<ContentSupplier<CellComponent>> CellComponents
	{
		get
		{
			PetalExceptions.ThrowIfNull(_cellComponents);
			return _cellComponents;
		}
	}

	private Register<ContentSupplier<ILandscaper>>? _landscapers;

	public Register<ContentSupplier<ILandscaper>> Landscapers
	{
		get
		{
			PetalExceptions.ThrowIfNull(_landscapers);
			return _landscapers;
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

	/// <summary>
	/// Initializes the registers to non-null values. Note that this does not add the content.
	/// </summary>
	/// <param name="registry">the registry to be registered to</param>
	public void SetupRegisters(Registry registry)
	{
		SetupAssetsRegister(registry);
		SetupEntityComponentsRegister(registry);
		SetupCellComponentsRegister(registry);
		SetupLandscapersRegister(registry);
		SetupCartographersRegister(registry);
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
		_entityComponents = new Register<ContentSupplier<EntityComponent>>(
			EntityComponentsRegisterName,
			HedgemenVanilla.ModID,
			registry);

		AddRegisterToRegistry(_entityComponents, registry);
	}

	private void SetupCellComponentsRegister(Registry registry)
	{
		_cellComponents = new Register<ContentSupplier<CellComponent>>(
			CellComponentsRegisterName,
			HedgemenVanilla.ModID,
			registry);

		AddRegisterToRegistry(_cellComponents, registry);
	}

	private void SetupLandscapersRegister(Registry registry)
	{
		_landscapers = new Register<ContentSupplier<ILandscaper>>(
			LandscapersRegisterName,
			HedgemenVanilla.ModID,
			registry);

		AddRegisterToRegistry(_landscapers, registry);
	}

	private void SetupCartographersRegister(Registry registry)
	{
		_cartographers = new Register<Cartographer>(
			CartographersRegisterName,
			HedgemenVanilla.ModID,
			registry);

		AddRegisterToRegistry(_cartographers, registry);
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
