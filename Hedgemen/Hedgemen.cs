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
			VirtualResolution = new Vector2Int(640, 360),
		})
		{
			BackgroundColor = Color.CornflowerBlue
		};

		var texture = Assets.LoadAsset<Texture2D>(new FileInfo("peach.png").Open(FileMode.Open));
		
		var image = scene.Root.Add(new Image
		{
			Bounds = new Rectangle(0, 0, 50, 50),
			Texture = texture,
			Name = "hedgemen:image_1",
			Color = Color.White,
			Anchor = Anchor.Center
		});

		image.OnFocusGained += node =>
		{
			if (node is Image imageNode)
			{
				imageNode.Color = Color.Red;
			}
		};
		
		image.OnFocusLost += node =>
		{
			if (node is Image imageNode)
			{
				imageNode.Color = Color.White;
			}
		};

		//image.OnMouseHover += node => Console.WriteLine("MouseHover");
		image.OnMouseDown += node => Console.WriteLine("MouseDown");
		image.OnMousePressed += node =>
		{
			Console.WriteLine("Click!");
			node.MarkDestroy();
		};
		image.OnMouseReleased += node => Console.WriteLine("MouseReleased");
		
		var image2 = image.Add(new Image
		{
			Bounds = new Rectangle(0, 0, 32, 32),
			Texture = texture,
			Name = "hedgemen:image_2",
			Color = Color.Green,
			Anchor = Anchor.Center
		});
		
		var image3 = image2.Add(new Image
		{
			Bounds = new Rectangle(8, 8, 16, 16),
			Texture = texture,
			Name = "hedgemen:image_3",
			Color = Color.Blue,
			Anchor = Anchor.BottomLeft
		});

		/*scene.AfterUpdate += (sender, args) =>
		{
			if (sender is Scene self)
			{
				if (self.Input.MouseButtonClicked(MouseButtons.LeftButton))
				{
					self.Root.SearchForTargetNode(out var selection);
					selection.Target?.Destroy();
				}

				if (self.Input.MouseButtonClicked(MouseButtons.RightButton))
				{
					self.FindNode("hedgemen:image_3")?.Destroy();
				}
			}
		};*/

		scene.BeforeUpdate += (sender, args) =>
		{
			if (ShouldResetAnchor())
			{
				image.Anchor = (Anchor)(_rng.Next(0, 9));
				image2.Anchor = (Anchor)(_rng.Next(0, 9));
				image3.Anchor = (Anchor)(_rng.Next(0, 9));
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

	private int _frames = 0;
	private Random _rng = new();
	private bool ShouldResetAnchor()
	{
		return false;
		_frames++;
		//if(_frames % 60 == 0)
		//	Console.WriteLine($"Frames: {_frames}");
		return _frames % 60 == 0;
	}

	protected override void OnExiting(object sender, EventArgs args)
	{
		base.OnExiting(sender, args);
	}
}