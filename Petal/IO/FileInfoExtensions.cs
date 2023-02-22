using System;
using System.IO;
using System.Text;

namespace Petal.Framework.IO;

public static class FileInfoExtensions
{
	public static string GetFullNameWithoutExtension(this FileInfo self)
		=> self.FullName[..self.FullName.LastIndexOf(self.Extension, StringComparison.InvariantCulture)];

	public static bool WriteString(
		this FileInfo self,
		string text,
		Encoding? encoding,
		FileMode fileMode = FileMode.Truncate)
	{
		encoding ??= Encoding.UTF8;
			
		using var writer = new StreamWriter(self.Open(fileMode), encoding);
		writer.Write(text);
		writer.Flush();
		return true;
	}
		
	public static bool WriteBytes(
		this FileInfo self,
		byte[] buffer,
		Encoding? encoding,
		FileMode fileMode = FileMode.Truncate)
		=> WriteBytes(self, buffer, 0, buffer.Length, encoding, fileMode);

	public static bool WriteBytes(
		this FileInfo self,
		byte[] buffer,
		int index,
		int count,
		Encoding? encoding,
		FileMode fileMode = FileMode.Truncate)
	{
		encoding ??= Encoding.UTF8;
			
		using var writer = new BinaryWriter(self.Open(fileMode), encoding);
		writer.Write(buffer, index, count);
		writer.Flush();
		return true;
	}
		
	public static string ReadString(
		this FileInfo self,
		Encoding? encoding,
		FileMode fileMode = FileMode.Open)
	{
		encoding ??= Encoding.UTF8;
			
		if (!self.Exists)
			return string.Empty;
			
		using var reader = new StreamReader(self.Open(fileMode), encoding);
		return reader.ReadToEnd();
	}

	public static byte[] ReadBytes(
		this FileInfo self,
		FileMode fileMode = FileMode.Open)
	{
		using var stream = self.Open(fileMode);
		using var ms = new MemoryStream();

		stream.CopyTo(ms);
		return ms.ToArray();
	}
}