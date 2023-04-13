using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Petal.Framework;

public class ContentBank
{
	public class ContentRegisteredAsyncArgs : EventArgs
	{
		public ContentValue RegisteredContent
		{
			get;
			init;
		}
	}
	
	public event EventHandler OnContentRegisteredAsync;
	
	private readonly Dictionary<NamespacedString, ContentValue> _bank = new();

	public ContentBank()
	{
		
	}

	public ContentValue Register(NamespacedString signature, object item)
	{
		var content = new ContentValue
		{
			BankedContentSignature = signature,
			Item = item
		};

		lock(_bank)
		{
			_bank.TryAdd(signature, content);
		}

		return content;
	}

	public async Task<ContentValue> RegisterAsync(NamespacedString contentName, object item)
	{
		var task = await Task.Run(() => Register(contentName, item));
		
		OnContentRegisteredAsync?.Invoke(this, new ContentRegisteredAsyncArgs
		{
			RegisteredContent = task
		});
		
		return task;
	}

	public ContentValue Get(NamespacedString signature)
	{
		lock (_bank)
		{
			if (_bank.TryGetValue(signature, out var content))
				return content;
		}

		return new ContentValue
		{
			BankedContentSignature = NamespacedString.Default,
			Item = null
		};
	}

	public ContentReference<TContent> Get<TContent>(NamespacedString signature)
		=> new (signature, this);

	public bool Replace(NamespacedString signature, object item)
	{
		lock(_bank)
		{
			if (!_bank.ContainsKey(signature))
				return false;

			var note = new ContentValue
			{
				BankedContentSignature = signature,
				Item = item
			};
			
			_bank[signature] = note;
		}

		return true;
	}
}

public readonly struct ContentValue
{
	public NamespacedString BankedContentSignature
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

	public TContent CastItemTo<TContent>()
	{
		if (Item is TContent content)
		{
			return content;
		}

		return default;
	}

	public bool CastItemTo<TContent>(out TContent content)
	{
		content = default;

		if (Item is TContent tItem)
		{
			content = tItem;
			return true;
		}

		return false;
	}
}

public sealed class ContentReference<TContent>
{
	public NamespacedString BankedContentSignature
	{
		get;
	}
	
	public TContent? Item
	{
		get;
		private set;
	}
	
	public ContentReference(NamespacedString contentName, ContentBank bank)
	{
		BankedContentSignature = contentName;
		Item = GetItemFromBank(contentName, bank);
	}

	public void ReloadItem(ContentBank bank)
	{
		Item = GetItemFromBank(BankedContentSignature, bank);
	}

	private static TContent GetItemFromBank(NamespacedString contentName, ContentBank bank)
	{
		var content = bank.Get(contentName);

		if (content.Item is TContent tContent)
		{
			return tContent;
		}

		return default;
	}

	public bool IsValid
		=> Item != null;
}