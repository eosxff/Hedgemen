using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Petal.Framework.Assets;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AssetType
{
	[JsonPropertyName("none")]
	[EnumMember(Value = "none")]
	None,
	[JsonPropertyName("texture")]
	[EnumMember(Value = "texture")]
	Texture,
	[JsonPropertyName("font")]
	[EnumMember(Value = "font")]
	Font,
	[JsonPropertyName("effect")]
	[EnumMember(Value = "effect")]
	Effect,
	[JsonPropertyName("sound_effect")]
	[EnumMember(Value = "sound_effect")]
	SoundEffect,
	[JsonPropertyName("song")]
	[EnumMember(Value = "song")]
	Song
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(AssetType))]
public partial class AssetTypeJsc : JsonSerializerContext
{

}
