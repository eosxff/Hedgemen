using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework.Util;

using static Petal.Framework.Util.PetalUtilities;

namespace Petal.Framework.Scenery;

public sealed class Skin
{
	public ContentRegistry? Registry
	{
		get;
		set;
	}
	
	public ButtonData Button
	{
		get;
		init;
	}

	public sealed class ButtonData
	{
		public ContentReference<Texture2D> NormalTexture;
		public ContentReference<Texture2D> HoverTexture;
		public ContentReference<Texture2D> InputTexture;
	}

	public static Skin FromJson(string json)
		=> ReadFromJson<DataRecord>(json, DataRecord.JsonDeserializeOptions).Create();

	[Serializable]
	public struct DataRecord : IDataRecord<Skin>
	{
		public static JsonSerializerOptions JsonDeserializeOptions
			=> new JsonSerializerOptions
			{
				IncludeFields = true,
				WriteIndented = true,
				Converters = {  }
			};
		
		[JsonInclude]
		[JsonPropertyName("button_data_normal_texture_name")]
		public string ButtonDataNormalTextureName;
		
		[JsonInclude]
		[JsonPropertyName("button_data_hover_texture_name")]
		public string ButtonDataHoverTextureName;
		
		[JsonInclude]
		[JsonPropertyName("button_data_input_texture_name")]
		public string ButtonDataInputTextureName;

		public Skin Create()
		{
			return new Skin
			{
				Button = new ButtonData
				{
					NormalTexture = new ContentReference<Texture2D>(ButtonDataNormalTextureName),
					HoverTexture = new ContentReference<Texture2D>(ButtonDataHoverTextureName),
					InputTexture = new ContentReference<Texture2D>(ButtonDataInputTextureName),
				}
			};
		}

		public void Read(Skin obj)
		{
			ButtonDataNormalTextureName = obj.Button.NormalTexture.ContentIdentifier;
			ButtonDataHoverTextureName = obj.Button.HoverTexture.ContentIdentifier;
			ButtonDataInputTextureName = obj.Button.InputTexture.ContentIdentifier;
		}
	}
}