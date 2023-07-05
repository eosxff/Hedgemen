using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Petal.Framework.Util;

namespace Petal.Framework;

public class ContentRegistry
{
	public class ContentRegisteredArgs : EventArgs
	{
		public ContentValue RegisteredContent
		{
			get;
			init;
		}
	}

	public class ContentReplacedArgs : EventArgs
	{
		public ContentValue ReplacedContent
		{
			get;
			init;
		}

		public ContentValue RegisteredContent
		{
			get;
			init;
		}
	}

	public event EventHandler<ContentRegisteredArgs> OnContentRegistered;
	public event EventHandler<ContentReplacedArgs> OnContentReplaced;

	private readonly Dictionary<NamespacedString, ContentValue> _registry = new();

	public ContentValue Register(NamespacedString identifier, object item)
	{
		var content = new ContentValue
		{
			ContentID = identifier,
			Item = item
		};

		lock (_registry)
		{
			_registry.TryAdd(identifier, content);
		}

		OnContentRegistered?.Invoke(this, new ContentRegisteredArgs
		{
			RegisteredContent = content
		});

		return content;
	}

	public async Task<ContentValue> RegisterAsync(NamespacedString identifier, object item)
	{
		var content = await Task.Run(() => Register(identifier, item));

		OnContentRegistered?.Invoke(this, new ContentRegisteredArgs
		{
			RegisteredContent = content
		});

		return content;
	}

	public ContentValue Get(NamespacedString identifier)
	{
		lock (_registry)
		{
			if (_registry.TryGetValue(identifier, out var content))
				return content;
		}

		return new ContentValue
		{
			ContentID = NamespacedString.Default,
			Item = null
		};
	}

	public bool TryGet(NamespacedString identifier, out ContentValue content)
	{
		lock (_registry)
		{
			bool result = _registry.TryGetValue(identifier, out content);
			return result;
		}
	}

	public ContentReference<TContent> Get<TContent>(NamespacedString identifier)
	{
		return new ContentReference<TContent>(identifier, this);
	}

	public bool Replace(NamespacedString identifier, object item)
	{
		lock (_registry)
		{
			if (!_registry.ContainsKey(identifier))
				return false;

			var replacedContent = _registry[identifier];

			var content = new ContentValue
			{
				ContentID = identifier,
				Item = item
			};
			
			_registry.ChangeValue(identifier, content);

			OnContentReplaced?.Invoke(this, new ContentReplacedArgs
			{
				ReplacedContent = replacedContent,
				RegisteredContent = content
			});
		}

		return true;
	}
}

public readonly struct ContentValue
{
	public NamespacedString ContentID
	{
		get;
		init;
	} = NamespacedString.Default;

	public object Item
	{
		get;
		init;
	} = new();

	public ContentValue()
	{
	}

	public override string ToString()
	{
		return $"{{ContentID: {ContentID} Item: {Item}}}";
	}
}

public sealed class ContentReference<TContent>
{
	public NamespacedString ContentID
	{
		get;
	}

	public TContent? Item
	{
		get;
		private set;
	}

	public ContentReference(NamespacedString identifier)
	{
		ContentID = identifier;
	}

	public ContentReference(NamespacedString identifier, ContentRegistry? registry)
	{
		ContentID = identifier;
		Item = GetItemFromRegistry(identifier, registry);
	}

	public void ReloadItem(ContentRegistry? registry)
	{
		if (registry is null)
			return;
		
		Item = GetItemFromRegistry(ContentID, registry);
	}

	private static TContent GetItemFromRegistry(NamespacedString identifier, ContentRegistry? registry)
	{
		if (registry is null)
			return default;

		registry.TryGet(identifier, out var content);

		if (content.Item is TContent tContent)
			return tContent;

		return default;
	}

	public bool HasItem
		=> Item is not null;

	[Serializable]
	public struct DataRecord : IDataRecord<ContentReference<TContent>>
	{
		[JsonPropertyName("content_id")]
		public NamespacedString ContentID;

		public ContentReference<TContent> Create()
		{
			return new ContentReference<TContent>(ContentID);
		}

		public void Read(ContentReference<TContent> obj)
		{
			ContentID = obj.ContentID;
		}
	}
}