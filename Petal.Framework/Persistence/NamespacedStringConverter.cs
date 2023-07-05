using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Petal.Framework.Persistence;

public class NamespacedStringConverter : JsonConverter<NamespacedString>
{
	public override NamespacedString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if(typeToConvert != typeof(string))
			return NamespacedString.Default;

		while (reader.Read())
		{
			string? value = reader.GetString();
			
			return string.IsNullOrEmpty(value) ? NamespacedString.Default : new NamespacedString(value);

		}
		
		return NamespacedString.Default;
	}

	public override void Write(Utf8JsonWriter writer, NamespacedString value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.FullName);
	}
}