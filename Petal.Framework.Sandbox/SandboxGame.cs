using System;
using System.IO;
using System.Text;
using Hgm.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Petal.Framework.EC;
using Petal.Framework.Graphics;
using Petal.Framework.Input;
using Petal.Framework.IO;
using Petal.Framework.Scenery;
using Petal.Framework.Scenery.Nodes;
using Petal.Framework.Windowing;

namespace Petal.Framework.Sandbox;

using Framework;

public class SandboxGame : PetalGame
{
	public static SandboxGame Instance
	{
		get;
		private set;
	}

	public SandboxGame()
	{
		Instance = this;
	}
}
