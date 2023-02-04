using System;
using System.Text.Json;
using Petal.Windowing;

namespace Petal;

[Serializable]
public sealed class GameSettings
{
	public static JsonSerializerOptions JsonDeserializeOptions
		=> new();

	public int WindowWidth
	{
		get;
		set;
	} = 960;

	public int WindowHeight
	{
		get;
		set;
	} = 540;

	public WindowMode WindowMode
	{
		get;
		set;
	} = WindowMode.Windowed;

	public int PreferredFramerate
	{
		get;
		set;
	} = 60;

	public bool Vsync
	{
		get;
		set;
	} = true;

	public bool IsMouseVisible
	{
		get;
		set;
	} = true;

	public static GameSettings FromJson(string json)
	{
		var settings = JsonSerializer.Deserialize<GameSettings>(json, JsonDeserializeOptions);
		if (settings == null)
		{
			throw new ArgumentException($"Settings can not be created from json.");
		}
		return settings;
	}
}