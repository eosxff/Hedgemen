using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Petal.Framework.Assets;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AssetType
{
	None,
	Texture,
	Font,
	Effect,
	SoundEffect,
	Song
}

public static class AssetTypeExtensions
{
	public static JsonTypeInfo<AssetType> GetJsonTypeInfo(this AssetType type)
		=> AssetTypeJsonTypeInfo.Default.AssetType;
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(AssetType))]
internal partial class AssetTypeJsonTypeInfo : JsonSerializerContext
{

}
