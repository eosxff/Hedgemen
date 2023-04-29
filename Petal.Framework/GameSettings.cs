using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Petal.Framework.Windowing;

using static Petal.Framework.Util.PetalUtilities;

namespace Petal.Framework;

[Serializable]
public struct GameSettings
{
	public static JsonSerializerOptions JsonDeserializeOptions
		=> new JsonSerializerOptions
		{
			IgnoreReadOnlyProperties = true,
			IgnoreReadOnlyFields = true,
			WriteIndented = true,
			Converters = {  }
		};

	[JsonInclude]
	[JsonPropertyName("window_width")]
	public int WindowWidth
	{
		get;
		set;
	} = 960;

	[JsonInclude]
	[JsonPropertyName("window_height")]
	public int WindowHeight
	{
		get;
		set;
	} = 540;

	[JsonInclude]
	[JsonPropertyName("window_mode")]
	public WindowMode WindowMode
	{
		get;
		set;
	} = WindowMode.Windowed;

	[JsonInclude]
	[JsonPropertyName("preferred_framerate")]
	public int PreferredFramerate
	{
		get;
		set;
	} = 60;

	[JsonInclude]
	[JsonPropertyName("vsync_enabled")]
	public bool Vsync
	{
		get;
		set;
	} = true;

	[JsonInclude]
	[JsonPropertyName("is_mouse_visible")]
	public bool IsMouseVisible
	{
		get;
		set;
	} = true;

	[JsonInclude]
	[JsonPropertyName("is_window_user_resizable")]
	public bool IsWindowUserResizable
	{
		get;
		set;
	} = false;

	public GameSettings()
	{
		
	}

	public static GameSettings FromJson(string json)
		=> ReadFromJson<GameSettings>(json, JsonDeserializeOptions);
}