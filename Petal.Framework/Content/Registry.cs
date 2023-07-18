using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Petal.Framework.Util.Logging;

namespace Petal.Framework.Content;

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
	private readonly ILogger _logger;

	public event EventHandler<RegisterAdded>? OnRegisterAdded;

	public Registry(ILogger logger)
	{
		_logger = logger;
	}

	public bool AddRegister(IRegister register)
	{
		if (_registers.ContainsKey(register.RegistryName))
			return false;
		
		_registers.Add(register.RegistryName, register);
		_logger.Debug($"Added {register.RegistryName} to the registry.");
		
		OnRegisterAdded?.Invoke(this, new RegisterAdded
		{
			Register = register
		});
		
		return true;
	}

	public bool GetRegister(
		NamespacedString id,
		[MaybeNullWhen(false)] out IRegister register)
	{
		register = default;

		if (!_registers.TryGetValue(id, out var registerFromRegisters))
		{
			_logger.Debug($"Could not retrieve {id} from registry.");
			return false;
		}

		register = registerFromRegisters;
		return true;
	}

	public bool GetRegister<TRegister>(
		NamespacedString id,
		[MaybeNullWhen(false)] out TRegister register) where TRegister : IRegister
	{
		register = default;
		
		if (!GetRegister(id, out var registerFromRegisters))
			return false;

		register = registerFromRegisters is TRegister castedRegister ? castedRegister : default;
		return register != null;
	}
}