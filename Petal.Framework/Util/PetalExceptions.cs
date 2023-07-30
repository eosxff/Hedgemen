using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Petal.Framework.Util;

/// <summary>
/// Utility class for exceptions.
/// </summary>
public static class PetalExceptions
{
	/// <summary>
	/// Throws <see cref="ArgumentNullException"/> if <paramref name="obj"/> is null. If no
	/// exception is raised then <paramref name="obj"/> is not null.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ThrowIfNull([NotNull] object? obj)
	{
		if (obj is null)
			throw new ArgumentNullException(nameof(obj));
	}

	/// <summary>
	/// Throws <see cref="ArgumentNullException"/> if <paramref name="obj"/> is null. If no
	/// exception is raised then <paramref name="obj"/> is not null.
	/// </summary>
	/// <param name="obj">the object to evaluate.</param>
	/// <param name="paramName">the parameter name to print if an exception is raised.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ThrowIfNull([NotNull] object? obj, string paramName)
	{
		if (obj is null)
			throw new ArgumentNullException(paramName);
	}
}

/// <summary>
/// Generic exception for petal.
/// </summary>
public sealed class PetalException : Exception
{
	public PetalException() { }
	public PetalException(string? message) : base(message) { }
	public PetalException(string? message, Exception? innerException) : base(message, innerException) { }
}
