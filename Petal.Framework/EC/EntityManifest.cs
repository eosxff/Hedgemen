﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Petal.Framework.Persistence;
using Petal.Framework.Util;

namespace Petal.Framework.EC;

[Serializable]
public class EntityManifest
{
	public static EntityManifest? FromJson(string json)
	{
		JsonData? jsonData = JsonSerializer.Deserialize(json, EntityManifestJsc.Default.JsonData);
		return jsonData.HasValue ? jsonData.Value.Create() : null;
	}
	
	public NamespacedString ContentIdentifier
	{
		get;
		set;
	} = NamespacedString.Default;

	public IReadOnlyDictionary<NamespacedString, DataStorage> Components
	{
		get;
		set;
	} = new Dictionary<NamespacedString, DataStorage>();

	[Serializable]
	public struct JsonData : IDataRecord<EntityManifest>
	{
		[JsonPropertyName("content_id"), JsonInclude]
		public string ContentIdentifier
		{
			get;
			set;
		}

		[JsonPropertyName("components"), JsonInclude]
		public Dictionary<string, JsonElement> Components
		{
			get;
			set;
		}

		public EntityManifest Create()
		{
			var dictionary = new Dictionary<NamespacedString, DataStorage>(Components.Count);

			foreach (var componentKvp in Components)
			{
				var entry = componentKvp.Value.Deserialize(
					DataStorageJsc.Default.DataStorage);
				
				dictionary.Add(componentKvp.Key, entry);
			}
			
			var manifest = new EntityManifest
			{
				ContentIdentifier = new NamespacedString(ContentIdentifier),
				Components = dictionary
			};
			
			return manifest;
		}

		public void Read(EntityManifest obj)
		{
			Components = new Dictionary<string, JsonElement>(obj.Components.Count);
			ContentIdentifier = obj.ContentIdentifier.FullName;

			foreach (var component in obj.Components)
			{
				var entry = JsonSerializer.SerializeToElement(
					component.Value,
					DataStorageJsc.Default.DataStorage);
				
				Components.Add(component.Key, entry);
			}
		}
	}
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(EntityManifest.JsonData))]
internal partial class EntityManifestJsc : JsonSerializerContext
{
	
}