using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Petal.Framework.Windowing;

[JsonConverter(typeof(JsonStringEnumConverter<WindowMode>))]
public enum WindowMode
{
	// unfortunately the json serializer does not work for custom enum names. maybe one day :/
	// that day has come (for serialization only)

	//[JsonPropertyName("windowed"), EnumMember(Value = "windowed")]
	Windowed,
	//[JsonPropertyName("borderless_windowed"), EnumMember(Value = "borderless_windowed")]
	BorderlessWindowed,
	//[JsonPropertyName("fullscreen"), EnumMember(Value = "fullscreen")]
	Fullscreen,
	//[JsonPropertyName("borderless_fullscreen"), EnumMember(Value = "borderless_fullscreen")]
	BorderlessFullscreen
}
