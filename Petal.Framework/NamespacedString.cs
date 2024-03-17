using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Petal.Framework.Util.Extensions;

namespace Petal.Framework;

[Serializable]
public struct NamespacedString
{
	public static string DefaultNamespace
		=> "nil";
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

	/// <summary>
	/// A valid namespaced string has 4 rules: it must not be null, it must contain a single colon, it must have 0
	/// spaces, and it must be a minimum of 3 characters (ex: a:b).
	/// </summary>
	/// <param name="str">the string to evaluate.</param>
	/// <returns>if the provided <paramref name="str"/> is a valid namespaced string.</returns>
	public static bool IsValidQualifiedString(string? str)
	{
		if (string.IsNullOrEmpty(str))
			return false;

		bool correctColonOccurrences = str.Occurrences(':') == 1;
		bool noSpaceOccurrences = !str.Contains(' ');
		bool hasMinimumThreeCharacters = str.Length >= 3; // "_:_"

		return correctColonOccurrences && noSpaceOccurrences && hasMinimumThreeCharacters;
	}

	private string _namespace;
	private string _name;

	public readonly bool IsDefaultNamespacedString
		=> this == Default;

	[JsonConstructor]
	public NamespacedString(string fullyQualifiedString)
	{
		if (!IsValidQualifiedString(fullyQualifiedString))
			throw new ArgumentException($"String '{fullyQualifiedString}' is not a valid namespaced string!");

		int colonIndex = fullyQualifiedString.IndexOf(':');
		_namespace = fullyQualifiedString[.. colonIndex];
		_name = fullyQualifiedString[colonIndex ..];
	}

	public NamespacedString(NamespacedString other) : this(other.Namespace, other.Name)
	{

	}

	public NamespacedString(string @namespace, string name)
	{
		_namespace = @namespace;
		_name = name;
	}

	/// <summary>
	/// Pre-colon string. namespace:_
	/// </summary>
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

	/// <summary>
	/// Post-colon string. _:name
	/// </summary>
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
	public readonly string FullName
		=> _namespace + ':' + _name;

	public override readonly bool Equals(object obj)
	{
		if (obj is not NamespacedString nsStr)
			return false;

		return FullName.Equals(nsStr.FullName, StringComparison.InvariantCulture);
	}

	public override readonly int GetHashCode()
		=> FullName.GetHashCode();

	public override readonly string ToString()
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
}
