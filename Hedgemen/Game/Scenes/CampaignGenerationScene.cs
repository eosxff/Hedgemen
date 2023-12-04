using System.IO;
using System.Text;
using System.Threading.Tasks;
using Hgm.Game.Systems;
using Hgm.Vanilla;
using Hgm.Vanilla.WorldGeneration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework;
using Petal.Framework.Graphics;
using Petal.Framework.IO;
using Petal.Framework.Scenery;
using Petal.Framework.Scenery.Nodes;

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
			new Vector2Int(320, 180));
	}


	protected override void OnDispose()
	{
		WorldGenerationCanvas.Dispose();
	}

	protected override void OnLoad()
	{
		WorldGenerationCanvas = Root.Add(new Canvas(
			Generator.StartingWorldCartographyOptions.MapDimensions,
			Renderer.RenderState.Graphics.GraphicsDevice)
		{
			Name = new NamespacedString("hgm:world_generation_canvas"),
			Bounds = new Rectangle(0, 0, 180, 180),
			Anchor = Anchor.Center,
			IsInteractable = false
		});

		WorldGenerationCanvas.ColorMap.Populate(() => Color.Black);
		WorldGenerationCanvas.ApplyColorMap();

		GenerateAndDisplayMap();
	}

	private async void GenerateAndDisplayMap()
	{
		var assets = HedgemenVanilla.Instance.Registers.Assets;
		var map = await Task.Run(
			() => Generator.StartingWorldCartographer.Generate(Generator.StartingWorldCartographyOptions));

		Root.Add(new Panel(Skin)
		{
			Bounds = new Rectangle(0, 8, 32, 64),
			Anchor = Anchor.BottomLeft
		});

		if (assets.GetItem("hgm:sprites/hedge_knight", out Texture2D texture))
		{
			Root.Add(new Image
			{
				Texture = texture,
				Bounds = new Rectangle(0, 8, 32, 64),
				Anchor = Anchor.BottomLeft
			});
		}

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
		GenerateMapCreatedPanel();
	}

	private void GenerateMapCreatedPanel()
	{

	}
}
