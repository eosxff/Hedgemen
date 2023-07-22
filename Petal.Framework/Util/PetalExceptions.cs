using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Petal.Framework.Util;

public static class PetalExceptions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ThrowIfNull([NotNull] object? obj)
	{
		if (obj is null)
			throw new ArgumentNullException(nameof(obj));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ThrowIfNull([NotNull] object? obj, string paramName)
	{
		if (obj is null)
			throw new ArgumentNullException(paramName);
	}
}
