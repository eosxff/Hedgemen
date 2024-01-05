using System.Threading.Tasks;

namespace Petal.Framework.Util.Extensions;

public static class TaskExtensions
{
	/// <summary>
	/// For clarity purposes, this exists to signify that it is intended for this <see cref="Task"/> to not be awaited.
	/// </summary>
	public static void NotAwaiting(this Task self)
	{

	}
}
