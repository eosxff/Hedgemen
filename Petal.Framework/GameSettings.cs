using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Petal.Framework.Util.Logging;
using Petal.Framework.Windowing;
using static Petal.Framework.Util.PetalUtilities;

namespace Petal.Framework;

[Serializable]
public struct GameSettings
{
	public static JsonTypeInfo<GameSettings> JsonTypeInfo
		=> GameSettingsJsonTypeInfo.Default.GameSettings;

	[JsonPropertyName("window_width"), JsonInclude]
	public int WindowWidth
	{
		get;
		set;
	} = 960;

	[JsonPropertyName("window_height"), JsonInclude]
	public int WindowHeight
	{
		get;
		set;
	} = 540;

	[JsonPropertyName("window_mode"), JsonInclude]
	public WindowMode WindowMode
	{
		get;
		set;
	} = WindowMode.Windowed;

	[JsonPropertyName("preferred_framerate"), JsonInclude]
	public int PreferredFramerate
	{
		get;
		set;
	} = 60;

	[JsonPropertyName("vsync_enabled"), JsonInclude]
	public bool Vsync
	{
		get;
		set;
	} = true;

	[JsonPropertyName("is_mouse_visible"), JsonInclude]
	public bool IsMouseVisible
	{
		get;
		set;
	} = true;

	[JsonPropertyName("is_window_user_resizable"), JsonInclude]
	public bool IsWindowUserResizable
	{
		get;
		set;
	} = false;

	[JsonPropertyName("is_debug"), JsonInclude]
	public bool IsDebug
	{
		get;
		set;
	} = false;

	public GameSettings()
	{
	}

	public static GameSettings FromJson(string json)
		=> ReadFromJson(json, JsonTypeInfo);

	public static GameSettings FromJson(JsonElement json)
		=> ReadFromJson(json, JsonTypeInfo);

	public readonly override string ToString()
	{
		return $"[{nameof(WindowWidth)}:{WindowWidth}, " +
			   $"{nameof(WindowHeight)}:{WindowHeight}, " +
			   $"{nameof(WindowMode)}:{WindowMode}, " +
			   $"{nameof(PreferredFramerate)}:{PreferredFramerate}, " +
			   $"{nameof(Vsync)}:{Vsync}, " +
			   $"{nameof(IsMouseVisible)}:{IsMouseVisible}, " +
			   $"{nameof(IsWindowUserResizable)}:{IsWindowUserResizable}, " +
			   $"{nameof(IsDebug)}:{IsDebug}]";
	}
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(GameSettings))]
[JsonConverter(typeof(JsonStringEnumConverter))]
internal partial class GameSettingsJsonTypeInfo : JsonSerializerContext
{

}
