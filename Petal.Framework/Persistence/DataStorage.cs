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
public sealed class DataStorage
{
	public static JsonTypeInfo<DataStorage> JsonTypeInfo =>
		DataStorageJsonTypeInfo.Default.DataStorage;

	private static JsonTypeInfo<DataStorageInstantiateData> InstantiateDataJsonTypeInfo =>
		DataStorageInstantiateDataJsonTypeInfo.Default.DataStorageInstantiateData;

	private static readonly Dictionary<string, Assembly> Assemblies = new();

	private static JsonSerializerOptions DefaultJsonOptions
		=> new()
		{
			WriteIndented = true,
			IncludeFields = true,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		};

	public static DataStorage FromJson(JsonElement element)
	{
		var storage = element.Deserialize(JsonTypeInfo);
		PetalExceptions.ThrowIfNull(storage);
		return storage;
	}

	public static DataStorage FromJson(string json)
	{
		PetalExceptions.ThrowIfNullOrEmpty(json, nameof(json));
		var storage = JsonSerializer.Deserialize(json, JsonTypeInfo);
		PetalExceptions.ThrowIfNull(storage);
		return storage;
	}

	public static DataStorage FromStream(Stream stream)
	{
		var reader = new StreamReader(stream);
		return FromJson(reader.ReadToEnd());
	}

	[JsonExtensionData]
	public Dictionary<string, JsonElement> ExtensionData
	{
		get;
		set;
	} = new();

	[RequiresUnreferencedCode("Use FromJson(string) instead if targeting ahead of time compilation.")]
	public static DataStorage FromJson(string json, JsonSerializerOptions? options)
	{
		options ??= DefaultJsonOptions;
		return JsonSerializer.Deserialize<DataStorage>(json, options);
	}

	public DataStorage()
	{
	}

	public DataStorage(object? obj)
	{
		WriteInstantiateData(obj);
	}

	public T? ReadData<T>(NamespacedString name)
	{
		bool found = ReadData<T>(name, out var field);
		return field;
	}

	public bool ReadData<T>(NamespacedString name, [NotNullWhen(true)] out T? field)
	{
		field = default;

		if (ExtensionData.TryGetValue(name, out var element))
		{
			field = element.Deserialize<T>(DefaultJsonOptions);
			return field != null;
		}

		return false;
	}

	public T? InstantiateData<T>() where T : IDataStorageHandler
	{
		if (!HasInstantiateData)
		{
			throw new InvalidOperationException($"Storage does not have instantiate data.");
		}

		bool found = GetStorageHandler<T>(this, out var field);
		return field;
	}

	public bool InstantiateData<T>([NotNullWhen(true)] out T? field) where T : IDataStorageHandler
	{
		field = default;
		return GetStorageHandler(this, out field);
	}

	public T ReadData<T>(NamespacedString name, T defaultValue)
	{
		return ReadData<T>(name, out var field) ? field : defaultValue;
	}

	public bool ReadData<T>(NamespacedString name, out T field, T defaultValue)
	{
		if (ReadData(name, out field))
			return true;

		field = defaultValue;
		return false;
	}

	public void WriteData(NamespacedString name, object field)
	{
		if (field is IDataStorageHandler serializableObject)
		{
			WriteDataHandler(name, serializableObject);
			return;
		}

		var serializedField = JsonSerializer.SerializeToElement(field, DefaultJsonOptions);
		ExtensionData.TryAdd(name, serializedField);
	}

	private bool GetStorageHandler<T>(NamespacedString name, [NotNullWhen(true)] out T? field)
		where T : IDataStorageHandler
	{
		field = default;

		if (!ExtensionData.TryGetValue(name.FullName, out var element))
			return false;

		var record = element.Deserialize<DataStorage>(DefaultJsonOptions);

		return GetStorageHandler(record, out field);
	}

	private bool GetStorageHandler<T>(DataStorage storage, [NotNullWhen(true)] out T? field)
		where T : IDataStorageHandler
	{
		field = default;

		if (!GetInstantiateData(out var instantiateData))
			return false;

		var assembly = GetAssembly(instantiateData.AssemblyFullName);

		object? obj = assembly.CreateInstance(instantiateData.TypeFullName);

		if (obj is not T tObj)
			return false;

		field = tObj;
		field.ReadStorage(this);

		return true;
	}

	private bool GetInstantiateData(out DataStorageInstantiateData instantiateData)
	{
		instantiateData = default;

		if (!ExtensionData.TryGetValue(InstantiateDataID.FullName, out var element))
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

	public T? GetStorageHandler<T>(NamespacedString name)
		where T : IDataStorageHandler
	{
		bool found = GetStorageHandler<T>(name, out var field);
		return field;
	}

	private void WriteDataHandler(NamespacedString name, IDataStorageHandler handler)
	{
		var fieldJsonElement =
			JsonSerializer.SerializeToElement(handler.WriteStorage(), DefaultJsonOptions);

		ExtensionData.TryAdd(name, fieldJsonElement);
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

		var instantiateData = new DataStorageInstantiateData
		{
			TypeFullName = typeFullName,
			AssemblyFullName = assemblyFullName
		};

		var element = JsonSerializer.SerializeToElement(
			instantiateData,
			InstantiateDataJsonTypeInfo);

		ExtensionData.Add(InstantiateDataID.FullName, element);
	}

	/// <summary>
	/// Whether or not this instance represents an object itself and thus can be instantiated directly.
	/// </summary>
	public bool HasInstantiateData
		=> ExtensionData.ContainsKey(InstantiateDataID.FullName);

	private static NamespacedString InstantiateDataID => NamespacedString.FromDefaultNamespace("instantiate_data");
}

[Serializable]
public struct DataStorageInstantiateData
{
	[JsonPropertyName("type_full_name"), JsonInclude]
	public string? TypeFullName;

	[JsonPropertyName("assembly_full_name"), JsonInclude]
	public string? AssemblyFullName;

	[JsonIgnore]
	public bool IsValid => !string.IsNullOrEmpty(TypeFullName) || !string.IsNullOrEmpty(AssemblyFullName);
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(DataStorage))]
internal partial class DataStorageJsonTypeInfo : JsonSerializerContext
{

}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(DataStorageInstantiateData))]
internal partial class DataStorageInstantiateDataJsonTypeInfo : JsonSerializerContext
{

}
