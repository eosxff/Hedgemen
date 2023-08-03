namespace Petal.Framework.Content;

public readonly struct ContentKey // maybe we should make this a class with the ability to automatically switch content?
{
	public readonly NamespacedString ContentID;
	public readonly IRegister? Register;
	public readonly object? Content;

	public ContentKey(NamespacedString contentID, IRegister? register, object? content)
	{
		ContentID = contentID;
		Register = register;
		Content = content;
	}

	public bool IsValid()
		=> Content is not null;
}
