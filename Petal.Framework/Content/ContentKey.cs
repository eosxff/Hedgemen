﻿namespace Petal.Framework.Content;

public readonly struct ContentKey
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