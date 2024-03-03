using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Petal.Framework.Util;

namespace Petal.Framework.Persistence;

/// <summary>
/// Persistent storage for game states between sessions.
/// </summary>
[Serializable]
public sealed class PersistentData
{
	/// <summary>
	/// Source generated type info.
	/// </summary>
	public static JsonTypeInfo<PersistentData> JsonTypeInfo =>
		PersistentDataJsonTypeInfo.Default.PersistentData;

	/// <summary>
	/// All the currently loaded assemblies in the current app domain. This is not thread safe and is regenerated each
	/// time a type's assembly is not found in the dictionary.
	/// </summary>
	private static readonly Dictionary<string, Assembly> Assemblies = new();

	/// <summary>
	/// Default serialization options for writing fields to json.
	/// </summary>
	private static JsonSerializerOptions DefaultJsonOptions
		=> new()
		{
			WriteIndented = true,
			IncludeFields = true,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		};

	/// <summary>
	/// Deserializes a json element to <see cref="PersistentData"/>.
	/// </summary>
	/// <param name="json">the json element to deserialize.</param>
	/// <returns></returns>
	public static PersistentData FromJson(JsonElement json)
	{
		var storage = json.Deserialize(JsonTypeInfo);
		PetalExceptions.ThrowIfNull(storage);
		return storage;
	}

	/// <summary>
	/// Deserializes a json string to <see cref="PersistentData"/>.
	/// </summary>
	/// <param name="json">the json element to deserialize.</param>
	/// <returns></returns>
	public static PersistentData FromJson(string json)
	{
		PetalExceptions.ThrowIfNullOrEmpty(json, nameof(json));
		var storage = JsonSerializer.Deserialize(json, JsonTypeInfo);
		PetalExceptions.ThrowIfNull(storage);
		return storage;
	}

	/// <summary>
	/// Deserializes a json string from to <see cref="PersistentData"/>.
	/// </summary>
	/// <param name="stream">the stream to read the json string from. The string should be UTF8 format.</param>
	/// <returns></returns>
	public static PersistentData FromStream(Stream stream)
	{
		var reader = new StreamReader(stream, Encoding.UTF8);
		return FromJson(reader.ReadToEnd());
	}

	/// <summary>
	/// This is only public for serialization purposes. Highly recommended to not mess with it directly.
	/// </summary>
	[JsonExtensionData, JsonInclude]
	public Dictionary<string, object> ExtensionData
	{
		get;
		set;
	} = new();

	/// <summary>
	/// This is only public for serialization purposes. Highly recommended to use static methods or other constructors
	/// to get instances of <see cref="PersistentData"/>.
	/// </summary>
	public PersistentData()
	{
	}

