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
		=> ThrowIfNull(obj, nameof(obj));

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

	/// <summary>
	/// Throws <see cref="ArgumentNullException"/> with <paramref name="message"/> if <paramref name="obj"/> is null.
	/// If no exception is raised then <paramref name="obj"/> is not null.
	/// </summary>
	/// <param name="obj">the object to evaluate.</param>
	/// <param name="message">the exception message to print.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ThrowIfNullWithMessage([NotNull] object? obj, string message)
		=> ThrowIfNullWithMessage(obj, nameof(obj), message);

	/// <summary>
	/// Throws <see cref="ArgumentNullException"/> with <paramref name="message"/> if <paramref name="obj"/> is null.
	/// If no exception is raised then <paramref name="obj"/> is not null.
	/// </summary>
	/// <param name="obj">the object to evaluate.</param>
	/// <param name="message">the exception message to print.</param>
	/// <param name="paramName">the parameter name to print if an exception is raised.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ThrowIfNullWithMessage([NotNull] object? obj, string paramName, string message)
	{
		if(obj is null)
			throw new ArgumentNullException(paramName, message);
	}

	/// <summary>
	/// Throws <see cref="ArgumentNullException"/> if <paramref name="str"/> is null and <see cref="ArgumentException"/>
	/// if <paramref name="str"/> is empty. If no exception is raised then <paramref name="str"/> is not null or empty.
	/// </summary>
	/// <param name="str">the <see cref="String"/> to evaluate.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ThrowIfNullOrEmpty([NotNull] string? str)
		=> ThrowIfNullOrEmpty(str, nameof(str));

	/// <summary>
	/// Throws <see cref="ArgumentNullException"/> if <paramref name="str"/> is null and <see cref="ArgumentException"/>
	/// if <paramref name="str"/> is empty. If no exception is raised then <paramref name="str"/> is not null or empty.
	/// </summary>
	/// <param name="str">the <see cref="String"/> to evaluate.</param>
	/// <param name="paramName">the parameter name to print if an exception is raised.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ThrowIfNullOrEmpty([NotNull] string? str, string paramName)
	{
		if (str is null)
			throw new ArgumentNullException(paramName);
		if (str.Length == 0)
			throw new ArgumentException($"{paramName} is empty.");
	}

	public static void ThrowIf(bool expression)
		=> ThrowIf(expression, $"{nameof(expression)} is false.");

	public static void ThrowIf(bool expression, string message)
	{
		if(!expression)
			throw new PetalException(message);
	}
}

/// <summary>
/// Generic exception for petal. <see cref="PetalGame.Logger"/> will ideally give more details when this exception
/// is raised.
/// </summary>
public sealed class PetalException : Exception
{
	public PetalException() { }
	public PetalException(string? message) : base(message) { }
	public PetalException(string? message, Exception? innerException) : base(message, innerException) { }
}
