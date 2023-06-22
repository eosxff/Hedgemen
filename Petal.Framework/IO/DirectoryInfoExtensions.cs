using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

namespace Petal.Framework.IO;

public delegate bool FileListFilter(FileInfo fileHandle);

public delegate bool DirectoryListFilter(DirectoryInfo directory);

public static class DirectoryInfoExtensions
{
	public static void Clear(this DirectoryInfo self)
	{
		if (!self.Exists)
			return;

		self.GetFiles().ToList().ForEach(e => e.Delete());
		self.GetDirectories().ToList().ForEach(e => e.Delete(true));
	}

	public static FileInfo FindFile(this DirectoryInfo self, string fileName)
	{
		return new FileInfo($"{self.FullName}/{fileName}");
	}

	public static FileInfo[] FindFiles(
		this DirectoryInfo self,
		params string[] fileNames)
	{
		var files = new List<FileInfo>(fileNames.Length);

		foreach (string fileName in fileNames)
			files.Add(FindFile(self, fileName));

		return files.ToArray();
	}

	public static DirectoryInfo FindDirectory(this DirectoryInfo self, string directoryName)
	{
		return new DirectoryInfo($"{self.FullName}/{directoryName}/");
	}

	public static DirectoryInfo[] FindDirectories(
		this DirectoryInfo self,
		params string[] directoryNames)
	{
		var directories = new List<DirectoryInfo>(directoryNames.Length);

		foreach (string directoryName in directoryNames)
			directories.Add(FindDirectory(self, directoryName));

		return directories.ToArray();
	}

	public static DirectoryInfo[] GetDirectories(
		this DirectoryInfo self,
		DirectoryListFilter filter)
	{
		var directoriesArray = self.GetDirectories();
		var directoriesList = new List<DirectoryInfo>();

		foreach (var directory in directoriesArray)
		{
			var directoryHandle = new DirectoryInfo(directory.FullName);
			if (filter(directoryHandle)) directoriesList.Add(directoryHandle);
		}

		return directoriesList.ToArray();
	}

	public static FileInfo[] GetFiles(
		this DirectoryInfo self,
		FileListFilter filter,
		bool recursive = true)
	{
		var files = new List<FileInfo>();

		switch (recursive)
		{
			case true:
				InternalListFilesRecursively(filter, self, files);
				break;
			
			case false:
				foreach (var file in self.GetFiles())
					if (filter(file))
						files.Add(file);
				break;
		}

		return files.ToArray();
	}

	public static FileInfo[] GetFilesRecursively(
		this DirectoryInfo self,
		FileListFilter? filter = null)
	{
		filter ??= _ => true;
		var files = new List<FileInfo>();

		InternalListFilesRecursively(filter, self, files);

		return files.ToArray();
	}

	public static void CopyTo(
		this DirectoryInfo self,
		DirectoryInfo dest)
	{
		FileSystem.CopyDirectory(self.FullName, dest.FullName);
	}

	private static void InternalListFilesRecursively(
		FileListFilter filter,
		DirectoryInfo directory,
		ICollection<FileInfo> files)
	{
		foreach (var file in directory.GetFiles())
			if (filter(file))
				files.Add(file);

		foreach (var dir in directory.GetDirectories())
			InternalListFilesRecursively(filter, dir, files);
	}
}