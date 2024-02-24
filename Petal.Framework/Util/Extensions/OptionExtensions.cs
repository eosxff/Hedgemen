using System.Threading.Tasks;
using Optional;
using Optional.Unsafe;

namespace Petal.Framework.Util.Extensions;

public static class OptionExtensions
{
	public static T? ValueOrNull<T>(this Option<T> self) where T : class
	{
		return self.ValueOrDefault();
	}
}