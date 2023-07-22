using System.Collections.Generic;

namespace Petal.Framework.Util;

public static class DictionaryExtensions
{
	public static bool ChangeKey<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey from, TKey to)
	{
		if (!self.ContainsKey(from))
			return false;

		var value = self[from];
		self.Remove(from);
		self.Add(to, value);

		return true;
	}

	public static bool ChangeValue<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, TValue newValue)
	{
		if (!self.ContainsKey(key))
			return false;

		self.Remove(key);
		self.Add(key, newValue);

		return true;
	}

	public static bool TryRemove<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key)
	{
		if (!self.ContainsKey(key))
			return false;

		self.Remove(key);
		return true;
	}
}