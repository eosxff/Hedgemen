using System;
using System.Text.Json;

namespace Petal.Framework.Util.Extensions;

public static class StringExtensions
{
	public static int Occurrences(this string str, char c)
	{
		int occurrences = 0;

		foreach (char cFromStr in str)
			occurrences = (cFromStr == c) ? occurrences + 1 : occurrences;

		return occurrences;
	}

	public static int Occurrences(this ReadOnlySpan<char> span, char c)
	{
		int occurrences = 0;

		foreach (char cFromSpan in span)
		{
			occurrences = (cFromSpan == c) ? occurrences + 1 : occurrences;
		}

		return occurrences;
	}

	public static NamespacedString ToNamespaced(this string self)
		=> new(self);
}
