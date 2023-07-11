using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Petal.Framework;

[Serializable]
public struct NamespacedString
{
	public static string DefaultNamespace
		=> "any";
	public static string DefaultName
		=> "null";

	public static NamespacedString Default
		=> new(DefaultNamespace, DefaultName);

	public static NamespacedString FromDefaultNamespace(string name)
	{
		return new NamespacedString(DefaultNamespace, name);
	}

	public static NamespacedString FromDefaultName(string @namespace)
	{
		return new NamespacedString(@namespace, DefaultName);
	}

	private string _namespace;
	private string _name;

	[JsonConstructor]
	public NamespacedString(string fullyQualifiedString)
	{
		if (fullyQualifiedString is null)
			throw new NullReferenceException(nameof(fullyQualifiedString));
		
		string[] fullyQualifiedStringSplit = fullyQualifiedString.Split(':');

		if (!IsValidQualifiedString(fullyQualifiedString))
			throw new ArgumentException($"String '{fullyQualifiedString}' is not a valid namespaced string!");

		_namespace = fullyQualifiedStringSplit[0];
		_name = fullyQualifiedStringSplit[1];
	}

	public NamespacedString(string @namespace, string name)
	{
		_namespace = @namespace;
		_name = name;
	}

	[JsonIgnore]
	public string Namespace
	{
		get
		{
			_namespace ??= DefaultNamespace;
			return _namespace;
		}

		set => _namespace = value;
	}

	[JsonIgnore]
	public string Name
	{
		get
		{
			_name ??= DefaultName;
			return _name;
		}

		set => _name = value;
	}

	[JsonPropertyName("namespaced_string")]
	public string FullName
		=> _namespace + ':' + _name;

	private static bool IsValidQualifiedString(string? fullyQualifiedString)
	{
		if (fullyQualifiedString is null)
			return false;
		
		string[] fullyQualifiedStringSplit = fullyQualifiedString.Split(':');

		bool correctNumberOfSplits = fullyQualifiedStringSplit.Length == 2;
		bool noSpaces = !fullyQualifiedString.Contains(' ');

		return correctNumberOfSplits && noSpaces;
	}

	public override string ToString()
	{
		return FullName;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator NamespacedString(string val)
	{
		return new NamespacedString(val);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator string(NamespacedString val)
	{
		return val.FullName;
	}

	public class JsonConverter : JsonConverter<NamespacedString>
	{
		public override NamespacedString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			string? value = reader.GetString();
			return string.IsNullOrEmpty(value) ? NamespacedString.Default : new NamespacedString(value);
		}

		public override void Write(Utf8JsonWriter writer, NamespacedString value, JsonSerializerOptions options)
		{
			writer.WriteStringValue(value.FullName);
		}
	}

	public class ListJsonConverter : JsonConverter<IList<NamespacedString>>
	{
		public override IList<NamespacedString> Read(
			ref Utf8JsonReader reader,
			Type typeToConvert,
			JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.StartArray)
				throw new JsonException();

			var list = new List<NamespacedString>();

			while (reader.Read())
			{
				if (reader.TokenType == JsonTokenType.EndArray)
					return list;

				if (reader.TokenType != JsonTokenType.String)
					throw new JsonException();

				string? namespacedString = reader.GetString();
				
				if(IsValidQualifiedString(namespacedString))
					list.Add(new NamespacedString(namespacedString));
			}
		
			return list;
		}

		public override void Write(Utf8JsonWriter writer, IList<NamespacedString> value, JsonSerializerOptions options)
		{
			writer.WriteStartArray();
		
			foreach (var namespacedString in value)
			{
				writer.WriteStringValue(namespacedString.FullName);
			}
		
			writer.WriteEndArray();
		}
	}

	public class ImmutableListJsonConverter : JsonConverter<IReadOnlyList<NamespacedString>>
	{
		public override IReadOnlyList<NamespacedString> Read(
			ref Utf8JsonReader reader,
			Type typeToConvert,
			JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.StartArray)
				throw new JsonException();

			var list = new List<NamespacedString>();

			while (reader.Read())
			{
				if (reader.TokenType == JsonTokenType.EndArray)
					return list;

				if (reader.TokenType != JsonTokenType.String)
					throw new JsonException();

				string? namespacedString = reader.GetString();
				
				if(IsValidQualifiedString(namespacedString))
					list.Add(new NamespacedString(namespacedString));
			}
		
			return list;
		}

		public override void Write(
			Utf8JsonWriter writer,
			IReadOnlyList<NamespacedString> value,
			JsonSerializerOptions options)
		{
			writer.WriteStartArray();
		
			foreach (var namespacedString in value)
			{
				writer.WriteStringValue(namespacedString.FullName);
			}
		
			writer.WriteEndArray();
		}
	}
}