	public PersistentData(object? obj)
	{
		WriteInstantiateData(obj);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TField? ReadField<TField>(NamespacedString name, TField? defaultValue = default)
	{
		ReadField(name.FullName, out var field, defaultValue);
		return field;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TField? ReadField<TField>(string name, TField? defaultValue = default)
	{
		ReadField(name, out var field, defaultValue);
		return field;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool ReadField<TField>(
		NamespacedString name,
		[NotNullWhen(true), NotNullIfNotNull(nameof(defaultValue))]
		out TField? field,
		TField defaultValue = default)
		=> ReadField(name.FullName, out field);

	public bool ReadField<TField>(
		string name,
		[NotNullWhen(true), NotNullIfNotNull(nameof(defaultValue))]
		out TField? field,
		TField? defaultValue = default)
	{
		field = defaultValue;

		if (!ExtensionData.TryGetValue(name, out object extension))
			return false;

		if (extension is JsonElement element)
		{
			var deserializedField = element.Deserialize<TField>(DefaultJsonOptions);

			if (deserializedField is null)
				return false;

			field = deserializedField;
			return true;
		}

		if (extension is TField dataT)
		{
			field = dataT;
			return true;
		}

		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[return: NotNullIfNotNull(nameof(defaultValue))]
	public TData? ReadData<TData>(
		NamespacedString name,
		TData? defaultValue = default)
		where TData : IPersistent
	{
		ReadData(name, out var data, defaultValue);
		return data;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[return: NotNullIfNotNull(nameof(defaultValue))]
	public TData? ReadData<TData>(
		string name,
		TData? defaultValue = default)
		where TData : IPersistent
	{
		ReadData(name, out var data, defaultValue);
		return data;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool ReadData<TData>(
		NamespacedString name,
		[NotNullWhen(true), NotNullIfNotNull(nameof(defaultValue))]
		out TData? data,
		TData? defaultValue = default)
		where TData : IPersistent
		=> ReadData(name.FullName, out data, defaultValue);

	public bool ReadData<TData>(
		string name,
		[NotNullWhen(true)]
		out TData? data,
		TData? defaultValue = default)
		where TData : IPersistent
	{
		data = defaultValue;

		if (!ExtensionData.TryGetValue(name, out object extension))
			return false;

		if (extension is not JsonElement element)
			return false;

		var persistentData = element.Deserialize(JsonTypeInfo);

		if (persistentData is null || !persistentData.HasInstantiateData)
			return false;

		if (!persistentData.InstantiateData(out TData instantiatedData))
			return false;

		instantiatedData.ReadData(persistentData);
		data = instantiatedData;
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool WriteField(NamespacedString name, object field)
		=> WriteField(name.FullName, field);

	public bool WriteField(string name, object field)
	{
		if (ExtensionData.ContainsKey(name))
			return false;

		var element = JsonSerializer.SerializeToElement(field, DefaultJsonOptions);
		ExtensionData.Add(name, element);
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool WriteData(NamespacedString name, IPersistent data)
		=> WriteData(name.FullName, data);

	public bool WriteData(string name, IPersistent data)
	{
		if (ExtensionData.ContainsKey(name))
			return false;

		var element = JsonSerializer.SerializeToElement(data.WriteData(), JsonTypeInfo);
		ExtensionData.Add(name, element);
		return true;
	}

	public T? InstantiateData<T>() where T : IPersistent
	{
		GetPersistentObject(this, out T field);
		return field;
	}

	public bool InstantiateData<T>([NotNullWhen(true)] out T? field) where T : IPersistent
	{
		field = default;
		return GetPersistentObject(this, out field);
	}

	public void WriteInstantiateData(object obj)
	{
		PetalExceptions.ThrowIfNull(obj);

		if (HasInstantiateData)
			throw new InvalidOperationException();

		string? typeFullName = obj.GetType().FullName;
		string? assemblyFullName = obj.GetType().Assembly.FullName;

		if (typeFullName is null || assemblyFullName is null)
			return;

		var instantiateData = new PersistentDataInstantiateData
		{
			TypeFullName = typeFullName,
			AssemblyFullName = assemblyFullName
		};

		var element = JsonSerializer.SerializeToElement(
			instantiateData,
			PersistentDataInstantiateData.JsonTypeInfo);

		ExtensionData.Add(InstantiateDataName, element);
	}

	/// <summary>
	/// Whether or not this instance represents an object itself and thus can be instantiated directly.
	/// </summary>
	public bool HasInstantiateData
		=> ExtensionData.ContainsKey(InstantiateDataName);

	public JsonElement Serialize()
	{
		return JsonSerializer.SerializeToElement(this, JsonTypeInfo);
	}

	private bool GetPersistentObject<T>(PersistentData storage, [NotNullWhen(true)] out T? field)
		where T : IPersistent
	{
		field = default;

		if (!GetInstantiateData(out var instantiateData))
			return false;

		var assembly = GetAssembly(instantiateData.AssemblyFullName, out bool foundAssembly);

		if (!foundAssembly)
			return false;

		object? obj = assembly.CreateInstance(instantiateData.TypeFullName);

		if (obj is not T tObj)
			return false;

		field = tObj;
		field.ReadData(this);
		return true;
	}

	private bool GetInstantiateData(out PersistentDataInstantiateData instantiateData)
	{
		instantiateData = default;

		if (!ExtensionData.TryGetValue(InstantiateDataName, out object extension))
			return false;

		if (extension is not JsonElement element)
			return false;

		instantiateData = element.Deserialize(PersistentDataInstantiateData.JsonTypeInfo);
		return true;
	}

	private static Assembly GetAssembly(string assemblyFullName, out bool result)
	{
		if(!Assemblies.ContainsKey(assemblyFullName))
			RepopulateAssemblies();

		result = Assemblies.TryGetValue(assemblyFullName, out var assembly);
		return assembly;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void RepopulateAssemblies()
	{
		Assemblies.Clear();

		var assemblies = AppDomain.CurrentDomain.GetAssemblies();

		foreach (var assembly in assemblies)
			Assemblies.Add(assembly.FullName!, assembly);
	}

	private void WritePersistentObject(NamespacedString name, IPersistent handler)
	{
		var fieldJsonElement =
			JsonSerializer.SerializeToElement(handler.WriteData(), JsonTypeInfo);

		ExtensionData.TryAdd(name, fieldJsonElement);
	}

	private static string InstantiateDataName => "instantiate_data";
}

[Serializable]
public struct PersistentDataInstantiateData
{
	public static JsonTypeInfo<PersistentDataInstantiateData> JsonTypeInfo =>
		PersistentDataInstantiateDataJsonTypeInfo.Default.PersistentDataInstantiateData;

	[JsonPropertyName("type_full_name"), JsonInclude]
	public string? TypeFullName;

	[JsonPropertyName("assembly_full_name"), JsonInclude]
	public string? AssemblyFullName;

	[JsonIgnore]
	public readonly bool IsValid => !string.IsNullOrEmpty(TypeFullName) || !string.IsNullOrEmpty(AssemblyFullName);
}

[JsonSourceGenerationOptions(WriteIndented = true, IgnoreReadOnlyProperties = true)]
[JsonSerializable(typeof(PersistentData))]
[JsonSerializable(typeof(JsonElement))]
internal partial class PersistentDataJsonTypeInfo : JsonSerializerContext
{

}

[JsonSourceGenerationOptions(WriteIndented = true, IgnoreReadOnlyProperties = true)]
[JsonSerializable(typeof(PersistentDataInstantiateData))]
[JsonSerializable(typeof(JsonElement))]
internal partial class PersistentDataInstantiateDataJsonTypeInfo : JsonSerializerContext
{

}
