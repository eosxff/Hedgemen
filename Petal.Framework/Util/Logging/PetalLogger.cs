using System;
using System.Diagnostics;
using System.Text;

namespace Petal.Framework.Util.Logging;

public class PetalLogger : ILogger
{
	private readonly StringBuilder _builder;
	private LogLevel _logLevel = LogLevel.Off;

	public PetalLogger()
	{
		_builder = new StringBuilder();
	}

	public event EventHandler<LogLevelChangedArgs> OnLogLevelChanged;

	public bool LogInvalidLevelsSilently
	{
		get;
		set;
	} = true;

	public LogLevel LogLevel
	{
		get => _logLevel;
		set
		{
			if (_logLevel == value)
				return;

			var args = new LogLevelChangedArgs
			{
				Old = _logLevel,
				New = value
			};

			_logLevel = value;

			OnLogLevelChanged?.Invoke(this, args);
		}
	}

	public string Format
	{
		get;
		set;
	} = "[%T/%L] %M [%c:%m:%l]";

	public string DateTimeFormat
	{
		get;
		set;
	} = "HH:mm:ss";

	public void Debug(string message)
		=> HandleAdd(message, LogLevel.Debug, 2);

	public void Info(string message)
		=> HandleAdd(message, LogLevel.Info, 2);

	public void Warn(string message)
		=> HandleAdd(message, LogLevel.Warn, 2);

	public void Error(string message)
		=> HandleAdd(message, LogLevel.Error, 2);

	public void Critical(string message)
		=> HandleAdd(message, LogLevel.Critical, 2);

	public void Add(string message, LogLevel logLevel)
		=> HandleAdd(message, logLevel, 2);

	private void HandleAdd(string message, LogLevel logLevel, int skipFrames)
	{
		// never log when it's supposed to be off
		if (logLevel == LogLevel.Off)
			return;

		bool silent = !ValidLogLevel(logLevel);

		if (!LogInvalidLevelsSilently && silent)
			return;

		var stackFrame = new StackFrame(skipFrames, true);
		Add(message, logLevel, stackFrame, silent);
	}

	private void Add(string message, LogLevel logLevel, StackFrame stackFrame, bool silent)
	{
		var oldConsoleColor = Console.ForegroundColor;
		var consoleColor = ConsoleColor.White;

		switch (logLevel)
		{
			case LogLevel.Debug:
				consoleColor = ConsoleColor.Green;
				break;

			case LogLevel.Info:
				break;

			case LogLevel.Warn:
				consoleColor = ConsoleColor.Yellow;
				break;

			case LogLevel.Error:
				consoleColor = ConsoleColor.Red;
				break;

			case LogLevel.Critical:
				consoleColor = ConsoleColor.Blue;
				break;
		}

		string dateTime = DateTime.Now.ToString(DateTimeFormat);
		string? className = stackFrame.GetMethod() != null ? stackFrame.GetMethod()?.DeclaringType?.Name : "null";
		string? fileName = stackFrame.GetFileName();
		string? methodName = stackFrame.GetMethod() != null ? stackFrame.GetMethod()?.Name : "null";
		int lineNumber = stackFrame.GetFileLineNumber();

		string fullMessage = Format
			.Replace("%L", logLevel.ToString())
			.Replace("%T", dateTime)
			.Replace("%c", className)
			.Replace("%n", fileName)
			.Replace("%m", methodName)
			.Replace("%l", lineNumber.ToString())
			.Replace("%M", message);

		if (!silent)
		{
			Console.ForegroundColor = consoleColor;
			Console.WriteLine(fullMessage);
			Console.ForegroundColor = oldConsoleColor;
		}

		_builder.Append(fullMessage).Append('\n');
	}

	private bool ValidLogLevel(LogLevel logLevel)
	{
		return logLevel >= LogLevel;
	}

	public override string ToString()
	{
		return _builder.ToString();
	}
}
