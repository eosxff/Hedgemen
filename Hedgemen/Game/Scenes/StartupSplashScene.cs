using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework;
using Petal.Framework.Graphics;
using Petal.Framework.Scenery;
using Petal.Framework.Scenery.Nodes;
using Petal.Framework.Util;

namespace Hgm.Game.Scenes;

public sealed class StartupSplashScene : Scene
{
	private Texture2D _splashBackground;

	public StartupSplashScene(Stage root, Skin skin, Stream backgroundStream, PetalGame game = null)
		: base(root, skin, game)
	{
		Name = "hgm:startup_splash_scene".ToNamespaced();
		BackgroundColor = Color.Black;
		ViewportAdapter = new BoxingViewportAdapter(
			Game.GraphicsDevice,
			Game.Window,
			new Vector2Int(320, 180));

		_splashBackground = Game.Assets.LoadAsset<Texture2D>(backgroundStream);
	}
	protected override void OnInitialize()
	{
		Root.Add(new Image
		{
			Texture = _splashBackground,
			Anchor = Anchor.TopRight,
			Color = Color.White,
			Bounds = ViewportAdapter.VirtualResolution.ToRectangleSize()
		});
	}
}
