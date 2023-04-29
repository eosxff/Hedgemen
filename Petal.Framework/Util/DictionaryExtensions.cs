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
}