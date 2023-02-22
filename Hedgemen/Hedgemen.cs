using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework;
using Petal.Framework.Input;
using Petal.Framework.Scenery;
using Petal.Framework.Scenery.Nodes;
using Petal.Framework.Windowing;

namespace Hgm;

public class Hedgemen : PetalGame
{
	public static Hedgemen Instance
	{
		get;
		private set;
	}

	public Hedgemen()
	{
		Instance = this;
	}

	protected override void Initialize()
	{
		base.Initialize();
		
		var scene = new Scene(new Stage
		{
			Tag = "root",
			Name = "hedgemen:ui/root_node",
			VirtualResolution = new Vector2Int(640, 360)
		});
		
		var texture = Assets.LoadAsset<Texture2D>(new FileInfo("peach.png").Open(FileMode.Open));
		
		scene.Root.Add(new Image
		{
			Bounds = new Rectangle(0, 0, 50, 50),
			Texture = texture,
			Name = "hedgemen:image_1",
			Color = Color.Red
		});
		
		var image2 = scene.Root.Add(new Image
		{
			Bounds = new Rectangle(25, 25, 50, 50),
			Texture = texture,
			Name = "hedgemen:image_2",
			Color = Color.Green
		});
		
		var image3 = image2.Add(new Image
		{
			Bounds = new Rectangle(50, 50, 50, 50),
			Texture = texture,
			Name = "hedgemen:image_3",
			Color = Color.Blue
		});

		scene.AfterUpdate += (sender, args) =>
		{
			if (sender is Scene self)
			{
				if (self.Input.MouseButtonClicked(MouseButtons.LeftButton))
				{
					var selection = new NodeSelection();
					self.Root.ScanForTargetNode(selection);
					selection.Target?.Destroy();
				}
			}
		};

		ChangeScenes(scene);
	}

	protected override GameSettings GetInitialGameSettings()
	{
		return new GameSettings
		{
			PreferredFramerate = 60,
			Vsync = false,
			WindowWidth = 1200,
			WindowHeight = 700,
			WindowMode = WindowMode.Windowed,
			IsMouseVisible = true
		};
	}

	protected override void OnExiting(object sender, EventArgs args)
	{
		base.OnExiting(sender, args);
	}
}