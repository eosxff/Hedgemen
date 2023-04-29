using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Petal.Framework.Util;

namespace Petal.Framework;

public class ContentRegistry
{
	public class ContentRegisteredAsyncArgs : EventArgs
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
	
	public event EventHandler? OnContentRegisteredAsync;
	public event EventHandler? OnContentRegistered;
	public event EventHandler? OnContentReplaced;
	
	private readonly Dictionary<NamespacedString, ContentValue> _registry = new();

	public ContentRegistry()
	{
		
	}

	public ContentValue Register(NamespacedString identifier, object item)
	{
		var content = new ContentValue
		{
			ContentIdentifier = identifier,
			Item = item
		};

		lock(_registry)
		{
			_registry.TryAdd(identifier, content);
		}
		
		OnContentRegistered?.Invoke(this, EventArgs.Empty);

		return content;
	}

	public async Task<ContentValue> RegisterAsync(NamespacedString identifier, object item)
	{
		var content = await Task.Run(() => Register(identifier, item));
		
		OnContentRegisteredAsync?.Invoke(this, new ContentRegisteredAsyncArgs
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
			ContentIdentifier = NamespacedString.Default,
			Item = null
		};
	}

	public bool TryGet(NamespacedString identifier, out ContentValue content)
	{
		lock (_registry)
		{
			var result = _registry.TryGetValue(identifier, out content);
			return result;
		}
	}

	public ContentReference<TContent> Get<TContent>(NamespacedString identifier)
		=> new (identifier, this);

	public bool Replace(NamespacedString identifier, object item)
	{
		lock(_registry)
		{
			if (!_registry.ContainsKey(identifier))
				return false;

			var replacedRegister = _registry[identifier];

			var register = new ContentValue
			{
				ContentIdentifier = identifier,
				Item = item
			};
			
			_registry[identifier] = register;
			
			OnContentReplaced?.Invoke(this, new ContentReplacedArgs
			{
				ReplacedContent = replacedRegister,
				RegisteredContent = register
			});
		}

		return true;
	}
}

public readonly struct ContentValue
{
	public NamespacedString ContentIdentifier
	{
		get;
		init;
	} = NamespacedString.Default;

	public object Item
	{
		get;
		init;
	} = new ();

	public ContentValue()
	{
		
	}

	public override string ToString()
		=> $"{{ContentIdentifier: {ContentIdentifier} Item: {Item}}}";
}

public sealed class ContentReference<TContent>
{
	public NamespacedString ContentIdentifier
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
		ContentIdentifier = identifier;
	}

	public ContentReference(NamespacedString identifier, ContentRegistry? registry)
	{
		ContentIdentifier = identifier;
		Item = GetItemFromRegistry(identifier, registry);
	}

	public void ReloadItem(ContentRegistry? registry)
	{
		if (registry is null)
			return;
		Item = GetItemFromRegistry(ContentIdentifier, registry);
	}

	private static TContent GetItemFromRegistry(NamespacedString identifier, ContentRegistry? registry)
	{
		if (registry is null)
			return default;
		
		registry.TryGet(identifier, out var content);

		if (content.Item is TContent tContent)
		{
			return tContent;
		}

		return default;
	}

	public bool HasItem
		=> Item is not null;
	
	[Serializable]
	public struct DataRecord : IDataRecord<ContentReference<TContent>>
	{
		[JsonPropertyName("content_id")]
		public NamespacedString ContentIdentifier;

		public ContentReference<TContent> Create()
		{
			return new ContentReference<TContent>(ContentIdentifier);
		}

		public void Read(ContentReference<TContent> obj)
		{
			ContentIdentifier = obj.ContentIdentifier;
		}
	}
}