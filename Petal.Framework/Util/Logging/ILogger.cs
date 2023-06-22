﻿namespace Petal.Framework.Util.Logging;

public interface ILogger
{
	public LogLevel LogLevel { get; set; }
	public string Format { get; set; }
	public string DateTimeFormat { get; set; }

	public void Add(string message, LogLevel logLevel);
	public void Debug(string message);
	public void Warn(string message);
	public void Error(string message);
	public void Critical(string message);
}