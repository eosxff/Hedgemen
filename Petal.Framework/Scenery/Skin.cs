using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework.Content;
using Petal.Framework.Util;

namespace Petal.Framework.Scenery;

public sealed class Skin
{
	private Register<object> _assets;

	public Register<object> Assets
	{
		get => _assets;
		set
		{
			_assets = value;
			Refresh();
		}
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

	public Skin(Register<object>? assets)
	{
		_assets = assets;

		if (assets is not null)
		{
			_button = new ButtonData
			{
				HoverTexture = assets.MakeReference<Texture2D>(NamespacedString.Default),
				InputTexture = assets.MakeReference<Texture2D>(NamespacedString.Default),
				NormalTexture = assets.MakeReference<Texture2D>(NamespacedString.Default),
			};

			_font = new FontData()
			{
				SmallFont = assets.MakeReference<SpriteFont>(NamespacedString.Default),
				MediumFont = assets.MakeReference<SpriteFont>(NamespacedString.Default),
				LargeFont = assets.MakeReference<SpriteFont>(NamespacedString.Default),
			};
		}

		else
		{
			_button = new ButtonData
			{
				HoverTexture = RegistryObject<Texture2D>.Empty,
				InputTexture = RegistryObject<Texture2D>.Empty,
				NormalTexture = RegistryObject<Texture2D>.Empty,
			};

			_font = new FontData()
			{
				SmallFont = RegistryObject<SpriteFont>.Empty,
				MediumFont = RegistryObject<SpriteFont>.Empty,
				LargeFont = RegistryObject<SpriteFont>.Empty,
			};
		}
	}

	public sealed class ButtonData
	{
		public RegistryObject<Texture2D> NormalTexture;
		public RegistryObject<Texture2D> HoverTexture;
		public RegistryObject<Texture2D> InputTexture;
	}

	public sealed class FontData
	{
		public RegistryObject<SpriteFont> SmallFont;
		public RegistryObject<SpriteFont> MediumFont;
		public RegistryObject<SpriteFont> LargeFont;
	}

	public void Refresh()
	{
		if (_assets is null)
			return;

		_button.NormalTexture = _assets.MakeReference<Texture2D>(_button.NormalTexture.Location);
		_button.HoverTexture = _assets.MakeReference<Texture2D>(_button.HoverTexture.Location);
		_button.InputTexture = _assets.MakeReference<Texture2D>(_button.InputTexture.Location);

		_font.SmallFont = _assets.MakeReference<SpriteFont>(_font.SmallFont.Location);
		_font.MediumFont = _assets.MakeReference<SpriteFont>(_font.MediumFont.Location);
		_font.LargeFont = _assets.MakeReference<SpriteFont>(_font.LargeFont.Location);
	}

	private void HandleRefresh()
	{
		if (!ShouldRefresh)
			return;

		Refresh();
		ShouldRefresh = false;
	}

	public static Skin FromJson(string json, Register<object> assets)
	{
		var skin = JsonSerializer.Deserialize(json, SkinDataRecordJsc.Default.DataRecord).Create(assets);
		skin.Assets = assets;

		return skin;
	}

	[Serializable]
	public struct DataRecord
	{
		[JsonIgnore]
		public static JsonSerializerOptions JsonDeserializeOptions
			=> new()
			{
				IncludeFields = true,
				WriteIndented = true,
				Converters = { }
			};

		[JsonPropertyName("button_data_normal_texture_name"), JsonInclude]
		[JsonConverter(typeof(NamespacedString.JsonConverter))]
		public NamespacedString ButtonDataNormalTextureName;

		[JsonPropertyName("button_data_hover_texture_name"), JsonInclude]
		[JsonConverter(typeof(NamespacedString.JsonConverter))]
		public NamespacedString ButtonDataHoverTextureName;

		[JsonPropertyName("button_data_input_texture_name"), JsonInclude]
		[JsonConverter(typeof(NamespacedString.JsonConverter))]
		public NamespacedString ButtonDataInputTextureName;

		[JsonPropertyName("font_data_small_font_name"), JsonInclude]
		[JsonConverter(typeof(NamespacedString.JsonConverter))]
		public NamespacedString FontDataSmallFontName;

		[JsonPropertyName("font_data_medium_font_name"), JsonInclude]
		[JsonConverter(typeof(NamespacedString.JsonConverter))]
		public NamespacedString FontDataMediumFontName;

		[JsonPropertyName("font_data_large_font_name"), JsonInclude]
		[JsonConverter(typeof(NamespacedString.JsonConverter))]
		public NamespacedString FontDataLargeFontName;

		public Skin Create(Register<object> assets)
		{
			return new Skin
			{
				Button = new ButtonData
				{
					NormalTexture = assets.MakeReference<Texture2D>(ButtonDataNormalTextureName),
					HoverTexture = assets.MakeReference<Texture2D>(ButtonDataHoverTextureName),
					InputTexture = assets.MakeReference<Texture2D>(ButtonDataInputTextureName)
				},

				Font = new FontData
				{
					SmallFont = assets.MakeReference<SpriteFont>(FontDataSmallFontName),
					MediumFont = assets.MakeReference<SpriteFont>(FontDataMediumFontName),
					LargeFont = assets.MakeReference<SpriteFont>(FontDataLargeFontName)
				}
			};
		}

		public void Read(Skin obj)
		{
			ButtonDataNormalTextureName = obj.Button.NormalTexture.Location;
			ButtonDataHoverTextureName = obj.Button.HoverTexture.Location;
			ButtonDataInputTextureName = obj.Button.InputTexture.Location;

			FontDataSmallFontName = obj.Font.SmallFont.Location;
			FontDataMediumFontName = obj.Font.MediumFont.Location;
			FontDataLargeFontName = obj.Font.LargeFont.Location;
		}
	}
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Skin.DataRecord))]
public partial class SkinDataRecordJsc : JsonSerializerContext
{

}
