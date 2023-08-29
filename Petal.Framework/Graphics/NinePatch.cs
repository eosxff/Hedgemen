using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace Petal.Framework.Graphics;

[Serializable]
public struct NinePatch
{
	[JsonPropertyName("top_left"), JsonInclude]
	public required Rectangle TopLeft
	{
		get;
		init;
	}

	[JsonPropertyName("top"), JsonInclude]
	public required Rectangle Top
	{
		get;
		init;
	}

	[JsonPropertyName("top_right"), JsonInclude]
	public required Rectangle TopRight
	{
		get;
		init;
	}

	[JsonPropertyName("left"), JsonInclude]
	public required Rectangle Left
	{
		get;
		init;
	}

	[JsonPropertyName("center"), JsonInclude]
	public required Rectangle Center
	{
		get;
		init;
	}

	[JsonPropertyName("right"), JsonInclude]
	public required Rectangle Right
	{
		get;
		init;
	}

	[JsonPropertyName("bottom_left"), JsonInclude]
	public required Rectangle BottomLeft
	{
		get;
		init;
	}

	[JsonPropertyName("bottom"), JsonInclude]
	public required Rectangle Bottom
	{
		get;
		init;
	}

	[JsonPropertyName("bottom_right"), JsonInclude]
	public required Rectangle BottomRight
	{
		get;
		init;
	}

	public Rectangle[] ToArray()
		=> new[] { TopLeft, Top, TopRight, Left, Center, Right, BottomLeft, Bottom, BottomRight };

	[SetsRequiredMembers]
	public NinePatch(Rectangle rectangle, int leftPadding, int rightPadding, int topPadding, int bottomPadding)
	{
		int x = rectangle.X;
		int y = rectangle.Y;
		int w = rectangle.Width;
		int h = rectangle.Height;
		int centerWidth = w - leftPadding - rightPadding;
		int centerHeight = h - topPadding - bottomPadding;
		int bottomY = y + h - bottomPadding;
		int rightX = x + w - rightPadding;
		int leftX = x + leftPadding;
		int topY = y + topPadding;

		TopLeft = new Rectangle(x, y, leftPadding, topPadding);
		Top = new Rectangle(leftX, y, centerWidth, topPadding);
		TopRight = new Rectangle(rightX, y, rightPadding, topPadding);
		Left = new Rectangle(x, topY, leftPadding, centerHeight);
		Center = new Rectangle(leftX, topY, centerWidth, centerHeight);
		Right = new Rectangle(rightX, topY, rightPadding, centerHeight);
		BottomLeft = new Rectangle(x, bottomY, leftPadding, bottomPadding);
		Bottom = new Rectangle(leftX, bottomY, centerWidth, bottomPadding);
		BottomRight = new Rectangle(rightX, bottomY, rightPadding, bottomPadding);
	}
}
