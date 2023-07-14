using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Petal.Framework.Util.Logging;

namespace Petal.Framework.Content;

public sealed class Registry
{
	private const int RegistersInitialCapacity = 0;
	
	private Dictionary<NamespacedString, IRegister> _registers = new(RegistersInitialCapacity);
	private ILogger _logger;

	public Registry(ILogger logger)
	{
		_logger = logger;
	}

	public bool AddRegister(IRegister register)
	{
		if (_registers.ContainsKey(register.RegistryName))
			return false;
		
		_registers.Add(register.RegistryName, register);
		return true;
	}

	public bool GetRegister<TRegister>(
		NamespacedString id,
		[MaybeNullWhen(false)] out TRegister register) where TRegister : IRegister
	{
		register = default;

		if (!_registers.TryGetValue(id, out var registerFromRegisters))
			return false;
		
		register = registerFromRegisters is TRegister castedRegister ? castedRegister : default;
		return register != null;
	}
}