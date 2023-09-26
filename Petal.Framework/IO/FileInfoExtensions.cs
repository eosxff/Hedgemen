using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Petal.Framework.IO;

public static class FileInfoExtensions
{
	public static string GetFullNameWithoutExtension(this FileInfo self)
	{
		return self.FullName.Substring(self.FullName.LastIndexOf(self.Extension, StringComparison.InvariantCulture));
	}

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

	public static void WriteBytes(
		this FileInfo self,
		byte[] buffer,
		Encoding? encoding,
		FileMode fileMode = FileMode.Truncate)
	{
		WriteBytes(self, buffer, 0, buffer.Length, encoding, fileMode);
	}

	public static void WriteBytes(
		this FileInfo self,
		byte[] buffer,
		int index,
		int count,
		Encoding? encoding,
		FileMode fileMode = FileMode.OpenOrCreate)
	{
		encoding ??= Encoding.UTF8;

		using var writer = new BinaryWriter(self.Open(fileMode), encoding);
		writer.Write(buffer, index, count);
		writer.Flush();
	}

	public async static Task WriteBytesAsync(
		this FileInfo self,
		byte[] buffer,
		int index,
		int count,
		Encoding? encoding,
		FileMode fileMode = FileMode.OpenOrCreate)
	{
		encoding ??= Encoding.UTF8;

		await using var stream = self.Open(fileMode);
		await stream.WriteAsync(buffer, 0, buffer.Length);
	}

	public async static Task WriteBytesAsync(
		this FileInfo self,
		ReadOnlyMemory<byte> buffer,
		int index,
		int count,
		Encoding? encoding,
		FileMode fileMode = FileMode.OpenOrCreate)
	{
		encoding ??= Encoding.UTF8;

		await using var stream = self.Open(fileMode);
		await stream.WriteAsync(buffer);
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

	public async static Task<string> ReadStringAsync(
		this FileInfo self,
		Encoding? encoding,
		FileMode fileMode = FileMode.Open)
	{
		encoding ??= Encoding.UTF8;

		if (!self.Exists)
			return string.Empty;

		using var reader = new StreamReader(self.Open(fileMode), encoding);
		string readTask = await reader.ReadToEndAsync();

		return readTask;
	}

	public static string ReadStringSilently(
		this FileInfo self,
		Encoding? encoding,
		FileMode fileMode = FileMode.Open)
	{
		encoding ??= Encoding.UTF8;

		if (!self.Exists)
			return string.Empty;

		try
		{
			return ReadString(self, encoding, fileMode);
		}

		catch (Exception e)
		{
			return string.Empty;
		}
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

	public async static Task<byte[]> ReadBytesAsync(
		this FileInfo self,
		FileMode fileMode = FileMode.Open)
	{
		await using var stream = self.Open(fileMode);
		using var ms = new MemoryStream();

		await stream.CopyToAsync(ms);
		return ms.ToArray();
	}

	public static byte[] ReadBytesSilently(
		this FileInfo self,
		FileMode fileMode = FileMode.Open)
	{
		try
		{
			return ReadBytes(self, fileMode);
		}

		catch (Exception e)
		{
			return Array.Empty<byte>();
		}
	}
}
