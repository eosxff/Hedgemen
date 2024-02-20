using System;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Hgm.Game.Campaigning;

public sealed class CampaignSession(DirectoryInfo sessionDirectory, Hedgemen hedgemen)
{
	public Hedgemen Hedgemen
	{
		get;
	} = hedgemen;

	public DirectoryInfo Directory
	{
		get;
	} = sessionDirectory;

	public void Save()
	{
		// todo stub
	}

	public sealed class PlayerLocation
	{
		public static JsonTypeInfo<PlayerLocation> JsonTypeInfo
			=> PlayerLocationJsonTypeInfo.Default.PlayerLocation;

		[JsonPropertyName("world_guid"), JsonInclude]
		public Guid WorldGuid
		{
			get;
			set;
		} = Guid.Empty;

		[JsonPropertyName("player_x"), JsonInclude]
		public int PlayerX
		{
			get;
			set;
		} = 0;

		[JsonPropertyName("player_y"), JsonInclude]
		public int PlayerY
		{
			get;
			set;
		}
	}
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(CampaignSession.PlayerLocation))]
internal partial class PlayerLocationJsonTypeInfo : JsonSerializerContext
{

}
