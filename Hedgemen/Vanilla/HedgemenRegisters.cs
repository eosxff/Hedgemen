using System;
using Hgm.Game;
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
	public static readonly NamespacedString LandscapersRegisterName = new("hgm:landscapers");
	public static readonly NamespacedString CartographersRegisterName = new("hgm:cartographers");
	public static readonly NamespacedString CampaignCreatorRegisterName = new("hgm:campaign_creators");
	public static readonly NamespacedString CampaignBehaviourRegisterName = new("hgm:campaign_behaviours");

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

	private Register<Supplier<ILandscaper>>? _landscapers;

	public Register<Supplier<ILandscaper>> Landscapers
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

	private Register<CampaignCreator> _campaignCreators;

	public Register<CampaignCreator> CampaignCreatorCreators
	{
		get
		{
			PetalExceptions.ThrowIfNull(_campaignCreators);
			return _campaignCreators;
		}
	}

	private Register<Supplier<ICampaignBehaviour>> _campaignBehaviours;

	public Register<Supplier<ICampaignBehaviour>> CampaignBehaviours
	{
		get
		{
			PetalExceptions.ThrowIfNull(_campaignBehaviours);
			return _campaignBehaviours;
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
		SetupCampaignCreatorsRegister(registry);
		SetupCampaignBehavioursRegister(registry);
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

	private void SetupLandscapersRegister(Registry registry)
	{
		_landscapers = new Register<Supplier<ILandscaper>>(
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

	private void SetupCampaignCreatorsRegister(Registry registry)
	{
		_campaignCreators = new Register<CampaignCreator>(
			CampaignCreatorRegisterName,
			HedgemenVanilla.ModID,
			registry);

		AddRegisterToRegistry(_campaignCreators, registry);
	}

	private void SetupCampaignBehavioursRegister(Registry registry)
	{
		_campaignBehaviours = new Register<Supplier<ICampaignBehaviour>>(
			CampaignBehaviourRegisterName,
			HedgemenVanilla.ModID,
			registry);

		AddRegisterToRegistry(_campaignBehaviours, registry);
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
