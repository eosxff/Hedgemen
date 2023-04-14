namespace Petal.Framework.Sandbox;

using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Framework;
using Scenery;
using Scenery.Nodes;
using Windowing;

public class SandboxGame : PetalGame
{
	private class Animal
	{
		public virtual string AnimalName
			=> "Animal";
	}

	private class Dog : Animal
	{
		public override string AnimalName
			=> "Dog";
	}

	private class Cat : Animal
	{
		public override string AnimalName
			=> "Cat";
	}
	
	public static SandboxGame? Instance
	{
		get;
		private set;
	}

	public SandboxGame()
	{
		Instance = this;
	}

	public ContentRegistry GameContent
	{
		get;
	} = new ();

	protected override void Initialize()
	{
		base.Initialize();

		Window.AllowUserResizing = true;
		
		GameContent.Register("sandbox:favourite_pet", new Animal());
		Console.WriteLine(GameContent.Get<Animal>("sandbox:favourite_pet").Item?.AnimalName);
		GameContent.Replace("sandbox:favourite_pet", new Dog());
		Console.WriteLine(GameContent.Get<Animal>("sandbox:favourite_pet").Item?.AnimalName);
		GameContent.Replace("sandbox:favourite_pet", new Cat());
		Console.WriteLine(GameContent.Get<Animal>("sandbox:favourite_pet").Item?.AnimalName);

		GameContent.Register("sandbox:best_number_ever", 128);
		GameContent.Register("sandbox:best_object_ever", new object());
		
		// image these textures actually existed in the content bank!
		var skin = new Skin
		{
			Button = new Skin.ButtonData
			{
				InputTexture = GameContent.Get<Texture2D>("sandbox:textures/ui_button_down"),
				HoverTexture = GameContent.Get<Texture2D>("sandbox:textures/ui_button_hover"),
				RegularTexture = GameContent.Get<Texture2D>("sandbox:textures/ui_button_regular")
			}
		};

		var contentRef = GameContent.Get<Texture2D>("sandbox:textures/ui_button_down");

		Console.WriteLine($"Found: {contentRef.Item != null}");

		/*if (contentRef.Item is int bestNumberEver)
		{
			Console.WriteLine("Best Number Ever: " + bestNumberEver);
		}

		var contentItem = GameContent.Get<object>("sandbox:best_object_ever");

		if (contentItem.Item is not null)
		{
			Console.WriteLine("Best Object Ever: " + contentItem.Item);
		}*/

		var scene = new Scene(new Stage
		{
			Tag = "root",
			Name = "sandbox:ui/root_node",
			VirtualResolution = new Vector2Int(640, 360),
		}, null)
		{
			BackgroundColor = Color.Black,
			Skin = skin
		};

		var texture = Assets.LoadAsset<Texture2D>(new FileInfo("peach.png").Open(FileMode.Open));
		
		var image = scene.Root.Add(new Image
		{
			Bounds = new Rectangle(0, 0, 64, 64),
			Texture = texture,
			Name = "sandbox:image_1",
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
		/*image.OnMouseDown += node => Console.WriteLine("MouseDown");
		image.OnMousePressed += node =>
		{
			Console.WriteLine("Click!");
			node.MarkedForDeletion = true;
		};
		image.OnMouseReleased += node => Console.WriteLine("MouseReleased");*/
		
		var image2 = image.Add(new Image
		{
			Bounds = new Rectangle(0, 0, 32, 32),
			Texture = texture,
			Name = "sandbox:image_2",
			Color = Color.Green,
			Anchor = Anchor.Center
		});
		
		var image3 = image2.Add(new Image
		{
			Bounds = new Rectangle(8, 8, 16, 16),
			Texture = texture,
			Name = "sandbox:image_3",
			Color = Color.Blue,
			Anchor = Anchor.BottomLeft
		});

		var image4 = scene.Root.Add(new Image
		{
			Bounds = new Rectangle(0, 0, scene.Root.VirtualResolution.X, scene.Root.VirtualResolution.Y),
			Anchor = Anchor.TopLeft,
			Color = Color.White,
			Name = "sandbox:image_4",
			Texture = texture
		});

		image4.OnMousePressed += node =>
		{
			node.IsMarkedForDeletion = true;
		};

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
			WindowWidth = 960,
			WindowHeight = 540,
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
		if(_frames % 60 == 0)
			Console.WriteLine($"Frames: {_frames}");
		return _frames % 60 == 0;
	}

	protected override void OnExiting(object sender, EventArgs args)
	{
		base.OnExiting(sender, args);
	}
}