using System.Collections.Generic;

namespace Petal.Framework.Persistence;

public static class PersistenceExtensions
{
	public static List<DataStorage> WriteStorageList<T>(this IReadOnlyList<T> self)
		where T : IDataStorageHandler
	{
		var list = new List<DataStorage>(self.Count);

		foreach (var element in self)
		{
			list.Add(element.WriteStorage());
		}

		return list;
	}

	public static List<T> ReadStorageList<T>(this IReadOnlyList<DataStorage> self)
		where T : IDataStorageHandler
	{
		var list = new List<T>(self.Count);

		foreach (var element in self)
		{
			if(!element.HasInstantiateData)
				continue;

			list.Add(element.InstantiateData<T>());
		}

		return list;
	}

	public static Dictionary<TKey, DataStorage> WriteStorageDictionary<TKey, TValue>(
		this IReadOnlyDictionary<TKey, TValue> self)
		where TValue : IDataStorageHandler
	{
		var dictionary = new Dictionary<TKey, DataStorage>(self.Count);

		foreach (var kvp in self)
		{
			dictionary.Add(kvp.Key, kvp.Value.WriteStorage());
		}

		return dictionary;
	}

	public static Dictionary<TKey, TValue> ReadStorageDictionary<TKey, TValue>(
		this IReadOnlyDictionary<TKey, DataStorage> self)
		where TValue : IDataStorageHandler
	{
		var dictionary = new Dictionary<TKey, TValue>(self.Count);

		foreach (var kvp in self)
		{
			if(!kvp.Value.HasInstantiateData)
				continue;

			dictionary.Add(kvp.Key, kvp.Value.InstantiateData<TValue>());
		}

		return dictionary;
	}
}
