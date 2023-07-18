﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework.Content;
using Petal.Framework.Util;

namespace Petal.Framework.Scenery;

public sealed class Skin
{
	public event EventHandler? OnSkinRefreshed;

	private Register<object> _assets;

	public Register<object> Assets
	{
		get => _assets;
		set
		{
			if (_assets is not null)
			{
				//_registry.OnContentReplaced -= RegistryShouldRefreshOnReplaced;
				//_registry.OnContentRemoved -= RegistryShouldRefreshOnRemoved;
			}

			_assets = value;

			if (_assets is not null)
			{
				//_registry.OnContentReplaced += RegistryShouldRefreshOnReplaced;
				//_registry.OnContentRemoved += RegistryShouldRefreshOnRemoved;
			}

			Refresh();
		}
	}

	private void RegistryShouldRefreshOnReplaced(object? sender, EventArgs args)
	{
		if (sender is not Registry registry)
			return;

		/*if (ContentIDMatchesAny(args.ReplacedContent.ContentID))
		{
			ShouldRefresh = true;
		}*/
	}

	private void RegistryShouldRefreshOnRemoved(object? sender, EventArgs args)
	{
		if (sender is not Registry registry)
			return;

		/*if (ContentIDMatchesAny(args.RemovedContent.ContentID))
		{
			ShouldRefresh = true;
		}*/
	}

	private bool ContentIDMatchesAny(NamespacedString contentID)
	{
		/*return contentID == Button.HoverTexture.ContentID ||
		       contentID == Button.InputTexture.ContentID ||
		       contentID == Button.NormalTexture.ContentID ||
		       contentID == Font.SmallFont.ContentID ||
		       contentID == Font.MediumFont.ContentID ||
		       contentID == Font.LargeFont.ContentID;*/

		return false;
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
				HoverTexture = assets.CreateRegistryObject<Texture2D>(NamespacedString.Default),
				InputTexture = assets.CreateRegistryObject<Texture2D>(NamespacedString.Default),
				NormalTexture = assets.CreateRegistryObject<Texture2D>(NamespacedString.Default),
			};

			_font = new FontData()
			{
				SmallFont = assets.CreateRegistryObject<SpriteFont>(NamespacedString.Default),
				MediumFont = assets.CreateRegistryObject<SpriteFont>(NamespacedString.Default),
				LargeFont = assets.CreateRegistryObject<SpriteFont>(NamespacedString.Default),
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

		_button.NormalTexture = _assets.CreateRegistryObject<Texture2D>(_button.NormalTexture.Key.ContentID);
		_button.HoverTexture = _assets.CreateRegistryObject<Texture2D>(_button.HoverTexture.Key.ContentID);
		_button.InputTexture = _assets.CreateRegistryObject<Texture2D>(_button.InputTexture.Key.ContentID);

		_font.SmallFont = _assets.CreateRegistryObject<SpriteFont>(_font.SmallFont.Key.ContentID);
		_font.MediumFont = _assets.CreateRegistryObject<SpriteFont>(_font.MediumFont.Key.ContentID);
		_font.LargeFont = _assets.CreateRegistryObject<SpriteFont>(_font.LargeFont.Key.ContentID);
	}

	private void HandleRefresh()
	{
		if (!ShouldRefresh)
			return;

		Refresh();
		ShouldRefresh = false;

		OnSkinRefreshed?.Invoke(this, EventArgs.Empty);
	}

	public static Skin FromJson(string json, Register<object> assets)
	{
		var skin = JsonSerializer.Deserialize(json, SkinDataRecordJsc.Default.DataRecord).Create(assets);
		skin.Assets = assets;

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

		public Skin Create(Register<object> assets)
		{
			return new Skin
			{
				Button = new ButtonData
				{
					NormalTexture = assets.CreateRegistryObject<Texture2D>(ButtonDataNormalTextureName),
					HoverTexture = assets.CreateRegistryObject<Texture2D>(ButtonDataHoverTextureName),
					InputTexture = assets.CreateRegistryObject<Texture2D>(ButtonDataInputTextureName)
				},

				Font = new FontData
				{
					SmallFont = assets.CreateRegistryObject<SpriteFont>(FontDataSmallFontName),
					MediumFont = assets.CreateRegistryObject<SpriteFont>(FontDataMediumFontName),
					LargeFont = assets.CreateRegistryObject<SpriteFont>(FontDataLargeFontName)
				}
			};
		}

		public void Read(Skin obj)
		{
			ButtonDataNormalTextureName = obj.Button.NormalTexture.Key.ContentID;
			ButtonDataHoverTextureName = obj.Button.HoverTexture.Key.ContentID;
			ButtonDataInputTextureName = obj.Button.InputTexture.Key.ContentID;

			FontDataSmallFontName = obj.Font.SmallFont.Key.ContentID;
			FontDataMediumFontName = obj.Font.MediumFont.Key.ContentID;
			FontDataLargeFontName = obj.Font.LargeFont.Key.ContentID;
		}
	}
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Skin.DataRecord))]
public partial class SkinDataRecordJsc : JsonSerializerContext
{

}
