using System.IO;
using System.Text;
using Hgm.Vanilla;
using Microsoft.Xna.Framework;
using Petal.Framework;
using Petal.Framework.Graphics;
using Petal.Framework.IO;
using Petal.Framework.Scenery;

namespace Hgm.Game.Scenes;

public sealed class CampaignScene : Scene
{
	public CampaignScene()
	{
		var assets = HedgemenVanilla.Instance.Registers.Assets;

		Skin = Skin.FromJson(new FileInfo("vanilla_skin.json").ReadString(Encoding.UTF8), assets);
		Name = new NamespacedString("hgm:campaign_scene");
		BackgroundColor = Color.Black;
		ViewportAdapter = new BoxingViewportAdapter(
			Game.GraphicsDevice,
			Game.Window,
			new Vector2Int(320, 180));
	}
}
