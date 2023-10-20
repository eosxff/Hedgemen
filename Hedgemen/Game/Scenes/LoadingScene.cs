using Microsoft.Xna.Framework;
using Petal.Framework;
using Petal.Framework.Content;
using Petal.Framework.Graphics;
using Petal.Framework.Scenery;

namespace Hgm.Game.Scenes;

public sealed class LoadingScene : Scene
{
	public Register<object> Assets
	{
		get;
	}

	public LoadingScene(Register<object> assets)
	{
		Assets = assets;

		BackgroundColor = Color.Black;
		Name = new NamespacedString("hgm:loading_scene");
		ViewportAdapter = new BoxingViewportAdapter(
			Game.GraphicsDevice,
			Game.Window,
			new Vector2Int(320, 180));
	}

	protected override void OnLoad()
	{

	}
}
