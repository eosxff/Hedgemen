using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Petal.Framework.Util;

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

	public static bool IsValidQualifiedString(string? str)
	{
		if (string.IsNullOrEmpty(str))
			return false;
		
		bool correctColonOccurrences = str.Occurrences(':') == 1;
		bool noSpaceOccurrences = str.Occurrences(' ') == 0;
		bool hasMinimumThreeCharacters = str.Length >= 3;

		return correctColonOccurrences && noSpaceOccurrences && hasMinimumThreeCharacters;
	}

	private string _namespace;
	private string _name;

	public bool IsDefaultNamespacedString
		=> this == Default;

	[JsonConstructor]
	public NamespacedString(string fullyQualifiedString)
	{
		if (!IsValidQualifiedString(fullyQualifiedString))
		{
			throw new ArgumentException($"String '{fullyQualifiedString}' is not a valid namespaced string!");
		}

		string[] fullyQualifiedStringSplit = fullyQualifiedString.Split(':');

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

	public override bool Equals(object obj)
	{
		if (obj is not NamespacedString nsStr)
			return false;

		return FullName.Equals(nsStr.FullName, StringComparison.InvariantCulture);
	}

	public override int GetHashCode()
		=> HashCode.Combine(Namespace, Name);

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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(NamespacedString val1, NamespacedString val2)
		=> val1.Equals(val2);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(NamespacedString val1, NamespacedString val2)
		=> !val1.Equals(val2);

	public class JsonConverter : JsonConverter<NamespacedString>
	{
		public override NamespacedString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			string? value = reader.GetString();
			return string.IsNullOrEmpty(value) ? Default : new NamespacedString(value);
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