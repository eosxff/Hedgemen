using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Petal.Framework.Persistence;

public class ListJsonConverter<T, TConverter> : JsonConverter<IList<T>>
	where TConverter : JsonConverter<T>, new()
{
	private readonly JsonConverter<T> _converter = new TConverter();

	public override IList<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var list = new List<T>();

		if (reader.TokenType != JsonTokenType.StartArray)
		{
			reader.Skip();
			return list;
		}

		while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
		{
			var item = _converter.Read(ref reader, typeof(T), options);

			if (item is null)
				continue;

			list.Add(item);
		}

		return list;
	}

	public override void Write(Utf8JsonWriter writer, IList<T> value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();

		foreach (var item in value)
			_converter.Write(writer, item, options);

		writer.WriteEndArray();
	}
}

public class ImmutableListJsonConverter<T, TConverter> : JsonConverter<IReadOnlyList<T>>
	where TConverter : JsonConverter<T>, new()
{
	private readonly JsonConverter<T> _converter = new TConverter();

	public override IReadOnlyList<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var list = new List<T>();

		if (reader.TokenType != JsonTokenType.StartArray)
		{
			reader.Skip();
			return list;
		}

		while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
		{
			var item = _converter.Read(ref reader, typeof(T), options);

			if (item is null)
				continue;

			list.Add(item);
		}

		return list;
	}

	public override void Write(Utf8JsonWriter writer, IReadOnlyList<T> value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();

		foreach (var item in value)
			_converter.Write(writer, item, options);

		writer.WriteEndArray();
	}
}
