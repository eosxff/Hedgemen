using System.Text.Json.Serialization;

namespace Petal.Windowing;

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