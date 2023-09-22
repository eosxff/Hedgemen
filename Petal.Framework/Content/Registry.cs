using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Petal.Framework.Util.Logging;

namespace Petal.Framework.Content;

/// <summary>
/// Central depository for content registering.
/// </summary>
public sealed class Registry
{
	public sealed class RegisterAdded : EventArgs
	{
		public required IRegister Register
		{
			get;
			init;
		}
	}

	private const int RegistersInitialCapacity = 0;

	private readonly Dictionary<NamespacedString, IRegister> _registers = new(RegistersInitialCapacity);

	public readonly ILogger Logger;

	/// <summary>
	/// Callback for successful register additions.
	/// </summary>
	public event EventHandler<RegisterAdded>? OnRegisterAdded;

	public Registry(ILogger logger)
	{
		Logger = logger;
	}

	/// <summary>
	/// Attempts to register an <see cref="IRegister"/> to the registry.
	/// </summary>
	/// <param name="register">the register to be added.</param>
	/// <returns>value indicating if the addition was successful.</returns>
	public bool AddRegister(IRegister register)
	{
		if (_registers.ContainsKey(register.RegistryName))
		{
			Logger.Warn($"Attempting to add a register to registry with a duplicate name. Ignored.");
			return false;
		}

		_registers.Add(register.RegistryName, register);
		Logger.Info($"Added {register.RegistryName} to the registry.");

		OnRegisterAdded?.Invoke(this, new RegisterAdded
		{
			Register = register
		});

		return true;
	}

	/// <summary>
	/// Attempts to retrieve a register from the registry.
	/// </summary>
	/// <param name="id">the register's name.</param>
	/// <param name="register">the retrieved register.</param>
	/// <returns>value indicating if the retrieval was a success.</returns>
	public bool GetRegister(
		NamespacedString id,
		[NotNullWhen(true)] out IRegister? register)
	{
		register = default;

		if (!_registers.TryGetValue(id, out var registerFromRegisters))
			return false;

		register = registerFromRegisters;
		return true;
	}

	/// <summary>
	/// Attempts to retrieve a register from the registry.
	/// </summary>
	/// <param name="id">the register's name.</param>
	/// <param name="register">the retrieved register.</param>
	/// <returns>value indicating if the retrieval was a success.</returns>
	/// <exception cref="InvalidCastException">if the retrieved register could not be cast
	/// to the desired type.</exception>
	public bool GetRegister<TRegister>(
		NamespacedString id,
		[NotNullWhen(true)] out TRegister? register) where TRegister : IRegister
	{
		register = default;

		if (!GetRegister(id, out var registerFromRegistry))
			return false;

		if (registerFromRegistry is not TRegister tRegister)
		{
			var type1 = registerFromRegistry.GetType();
			var type2 = typeof(TRegister);

			throw new InvalidCastException($"Cannot cast {type1} to {type2}");
		}

		register = tRegister;
		return register != null;
	}
}
