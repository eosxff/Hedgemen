using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Petal.Framework.Util;

namespace Petal.Framework.Persistence;

[Serializable]
public sealed class PersistentData
{
	public static JsonTypeInfo<PersistentData> JsonTypeInfo =>
		PersistentDataJsonTypeInfo.Default.PersistentData;

	private static JsonTypeInfo<PersistentDataInstantiateData> InstantiateDataJsonTypeInfo =>
		PersistentDataInstantiateDataJsonTypeInfo.Default.PersistentDataInstantiateData;

	private static readonly Dictionary<string, Assembly> Assemblies = new();

	private static JsonSerializerOptions DefaultJsonOptions
		=> new()
		{
			WriteIndented = true,
			IncludeFields = true,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		};

	public static PersistentData FromJson(JsonElement element)
	{
		var storage = element.Deserialize(JsonTypeInfo);
		PetalExceptions.ThrowIfNull(storage);
		return storage;
	}

	public static PersistentData FromJson(string json)
	{
		PetalExceptions.ThrowIfNullOrEmpty(json, nameof(json));
		var storage = JsonSerializer.Deserialize(json, JsonTypeInfo);
		PetalExceptions.ThrowIfNull(storage);
		return storage;
	}

	public static PersistentData FromStream(Stream stream)
	{
		var reader = new StreamReader(stream);
		return FromJson(reader.ReadToEnd());
	}

	/// <summary>
	/// This is only public for serialization purposes. Highly recommended to not mess with it directly.
	/// </summary>
	[JsonExtensionData]
	public Dictionary<string, object> ExtensionData
	{
		get;
		set;
	} = new();

	[RequiresUnreferencedCode("Use FromJson(string) instead if targeting ahead of time compilation.")]
	public static PersistentData FromJson(string json, JsonSerializerOptions? options)
	{
		options ??= DefaultJsonOptions;
		return JsonSerializer.Deserialize<PersistentData>(json, options);
	}

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
		[NotNullWhen(true)]
		out TField? field,
		TField defaultValue = default)
		=> ReadField(name.FullName, out field);

	public bool ReadField<TField>(
		string name,
		[NotNullWhen(true)]
		out TField? field,
		TField defaultValue = default)
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
			ExtensionData[name] = field; // todo maybe caching reference types is a terrible idea
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
	public bool ReadData<TData>(
		NamespacedString name,
		[NotNullWhen(true)]
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
			InstantiateDataJsonTypeInfo);

		ExtensionData.Add(InstantiateDataName, element);
	}

	/// <summary>
	/// Whether or not this instance represents an object itself and thus can be instantiated directly.
	/// </summary>
	public bool HasInstantiateData
		=> ExtensionData.ContainsKey(InstantiateDataName);

	private bool GetPersistentObject<T>(PersistentData storage, [NotNullWhen(true)] out T? field)
		where T : IPersistent
	{
		field = default;

		if (!GetInstantiateData(out var instantiateData))
			return false;

		var assembly = GetAssembly(instantiateData.AssemblyFullName);

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

		instantiateData = element.Deserialize(InstantiateDataJsonTypeInfo);
		return true;
	}

	private static Assembly GetAssembly(string assemblyFullName)
	{
		if(!Assemblies.ContainsKey(assemblyFullName))
			RepopulateAssemblies();

		bool found = Assemblies.TryGetValue(assemblyFullName, out var assembly);

		if (!found)
			throw new TypeAccessException($"{assemblyFullName} could not be found in the app domain.");

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
			JsonSerializer.SerializeToElement(handler.WriteData(), DefaultJsonOptions);

		ExtensionData.TryAdd(name, fieldJsonElement);
	}

	private static string InstantiateDataName => "instantiate_data";
}

[Serializable]
public struct PersistentDataInstantiateData
{
	[JsonPropertyName("type_full_name"), JsonInclude]
	public string? TypeFullName;

	[JsonPropertyName("assembly_full_name"), JsonInclude]
	public string? AssemblyFullName;

	[JsonIgnore]
	public bool IsValid => !string.IsNullOrEmpty(TypeFullName) || !string.IsNullOrEmpty(AssemblyFullName);
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(PersistentData))]
internal partial class PersistentDataJsonTypeInfo : JsonSerializerContext
{

}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(PersistentDataInstantiateData))]
internal partial class PersistentDataInstantiateDataJsonTypeInfo : JsonSerializerContext
{

}
