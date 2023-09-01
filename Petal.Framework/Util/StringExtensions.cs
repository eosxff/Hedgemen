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

	public static NamespacedString ToNamespaced(this string self)
		=> new(self);
}
