namespace Petal.Framework.Modding;

public static class NamespacedStringExtensions
{
	public static T? FromContentRegistry<T>(this NamespacedString self, ContentRegistry contentRegistry)
	{
		if (contentRegistry is null)
			return default;

		contentRegistry.TryGet(self, out var contentValue);

		if (contentValue.Item is T content)
		{
			return content;
		}

		return default;
	}
}