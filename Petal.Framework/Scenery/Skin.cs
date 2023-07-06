using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework.Util;

namespace Petal.Framework.Scenery;

public sealed class Skin
{
	private ContentRegistry? _contentRegistry;
	
	public ContentRegistry? ContentRegistry
	{
		get => _contentRegistry;
		private set
		{
			_contentRegistry = value;
			Refresh();
		}
	}

	public ButtonData Button
	{
		get;
		init;
	}

	public FontData Font
	{
		get;
		init;
	}

	public Skin() : this(null)
	{
	}

	public Skin(ContentRegistry? contentRegistry)
	{
		_contentRegistry = contentRegistry;
		
		Button = new ButtonData
		{
			HoverTexture = new ContentReference<Texture2D>(NamespacedString.Default, contentRegistry),
			InputTexture = new ContentReference<Texture2D>(NamespacedString.Default, contentRegistry),
			NormalTexture = new ContentReference<Texture2D>(NamespacedString.Default, contentRegistry)
		};

		Font = new FontData()
		{
			SmallFont = new ContentReference<SpriteFont>(NamespacedString.Default, contentRegistry),
			MediumFont = new ContentReference<SpriteFont>(NamespacedString.Default, contentRegistry),
			LargeFont = new ContentReference<SpriteFont>(NamespacedString.Default, contentRegistry)
		};
	}

	public sealed class ButtonData
	{
		public ContentReference<Texture2D> NormalTexture;
		public ContentReference<Texture2D> HoverTexture;
		public ContentReference<Texture2D> InputTexture;
	}

	public sealed class FontData
	{
		public ContentReference<SpriteFont> SmallFont;
		public ContentReference<SpriteFont> MediumFont;
		public ContentReference<SpriteFont> LargeFont;
	}

	public void Refresh()
	{
		if (ContentRegistry is null)
			return;

		Button.NormalTexture.ReloadItem(ContentRegistry);
		Button.HoverTexture.ReloadItem(ContentRegistry);
		Button.InputTexture.ReloadItem(ContentRegistry);
		
		Font.SmallFont.ReloadItem(ContentRegistry);
		Font.MediumFont.ReloadItem(ContentRegistry);
		Font.LargeFont.ReloadItem(ContentRegistry);
	}

	public static Skin FromJson(string json, ContentRegistry? registry)
	{
		var skin = JsonSerializer.Deserialize(json, SkinDataRecordJsc.Default.DataRecord).Create();
		
		skin.ContentRegistry = registry;

		return skin;
	}

	[Serializable]
	public struct DataRecord : IDataRecord<Skin>
	{
		[JsonIgnore]
		public static JsonSerializerOptions JsonDeserializeOptions
			=> new()
			{
				IncludeFields = true,
				WriteIndented = true,
				Converters = { }
			};

		[JsonInclude, JsonPropertyName("button_data_normal_texture_name")]
		public string ButtonDataNormalTextureName;

		[JsonInclude, JsonPropertyName("button_data_hover_texture_name")]
		public string ButtonDataHoverTextureName;

		[JsonInclude, JsonPropertyName("button_data_input_texture_name")]
		public string ButtonDataInputTextureName;
		
		[JsonInclude, JsonPropertyName("font_data_small_font_name")]
		public string FontDataSmallFontName;
		
		[JsonInclude, JsonPropertyName("font_data_medium_font_name")]
		public string FontDataMediumFontName;
		
		[JsonInclude, JsonPropertyName("font_data_large_font_name")]
		public string FontDataLargeFontName;

		public Skin Create()
		{
			return new Skin
			{
				Button = new ButtonData
				{
					NormalTexture = new ContentReference<Texture2D>(ButtonDataNormalTextureName),
					HoverTexture = new ContentReference<Texture2D>(ButtonDataHoverTextureName),
					InputTexture = new ContentReference<Texture2D>(ButtonDataInputTextureName)
				},
				
				Font = new FontData
				{
					SmallFont = new ContentReference<SpriteFont>(FontDataSmallFontName),
					MediumFont = new ContentReference<SpriteFont>(FontDataMediumFontName),
					LargeFont = new ContentReference<SpriteFont>(FontDataLargeFontName),
				}
			};
		}

		public void Read(Skin obj)
		{
			ButtonDataNormalTextureName = obj.Button.NormalTexture.ContentID.FullName;
			ButtonDataHoverTextureName = obj.Button.HoverTexture.ContentID.FullName;
			ButtonDataInputTextureName = obj.Button.InputTexture.ContentID.FullName;

			FontDataSmallFontName = obj.Font.SmallFont.ContentID.FullName;
			FontDataMediumFontName = obj.Font.MediumFont.ContentID.FullName;
			FontDataLargeFontName = obj.Font.LargeFont.ContentID.FullName;
		}
	}
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Skin.DataRecord))]
public partial class SkinDataRecordJsc : JsonSerializerContext
{
	
}