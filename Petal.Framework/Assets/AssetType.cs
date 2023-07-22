using System.Text.Json.Serialization;

namespace Petal.Framework.Assets;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AssetType
{
	[JsonPropertyName("none")]
	None,
	[JsonPropertyName("texture")]
	Texture,
	[JsonPropertyName("font")]
	Font,
	[JsonPropertyName("effect")]
	Effect,
	[JsonPropertyName("sound_effect")]
	SoundEffect,
	[JsonPropertyName("song")]
	Song
}
