using System.Collections.Generic;

namespace Petal.Framework.Persistence;

public static class PersistenceExtensions
{
	public static List<PersistentData> WritePersistentList<T>(this IReadOnlyList<T> self)
		where T : IPersistent
	{
		var list = new List<PersistentData>(self.Count);

		foreach (var element in self)
		{
			list.Add(element.WriteData());
		}

		return list;
	}

	public static List<T> ReadPersistentList<T>(this IReadOnlyList<PersistentData> self)
		where T : IPersistent
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

	public static Dictionary<TKey, PersistentData> WritePersistentDictionary<TKey, TValue>(
		this IReadOnlyDictionary<TKey, TValue> self)
		where TValue : IPersistent
	{
		var dictionary = new Dictionary<TKey, PersistentData>(self.Count);

		foreach (var kvp in self)
		{
			dictionary.Add(kvp.Key, kvp.Value.WriteData());
		}

		return dictionary;
	}

	public static Dictionary<TKey, TValue> ReadPersistentDictionary<TKey, TValue>(
		this IReadOnlyDictionary<TKey, PersistentData> self)
		where TValue : IPersistent
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
