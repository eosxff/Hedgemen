using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Petal.Framework.Util.Logging;

public class PetalLogger : ILogger
{
	private readonly struct LogEntry
	{
		public required string Message
		{
			get;
			init;
		}

		public required StackFrame StackFrame
		{
			get;
			init;
		}

		public required LogLevel Level
		{
			get;
			init;
		}

		public required bool Silent
		{
			get;
			init;
		}
	}

	private readonly StringBuilder _builder = new();
	private readonly BlockingCollection<LogEntry> _entries = new();

	private LogLevel _logLevel = LogLevel.Off;

	public event EventHandler<LogLevelChangedArgs> OnLogLevelChanged;

	public PetalLogger()
	{
		Task.Factory.StartNew(HandleEntries);
	}

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
		=> HandleAddEntry(message, LogLevel.Debug, 2);

	public void Info(string message)
		=> HandleAddEntry(message, LogLevel.Info, 2);

	public void Warn(string message)
		=> HandleAddEntry(message, LogLevel.Warn, 2);

	public void Error(string message)
		=> HandleAddEntry(message, LogLevel.Error, 2);

	public void Critical(string message)
		=> HandleAddEntry(message, LogLevel.Critical, 2);

	public void Add(string message, LogLevel logLevel)
		=> HandleAddEntry(message, logLevel, 2);

	private void HandleAddEntry(string message, LogLevel level, int stackFrameSkipFrames)
	{
		// never log when it's supposed to be off
		if (level == LogLevel.Off)
			return;

		bool silent = !ValidLogLevel(level);

		if (!LogInvalidLevelsSilently && silent)
			return;

		_entries.Add(new LogEntry
		{
			Message = message,
			Level = level,
			StackFrame = new StackFrame(stackFrameSkipFrames, true),
			Silent = silent
		});
	}

	private void HandleEntries()
	{
		foreach (var entry in _entries.GetConsumingEnumerable())
		{
			Add(entry.Message, entry.Level, entry.StackFrame, entry.Silent);
		}
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
