using System;
using System.Collections.Generic;

namespace Petal.Framework;

public class ContentBank
{
	private readonly Dictionary<NamespacedString, ContentValue> _bank = new();

	public ContentBank()
	{
		
	}

	public ContentValue Register(NamespacedString contentSig, object item)
	{
		var content = new ContentValue
		{
			Signature = contentSig,
			Item = item
		};

		lock(_bank)
		{
			_bank.TryAdd(contentSig, content);
		}
		
		return content;
	}

	public ContentValue Get(NamespacedString contentSig)
	{
		lock (_bank)
		{
			if (_bank.TryGetValue(contentSig, out var content))
				return content;
		}

		return new ContentValue
		{
			Signature = NamespacedString.Default,
			Item = null
		};
	}

	public ContentReference<TContent> Get<TContent>(NamespacedString contentSig)
		=> new (contentSig, this);

	public bool Replace(NamespacedString contentSig, object item)
	{
		lock(_bank)
		{
			if (!_bank.ContainsKey(contentSig))
				return false;

			var note = new ContentValue
			{
				Signature = contentSig,
				Item = item
			};
			
			_bank[contentSig] = note;
		}

		return true;
	}
}

public readonly struct ContentValue
{
	public NamespacedString Signature
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
	public NamespacedString Signature
	{
		get;
	}
	
	public TContent? Item
	{
		get;
		private set;
	}
	
	public ContentReference(NamespacedString contentSig, ContentBank bank)
	{
		Signature = contentSig;
		Item = GetItemFromBank(contentSig, bank);
	}

	public void ReloadItem(ContentBank bank)
	{
		Item = GetItemFromBank(Signature, bank);
	}

	private static TContent GetItemFromBank(NamespacedString contentSig, ContentBank bank)
	{
		var content = bank.Get(contentSig);

		if (content.Item is TContent tContent)
		{
			return tContent;
		}

		return default;
	}
}