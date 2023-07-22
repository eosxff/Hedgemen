using System.Text.Json.Serialization;

namespace Petal.Framework.Windowing;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WindowMode
{
	[JsonPropertyName("windowed")]
	Windowed,
	[JsonPropertyName("borderless_windowed")]
	BorderlessWindowed,
	[JsonPropertyName("fullscreen")]
	Fullscreen,
	[JsonPropertyName("borderless_fullscreen")]
	BorderlessFullscreen
}
