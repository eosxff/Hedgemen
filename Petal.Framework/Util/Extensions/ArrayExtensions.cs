using System.Collections.Generic;

namespace Petal.Framework.Util.Extensions;

public static class ArrayExtensions
{
	public static IEnumerable<T> ToEnumerable<T>(this T[,] self)
	{
		foreach (var element in self)
			yield return element;
	}
}
