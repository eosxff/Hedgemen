using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework.Util;

namespace Petal.Framework.Scenery;

public sealed class Skin
{
	public event EventHandler? OnSkinRefreshed;
	
	private ContentRegistry? _contentRegistry;
	
	public ContentRegistry? ContentRegistry
	{
		get => _contentRegistry;
		set
		{
			if (_contentRegistry is not null)
			{
				_contentRegistry.OnContentReplaced -= ContentRegistryShouldRefreshOnReplaced;
				_contentRegistry.OnContentRemoved -= ContentRegistryShouldRefreshOnRemoved;
			}
			
			_contentRegistry = value;

			if (_contentRegistry is not null)
			{
				_contentRegistry.OnContentReplaced += ContentRegistryShouldRefreshOnReplaced;
				_contentRegistry.OnContentRemoved += ContentRegistryShouldRefreshOnRemoved;
			}
			
			Refresh();
		}
	}

	private void ContentRegistryShouldRefreshOnReplaced(object? sender, ContentRegistry.ContentReplacedArgs args)
	{
		if (sender is not ContentRegistry registry)
			return;

		if (ContentIDMatchesAny(args.ReplacedContent.ContentID))
		{
			ShouldRefresh = true;
		}
	}
	
	private void ContentRegistryShouldRefreshOnRemoved(object? sender, ContentRegistry.ContentRemovedArgs args)
	{
		if (sender is not ContentRegistry registry)
			return;

		if (ContentIDMatchesAny(args.RemovedContent.ContentID))
		{
			ShouldRefresh = true;
		}
	}

	private bool ContentIDMatchesAny(NamespacedString contentID)
	{
		return contentID == Button.HoverTexture.ContentID ||
		       contentID == Button.InputTexture.ContentID ||
		       contentID == Button.NormalTexture.ContentID ||
		       contentID == Font.SmallFont.ContentID ||
		       contentID == Font.MediumFont.ContentID ||
		       contentID == Font.LargeFont.ContentID;
	}

	private ButtonData _button;

	public ButtonData Button
	{
		get
		{
			HandleRefresh();
			return _button;
		}
		init => _button = value;
	}

	private FontData _font;

	public FontData Font
	{
		get
		{
			HandleRefresh();
			return _font;
		}
		init => _font = value;
	}

	public bool ShouldRefresh
	{
		get;
		set;
	} = true;

	public Skin() : this(null)
	{
	}

	public Skin(ContentRegistry? contentRegistry)
	{
		ContentRegistry = contentRegistry;

		_button = new ButtonData
		{
			HoverTexture = new ContentReference<Texture2D>(NamespacedString.Default, contentRegistry),
			InputTexture = new ContentReference<Texture2D>(NamespacedString.Default, contentRegistry),
			NormalTexture = new ContentReference<Texture2D>(NamespacedString.Default, contentRegistry)
		};

		_font = new FontData()
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

		_button.NormalTexture.ReloadItem(ContentRegistry);
		_button.HoverTexture.ReloadItem(ContentRegistry);
		_button.InputTexture.ReloadItem(ContentRegistry);
		
		_font.SmallFont.ReloadItem(ContentRegistry);
		_font.MediumFont.ReloadItem(ContentRegistry);
		_font.LargeFont.ReloadItem(ContentRegistry);
	}

	private void HandleRefresh()
	{
		if (!ShouldRefresh)
			return;

		Refresh();
		ShouldRefresh = false;
		
		OnSkinRefreshed?.Invoke(this, EventArgs.Empty);
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
		[JsonConverter(typeof(NamespacedString.JsonConverter))]
		public NamespacedString ButtonDataNormalTextureName;

		[JsonInclude, JsonPropertyName("button_data_hover_texture_name")]
		[JsonConverter(typeof(NamespacedString.JsonConverter))]
		public NamespacedString ButtonDataHoverTextureName;

		[JsonInclude, JsonPropertyName("button_data_input_texture_name")]
		[JsonConverter(typeof(NamespacedString.JsonConverter))]
		public NamespacedString ButtonDataInputTextureName;
		
		[JsonInclude, JsonPropertyName("font_data_small_font_name")]
		[JsonConverter(typeof(NamespacedString.JsonConverter))]
		public NamespacedString FontDataSmallFontName;
		
		[JsonInclude, JsonPropertyName("font_data_medium_font_name")]
		[JsonConverter(typeof(NamespacedString.JsonConverter))]
		public NamespacedString FontDataMediumFontName;
		
		[JsonInclude, JsonPropertyName("font_data_large_font_name")]
		[JsonConverter(typeof(NamespacedString.JsonConverter))]
		public NamespacedString FontDataLargeFontName;

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
			ButtonDataNormalTextureName = obj.Button.NormalTexture.ContentID;
			ButtonDataHoverTextureName = obj.Button.HoverTexture.ContentID;
			ButtonDataInputTextureName = obj.Button.InputTexture.ContentID;

			FontDataSmallFontName = obj.Font.SmallFont.ContentID;
			FontDataMediumFontName = obj.Font.MediumFont.ContentID;
			FontDataLargeFontName = obj.Font.LargeFont.ContentID;
		}
	}
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Skin.DataRecord))]
public partial class SkinDataRecordJsc : JsonSerializerContext
{
	
}