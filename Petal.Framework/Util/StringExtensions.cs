using System;
using System.Text.Json;

namespace Petal.Framework.Util;

public static class StringExtensions
{
	public static int Occurrences(this string str, char c)
	{
		int occurrences = 0;

		foreach (var cStr in str)
		{
			occurrences = (cStr == c) ? occurrences + 1 : occurrences;
		}

		return occurrences;
	}

	public static int Occurrences(this ReadOnlySpan<char> span, char c)
	{
		int occurrences = 0;

		foreach (var cStr in span)
		{
			occurrences = (cStr == c) ? occurrences + 1 : occurrences;
		}

		return occurrences;
	}

	public static NamespacedString AsNamespace(this string self, string name)
	{
		return new NamespacedString(self, name);
	}

	public static NamespacedString AsNamespacedName(this string self, string nameSpace)
	{
		return new NamespacedString(nameSpace, self);
	}
}
