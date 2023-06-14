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
public sealed class SerializedData
{
	private static readonly Dictionary<string, Assembly> Assemblies = new();

	private static JsonSerializerOptions DefaultJsonOptions
		=> new()
		{
			WriteIndented = true,
			IncludeFields = true
		};

	[JsonInclude, JsonExtensionData]
	public Dictionary<string, JsonElement> Fields
	{
		get;
		private set;
	} = new();

	public static SerializedData FromJson(string json)
	{
		return JsonSerializer.Deserialize<SerializedData>(json, DefaultJsonOptions);
	}

	public SerializedData()
	{
	}

	public SerializedData(object obj)
	{
		RegisterObject(obj);
	}

	public T? GetField<T>(NamespacedString name)
	{
		bool found = GetField<T>(name, out var field);
		return field;
	}

	public bool GetField<T>(NamespacedString name, [MaybeNullWhen(false)] out T field)
	{
		field = default;

		if (Fields.TryGetValue(name, out var element))
		{
			field = element.Deserialize<T>(DefaultJsonOptions);
			return field != null;
		}

		return false;
	}

	public T? GetSerializedObject<T>() where T : ISerializableObject
	{
		bool found = GetSerializedObject<T>(this, out var field);
		return field;
	}

	public bool GetSerializedObject<T>([MaybeNullWhen(false)] out T field) where T : ISerializableObject
	{
		field = default;
		return GetSerializedObject(this, out field);
	}

	public bool GetSerializedObject<T>(NamespacedString name, [MaybeNullWhen(false)] out T field)
		where T : ISerializableObject
	{
		field = default;

		if (!Fields.TryGetValue(name, out var element))
			return false;

		var record = element.Deserialize<SerializedData>(DefaultJsonOptions);

		return GetSerializedObject(record, out field);
	}

	private bool GetSerializedObject<T>(SerializedData data, [MaybeNullWhen(false)] out T field)
		where T : ISerializableObject
	{
		field = default;

		if (data.GetField(
			    new NamespacedString(NamespacedString.DefaultNamespace, "object_assembly_fullname"),
			    out string assemblyFullName) &&
		    data.GetField(
			    new NamespacedString(NamespacedString.DefaultNamespace, "object_type_fullname"),
			    out string typeFullName))
		{
			bool found = Assemblies.TryGetValue(assemblyFullName, out var assembly);

			if (!found)
				RepopulateAssemblies();

			found = Assemblies.TryGetValue(assemblyFullName, out assembly);

			if (found)
			{
				object obj = assembly.CreateInstance(typeFullName);

				if (obj is T tObj)
				{
					field = tObj;
					field.ReadObjectState(this);
				}
			}
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

	public T? GetSerializedObject<T>(NamespacedString name)
		where T : ISerializableObject
	{
		bool found = GetSerializedObject<T>(name, out var field);
		return field;
	}

	public T GetField<T>(NamespacedString name, T defaultValue)
	{
		return GetField<T>(name, out var field) ? field : defaultValue;
	}

	public bool GetField<T>(NamespacedString name, [MaybeNullWhen(false)] out T field, T defaultValue)
	{
		if (GetField(name, out field))
			return true;

		field = defaultValue;
		return false;
	}

	public void AddField(NamespacedString name, object field)
	{
		if (field is ISerializableObject serializableObject)
		{
			AddSerializedObject(name, serializableObject);
			return;
		}

		var serializedField = JsonSerializer.SerializeToElement(field, DefaultJsonOptions);
		Fields.TryAdd(name, serializedField);
	}

	public void AddSerializedObject(NamespacedString name, ISerializableObject serializableObject)
	{
		var serializedField =
			JsonSerializer.SerializeToElement(serializableObject.WriteObjectState(), DefaultJsonOptions);

		Fields.TryAdd(name, serializedField);
	}

	public void ReplaceField(NamespacedString name, object field)
	{
		if (!Fields.ContainsKey(name))
			return;

		var serializedField = JsonSerializer.SerializeToElement(field, DefaultJsonOptions);
		Fields.ChangeValue(name, serializedField);
	}

	public void RegisterObject(object obj)
	{
		AddField(
			new NamespacedString(NamespacedString.DefaultNamespace, "object_type_fullname"),
			obj.GetType().FullName);
		AddField(
			new NamespacedString(NamespacedString.DefaultNamespace, "object_assembly_fullname"),
			obj.GetType().Assembly.FullName);
	}
}