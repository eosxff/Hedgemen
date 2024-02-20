using System;
using System.IO;
using Hgm;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework.Content;
using Petal.Framework.Modding;

namespace Example;

public class ExampleMod : PetalMod
{
	public static Hedgemen Game
		=> Hedgemen.InstanceOrThrow;

	public RegistryObject<Texture2D> TestAsset = null;

	protected override void OnLoadedToPetalModLoader()
	{
		Game.Logger.Info($"Loaded {nameof(ExampleMod)}");
	}

	protected override void PrePetalModLoaderModSetupPhase(ModLoaderSetupContext context)
	{
		var assetLoader = Game.Assets;
		var assetsRegister = new DeferredRegister<object>("hgm:assets", Manifest.ModID, Game.Registry);

		assetsRegister.AddKey(
			"example:test_asset",
			assetLoader.LoadAsset<Texture2D>(new FileInfo("button_hover.png").Open(FileMode.Open)));

		assetsRegister.OnForwarded += OnForwardedAssetsRegister;
	}

	private void OnForwardedAssetsRegister(object sender, EventArgs args)
	{
		if (!Game.Registry.GetRegister("hgm:assets", out var assets))
			return;

		TestAsset = assets.MakeReference<Texture2D>("example:test_asset");

		Game.Logger.Info($"Test asset: {TestAsset.Location}. Is valid: {TestAsset.IsPresent}");
		Game.Logger.Info($"Finished forwarding assets register!");
	}
}
