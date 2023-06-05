using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Petal.Framework.Util;

namespace Petal.Framework.EntityComponent.Persistence;

[Serializable]
public sealed class SerializedRecord
{
	// is it just better to query appdomain assemblies?
	private static Func<IReadOnlyDictionary<string, Assembly>> _defaultRegisteredAssemblies = ()
		=> new Dictionary<string, Assembly>
		{
			{ typeof(string).Assembly.FullName!, typeof(string).Assembly }
		};

	public static Func<IReadOnlyDictionary<string, Assembly>> DefaultRegisteredAssemblies
	{
		get => _defaultRegisteredAssemblies;
		set
		{
			lock (_defaultRegisteredAssemblies)
			{
				_defaultRegisteredAssemblies = value;
			}
		}
	}

	private static JsonSerializerOptions DefaultJsonOptions
		=> new()
		{
			WriteIndented = true,
			IncludeFields = true
		};

	[JsonInclude]
	[JsonExtensionData]
	public Dictionary<string, JsonElement> Fields
	{
		get;
		private set;
	} = new();

	public static SerializedRecord FromJson(string json)
	{
		return JsonSerializer.Deserialize<SerializedRecord>(json, DefaultJsonOptions);
	}

	public SerializedRecord()
	{
	}

	public SerializedRecord(object obj)
	{
		RegisterObject(obj);
	}

	public T? GetField<T>(NamespacedString name)
	{
		var found = GetField<T>(name, out var field);
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
		var found = GetSerializedObject<T>(this, out var field);
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

		var record = element.Deserialize<SerializedRecord>(DefaultJsonOptions);

		return GetSerializedObject(record, out field);
	}

	private bool GetSerializedObject<T>(SerializedRecord record, [MaybeNullWhen(false)] out T field)
		where T : ISerializableObject
	{
		field = default;

		if (record.GetField(
			    new NamespacedString(NamespacedString.DefaultNamespace, "object_assembly_fullname"),
			    out string assemblyFullName) &&
		    record.GetField(
			    new NamespacedString(NamespacedString.DefaultNamespace, "object_type_fullname"),
			    out string typeFullName))
		{
			var registeredAssemblies = DefaultRegisteredAssemblies();

			var found = registeredAssemblies.TryGetValue(assemblyFullName, out var assembly);

			if (found)
			{
				var obj = assembly.CreateInstance(typeFullName);

				if (obj is T tObj)
				{
					field = tObj;
					field.ReadObjectState(this);
				}
			}
		}

		return true;
	}

	public T? GetSerializedObject<T>(NamespacedString name)
		where T : ISerializableObject
	{
		var found = GetSerializedObject<T>(name, out var field);
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