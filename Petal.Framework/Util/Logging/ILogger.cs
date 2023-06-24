using System;

namespace Petal.Framework.Util.Logging;

public struct LogLevelChangedArgs
{
	public required LogLevel Old
	{
		get;
		init;
	}

	public required LogLevel New
	{
		get;
		init;
	}
}

public interface ILogger
{
	public event EventHandler<LogLevelChangedArgs> OnLogLevelChanged;

	public LogLevel LogLevel
	{
		get;
		set;
	}

	public string Format
	{
		get;
		set;
	}

	public string DateTimeFormat
	{
		get;
		set;
	}

	public void Add(string message, LogLevel logLevel);
	public void Debug(string message);
	public void Warn(string message);
	public void Error(string message);
	public void Critical(string message);
}