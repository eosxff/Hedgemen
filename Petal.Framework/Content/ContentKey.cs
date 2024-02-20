namespace Petal.Framework.Content;

// maybe we should make this a class with the ability to automatically switch content?
public readonly struct ContentKey(NamespacedString location, IRegister? register, object? content)
{
	public readonly NamespacedString Location = location;
	public readonly IRegister? Register = register;
	public readonly object? Content = content;

	public bool IsValid()
		=> Content is not null;
}
