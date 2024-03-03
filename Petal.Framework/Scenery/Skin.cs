using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework.Content;

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

	private PanelData _panel;

	public PanelData Panel
	{
		get
		{
			HandleRefresh();
			return _panel;
		}
		init => _panel = value;
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
				HorizontalPadding = 0,
				VerticalPadding = 0
			};

			_font = new FontData
			{
				SmallFont = assets.MakeReference<SpriteFont>(NamespacedString.Default),
				MediumFont = assets.MakeReference<SpriteFont>(NamespacedString.Default),
				LargeFont = assets.MakeReference<SpriteFont>(NamespacedString.Default),
			};

			_panel = new PanelData
			{
				PanelTexture = assets.MakeReference<Texture2D>(NamespacedString.Default),
				BorderPadding = 0
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
		public int HorizontalPadding;
		public int VerticalPadding;
	}

	public sealed class FontData
	{
		public RegistryObject<SpriteFont> SmallFont;
		public RegistryObject<SpriteFont> MediumFont;
		public RegistryObject<SpriteFont> LargeFont;
	}

	public sealed class PanelData
	{
		public RegistryObject<Texture2D> PanelTexture;
		public int BorderPadding;
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
		var skin = JsonSerializer.Deserialize(json, JsonData.JsonTypeInfo).Create(assets);
		skin.Assets = assets;
		return skin;
	}

	public async static Task<Skin> FromJsonAsync(string json, Register<object> assets)
	{
		var skin = await Task.Run(() => FromJson(json, assets));
		skin.Assets = assets;
		return skin;
	}

	[Serializable]
	public struct JsonData
	{
		public static JsonTypeInfo<JsonData> JsonTypeInfo
			=> SkinJsonDataJsonTypeInfo.Default.JsonData;

		public static JsonSerializerOptions JsonDeserializeOptions
			=> new()
			{
				IncludeFields = true,
				WriteIndented = true,
				Converters = { new NamespacedString.JsonConverter() }
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

		[JsonPropertyName("button_data_horizontal_padding"), JsonInclude]
		public int ButtonDataHorizontalPadding;

		[JsonPropertyName("button_data_vertical_padding"), JsonInclude]
		public int ButtonDataVerticalPadding;

		[JsonPropertyName("font_data_small_font_name"), JsonInclude]
		[JsonConverter(typeof(NamespacedString.JsonConverter))]
		public NamespacedString FontDataSmallFontName;

		[JsonPropertyName("font_data_medium_font_name"), JsonInclude]
		[JsonConverter(typeof(NamespacedString.JsonConverter))]
		public NamespacedString FontDataMediumFontName;

		[JsonPropertyName("font_data_large_font_name"), JsonInclude]
		[JsonConverter(typeof(NamespacedString.JsonConverter))]
		public NamespacedString FontDataLargeFontName;

		[JsonPropertyName("panel_data_texture_name"), JsonInclude]
		[JsonConverter(typeof(NamespacedString.JsonConverter))]
		public NamespacedString PanelDataPanelTextureName;

		[JsonPropertyName("panel_data_border_padding"), JsonInclude]
		public int PanelDataBorderPadding;

		public readonly Skin Create(Register<object> assets)
		{
			return new Skin
			{
				Button = new ButtonData
				{
					NormalTexture = assets.MakeReference<Texture2D>(ButtonDataNormalTextureName),
					HoverTexture = assets.MakeReference<Texture2D>(ButtonDataHoverTextureName),
					InputTexture = assets.MakeReference<Texture2D>(ButtonDataInputTextureName),
					HorizontalPadding = ButtonDataHorizontalPadding,
					VerticalPadding = ButtonDataVerticalPadding
				},

				Font = new FontData
				{
					SmallFont = assets.MakeReference<SpriteFont>(FontDataSmallFontName),
					MediumFont = assets.MakeReference<SpriteFont>(FontDataMediumFontName),
					LargeFont = assets.MakeReference<SpriteFont>(FontDataLargeFontName)
				},

				Panel = new PanelData
				{
					PanelTexture = assets.MakeReference<Texture2D>(PanelDataPanelTextureName),
					BorderPadding = PanelDataBorderPadding
				}
			};
		}

		public void Read(Skin obj)
		{
			ButtonDataNormalTextureName = obj.Button.NormalTexture.Location;
			ButtonDataHoverTextureName = obj.Button.HoverTexture.Location;
			ButtonDataInputTextureName = obj.Button.InputTexture.Location;
			ButtonDataHorizontalPadding = obj.Button.HorizontalPadding;
			ButtonDataVerticalPadding = obj.Button.VerticalPadding;

			FontDataSmallFontName = obj.Font.SmallFont.Location;
			FontDataMediumFontName = obj.Font.MediumFont.Location;
			FontDataLargeFontName = obj.Font.LargeFont.Location;

			PanelDataPanelTextureName = obj.Panel.PanelTexture.Location;
			PanelDataBorderPadding = obj.Panel.BorderPadding;
		}
	}
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Skin.JsonData))]
internal partial class SkinJsonDataJsonTypeInfo : JsonSerializerContext
{

}
