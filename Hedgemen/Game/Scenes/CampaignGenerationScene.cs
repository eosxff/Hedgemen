using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Hgm.Game.Systems;
using Hgm.Vanilla;
using Hgm.Vanilla.WorldGeneration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Petal.Framework;
using Petal.Framework.Graphics;
using Petal.Framework.IO;
using Petal.Framework.Scenery;
using Petal.Framework.Scenery.Nodes;
using Petal.Framework.Util;
using Petal.Framework.Util.Extensions;

namespace Hgm.Game.Scenes;

public sealed class CampaignGenerationScene : Scene
{
	public CampaignGenerator Generator
	{
		get;
	}

	public Canvas WorldGenerationCanvas
	{
		get;
		private set;
	}

	public Panel WorldGenerationPanel
	{
		get;
		private set;
	}

	public CampaignGenerationScene(CampaignGenerator generator)
	{
		var assets = HedgemenVanilla.Instance.Registers.Assets;

		Generator = generator;
		Skin = Skin.FromJson(new FileInfo("vanilla_skin.json").ReadString(Encoding.UTF8), assets);
		Name = new NamespacedString("hgm:campaign_scene");
		BackgroundColor = Color.Black;
		ViewportAdapter = new BoxingViewportAdapter(
			Game.GraphicsDevice,
			Game.Window,
			new Vector2Int(320*2, 180*2));

		BeforeUpdate += (sender, args) =>
		{
			if (Input.IsKeyPressed(Keys.Enter))
			{
				if (WorldGenerationCanvas is not null)
					WorldGenerationCanvas.IsMarkedForDeletion = true;

				WorldGenerationCanvas = CreateWorldGenerationCanvas();
				GenerateAndDisplayMap();
			}
		};

		AfterUpdate += (sender, args) =>
		{

		};
	}


	protected override void OnDispose()
	{
		WorldGenerationCanvas.Dispose();
	}

	protected override void OnLoad()
	{
		var assets = HedgemenVanilla.Instance.Registers.Assets;

		WorldGenerationPanel = Root.Add(new Panel(Skin)
		{
			Bounds = new Rectangle(0, 8, 32, 64),
			Anchor = Anchor.BottomLeft
		});

		WorldGenerationCanvas = CreateWorldGenerationCanvas();

		if (assets.GetItem("hgm:sprites/hedge_knight".ToNamespaced(), out Texture2D texture))
		{
			Root.Add(new Image
			{
				Texture = texture,
				Bounds = new Rectangle(0, 8, 32, 64),
				Anchor = Anchor.BottomLeft
			});
		}

		GenerateAndDisplayMap();
		GenerateMapCreatedPanel();
	}

	private Canvas CreateWorldGenerationCanvas()
	{
		WorldGenerationCanvas = Root.Add(new Canvas(
			Generator.StartingWorldCartographer.NoiseGenerationArgs.Dimensions, // todo maybe this sucks
			Renderer.RenderState.Graphics.GraphicsDevice)
		{
			Name = new NamespacedString("hgm:world_generation_canvas"),
			//Bounds = new Rectangle(0, 0, 180, 180),
			Bounds = new Rectangle(0, 0, 312, 312),
			Anchor = Anchor.Center,
			IsInteractable = false
		});

		WorldGenerationCanvas.ColorMap.Populate(() => Color.Black);
		WorldGenerationCanvas.ApplyColorMap();

		return WorldGenerationCanvas;
	}

	private async void GenerateAndDisplayMap()
	{
		var assets = HedgemenVanilla.Instance.Registers.Assets;

		var noiseArgs = Generator.StartingWorldCartographer.NoiseGenerationArgs;
		noiseArgs.Seed = new Random().Next(int.MinValue, int.MaxValue);
		Generator.StartingWorldCartographer.NoiseGenerationArgs = noiseArgs;

		var map = await Task.Run(
			() => WorldGenerationSystem.GenerateWorldMap(Generator.StartingWorldCartographer));

		var colorMap = WorldGenerationCanvas.ColorMap;
		var mapPixelColorQuery = new QueryMapPixelColorEvent();

		colorMap.Iterate((_, i) =>
		{
			var mapCell = map.Cells[i.X, i.Y];

			if (mapCell.WillRespondToEvent<QueryMapPixelColorEvent>())
			{
				mapCell.PropagateEvent(mapPixelColorQuery);
				colorMap[i.X, i.Y] = mapPixelColorQuery.MapPixelColor;
			}
		});

		WorldGenerationCanvas.ApplyColorMap();
		SaveGeneratedMapToFile(colorMap);
	}

	private void SaveGeneratedMapToFile(Map<Color> colorMap)
	{
		var mapFile = new FileInfo($"map-{DateTime.Now:yyyy-MM-dd_hh-mm-ss}.png");

		if(mapFile.Exists)
			mapFile.Delete();

		var mapTexture = new Texture2D(Game.GraphicsDevice, colorMap.Width, colorMap.Height);
		mapTexture.SetData(colorMap.ToArray());

		mapTexture.SaveAsPng(
			mapFile.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite),
			colorMap.Width,
			colorMap.Height);
	}

	private void GenerateMapCreatedPanel()
	{

	}
}
