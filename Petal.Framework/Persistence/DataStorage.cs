using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Petal.Framework.Util;

namespace Petal.Framework.Persistence;

[Serializable]
public sealed class DataStorage
{
	private static readonly Dictionary<string, Assembly> Assemblies = new();

	private static JsonSerializerOptions DefaultJsonOptions
		=> new()
		{
			WriteIndented = true,
			IncludeFields = true,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		};

	[JsonPropertyName("type_full_name"), JsonInclude]
	public string? TypeFullName
	{
		get;
		set;
	}

	[JsonPropertyName("assembly_full_name"), JsonInclude]
	public string? AssemblyFullName
	{
		get;
		set;
	}

	public bool IsStorageHandler
		=> !string.IsNullOrEmpty(TypeFullName) && !string.IsNullOrEmpty(AssemblyFullName);
		
	[JsonInclude, JsonExtensionData]
	public Dictionary<string, JsonElement> Fields
	{
		get;
		private set;
	} = new();

	[RequiresUnreferencedCode("Use FromJson(string) instead if targeting ahead of time compilation.")]
	public static DataStorage FromJson(string json, JsonSerializerOptions? options)
	{
		options ??= DefaultJsonOptions;
		return JsonSerializer.Deserialize<DataStorage>(json, options);
	}

	public static DataStorage FromJson(string json)
	{
		return JsonSerializer.Deserialize(json, DataStorageJsc.Default.DataStorage);
	}

	public DataStorage()
	{
	}

	public DataStorage(object? obj)
	{
		SetSelfData(obj);
	}

	public T? ReadData<T>(NamespacedString name)
	{
		bool found = ReadData<T>(name, out var field);
		return field;
	}

	public bool ReadData<T>(NamespacedString name, [MaybeNullWhen(false)] out T field)
	{
		field = default;
		var genericType = typeof(T);

		if (Fields.TryGetValue(name, out var element))
		{
			field = element.Deserialize<T>(DefaultJsonOptions);
			return field != null;
		}

		return false;
	}

	public T? ReadData<T>() where T : IDataStorageHandler
	{
		if (!IsStorageHandler)
		{
			throw new InvalidOperationException($"{nameof(DataStorage)} cannot be instantiated from itself." +
			                                    $"IsStorageHandler returned false.");
		}
		
		bool found = GetStorageHandler<T>(this, out var field);
		return field;
	}

	public bool ReadData<T>([MaybeNullWhen(false)] out T field) where T : IDataStorageHandler
	{
		field = default;
		return GetStorageHandler(this, out field);
	}

	private bool GetStorageHandler<T>(NamespacedString name, [MaybeNullWhen(false)] out T field)
		where T : IDataStorageHandler
	{
		field = default;

		if (!Fields.TryGetValue(name, out var element))
			return false;

		var record = element.Deserialize<DataStorage>(DefaultJsonOptions);

		return GetStorageHandler(record, out field);
	}

	private bool GetStorageHandler<T>(DataStorage storage, [MaybeNullWhen(false)] out T field)
		where T : IDataStorageHandler
	{
		field = default;

		if (TypeFullName is null || AssemblyFullName is null)
			return false;
		
		if(!Assemblies.ContainsKey(AssemblyFullName))
			RepopulateAssemblies();

		bool found = Assemblies.TryGetValue(AssemblyFullName, out var assembly);

		if (!found)
			throw new TypeAccessException($"{TypeFullName} could not be found in the application.");

		object obj = assembly.CreateInstance(TypeFullName);

		if (obj is T tObj)
		{
			field = tObj;
			field.ReadStorage(this);
		}
		
		return true;
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
			AddStorageHandler(name, serializableObject);
			return;
		}

		var serializedField = JsonSerializer.SerializeToElement(field, DefaultJsonOptions);
		Fields.TryAdd(name, serializedField);
	}

	private void AddStorageHandler(NamespacedString name, IDataStorageHandler handler)
	{
		var fieldJsonElement =
			JsonSerializer.SerializeToElement(handler.WriteStorage(), DefaultJsonOptions);

		Fields.TryAdd(name, fieldJsonElement);
	}

	public void ReplaceData(NamespacedString name, object field)
	{
		if (!Fields.ContainsKey(name))
			return;

		var fieldJsonElement = JsonSerializer.SerializeToElement(field, DefaultJsonOptions);
		Fields.ChangeValue(name, fieldJsonElement);
	}

	public void SetSelfData(object? obj)
	{
		if (obj is null)
			return;

		string? typeFullName = obj.GetType().FullName;
		string? assemblyFullName = obj.GetType().Assembly.FullName;

		if (typeFullName is null || assemblyFullName is null)
			return;

		TypeFullName = typeFullName;
		AssemblyFullName = assemblyFullName;
	}
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(DataStorage))]
public partial class DataStorageJsc : JsonSerializerContext
{
	
}