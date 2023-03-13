using System.Text.Json.Serialization;

namespace Petal.Framework.Windowing;

public enum WindowMode
{
	[JsonPropertyName("windowed")]
	Windowed,
	[JsonPropertyName("borderlessWindowed")]
	BorderlessWindowed,
	[JsonPropertyName("fullscreen")]
	Fullscreen,
	[JsonPropertyName("borderlessFullscreen")]
	BorderlessFullscreen
}