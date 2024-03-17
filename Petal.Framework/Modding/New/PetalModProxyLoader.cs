using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Petal.Framework.IO;
using Petal.Framework.Util.Logging;

namespace Petal.Framework.Modding.New;

public readonly struct PetalModProxyLoadArgs
{
	public required PetalGame Game
	{
		get;
		init;
	}

	public required bool LoadFirstPartyProxiesOnly
	{
		get;
		init;
	}

	public required IReadOnlyList<IPetalModProxy> FirstPartyProxies
	{
		get;
		init;
	}

	public required DirectoryInfo ProxyDirectory
	{
		get;
		init;
	}

	public required string ProxyManifestFileName
	{
		get;
		init;
	}
}

/// <summary>
/// Barebones .dll loader.
/// </summary>
public sealed class PetalModProxyLoader(ILogger logger)
{
	public readonly ILogger Logger = logger;

	public List<IPetalModProxy> LoadModProxies(PetalModProxyLoadArgs args)
	{
		Logger.Debug($"Loading mod proxies.");

		if(args.LoadFirstPartyProxiesOnly)
			Logger.Warn($"{nameof(args.LoadFirstPartyProxiesOnly)} is enabled. Loading only first party proxies.");

		var proxyDirectoriesQuery = args.LoadFirstPartyProxiesOnly
			? []
			: QueryProxiesFromDirectory(args);

		if(!args.LoadFirstPartyProxiesOnly)
			Logger.Info($"Number of proxies recognized in {args.ProxyDirectory.Name}: {proxyDirectoriesQuery.Count}.");

		{
			int fcpCount = args.FirstPartyProxies.Count;
			int pdqCount = proxyDirectoriesQuery.Count;
			int totalCount = fcpCount + pdqCount;
			Logger.Info($"Loading {totalCount} proxies. first party: {fcpCount}, third party: {pdqCount}.");
		}

		var proxies = new List<IPetalModProxy>(args.FirstPartyProxies.Count + proxyDirectoriesQuery.Count);

		foreach(var proxy in args.FirstPartyProxies)
			proxies.Add(proxy);

		Logger.Info("Loading third party proxies.");

		foreach(var proxyDirectory in proxyDirectoriesQuery)
		{
			if(!LoadThirdPartyProxy(proxyDirectory, args, out var proxy))
				continue;

			Logger.Info($"Loaded third party proxy {proxy.GetType().Name}.");
			proxies.Add(proxy);
		}

		Logger.Info("Awaking proxies.");

		foreach(var proxy in proxies)
		{
			if(!proxy.Awake(args))
				Logger.Warn($"Proxy {proxy.GetType().FullName} did not awake correctly.");
		}

		return proxies;
	}

	private bool LoadThirdPartyProxy(
		DirectoryInfo directory,
		PetalModProxyLoadArgs args,
		[MaybeNullWhen(false)]
		out IPetalModProxy proxy)
	{
		proxy = null;
		var manifestFile = directory.FindFile(args.ProxyManifestFileName);

		if(!manifestFile.Exists)
		{
			Logger.Error($"Could not find file. Path: '{manifestFile.FullName}'.");
			return false;
		}

		string manifestJson = manifestFile.ReadString(Encoding.UTF8);

		if(string.IsNullOrEmpty(manifestJson))
		{
			Logger.Error($"'{manifestFile.FullName}' was read as an empty string.");
			return false;
		}

		var manifest = JsonSerializer.Deserialize(manifestJson, PetalModProxyManifest.JsonTypeInfo);

		if(manifest is null)
		{
			Logger.Error($"'{manifestFile.FullName}' deserialized to null.");
			return false;
		}

		if(string.IsNullOrEmpty(manifest.DllFileName))
		{
			Logger.Error($"{nameof(manifest.DllFileName)} must not be null or empty.");
			return false;
		}

		if(string.IsNullOrEmpty(manifest.ProxyTypeFullName))
		{
			Logger.Error($"{nameof(manifest.ProxyTypeFullName)} must not be null or empty.");
			return false;
		}

		var dllFile = directory.FindFile(manifest.DllFileName);

		if(!dllFile.Exists)
		{
			Logger.Error($"Dll file '{dllFile.FullName}' does not exist.");
			return false;
		}

#pragma warning disable IL2026 // if we reach this point we're assuming it's confirmed to not be AOT compiled
		var assembly = Assembly.LoadFile(dllFile.FullName);

		if(assembly is null)
		{
			Logger.Error($"Could not create assembly from '{dllFile.FullName}'.");
			return false;
		}

		var modMainType = assembly.GetType(manifest.ProxyTypeFullName);

		if(modMainType is null)
		{
			Logger.Error($"Could not get {manifest.ProxyTypeFullName} from {assembly.FullName}");
			return false;
		}

#pragma warning disable IL2072 // if we reach this point we're assuming it's confirmed to not be AOT compiled
		var modProxy = (IPetalModProxy)Activator.CreateInstance(modMainType, true);

		if(modProxy is null)
		{
			Logger.Error($"Could not create {modMainType.FullName} from {assembly.FullName}.");
			return false;
		}

		foreach(var file in directory.GetFiles())
		{
			if(file.Extension != ".dll" || file.FullName == dllFile.FullName)
				continue;

			Logger.Debug($"Attempting to load ");
		}

		proxy = modProxy;
		return true;
	}

	private List<DirectoryInfo> QueryProxiesFromDirectory(PetalModProxyLoadArgs args)
	{
		var directory = args.ProxyDirectory;

		if(!directory.Exists)
		{
			Logger.Error($"Trying to load proxies from '{directory.FullName}', but it does not exist.");
			return [];
		}

		var validModDirectories = directory.GetDirectories(FilterDirectory);

		var validProxyDirectories = new List<DirectoryInfo>(validModDirectories.Length);

		foreach (var dir in validModDirectories)
			validProxyDirectories.Add(dir);

		return validProxyDirectories;

		bool FilterDirectory(DirectoryInfo d)
			=> d.FindFile(args.ProxyManifestFileName).Exists;
	}
}

public sealed class PetalModProxyManifest
{
	public static JsonTypeInfo<PetalModProxyManifest> JsonTypeInfo
		=> PetalModProxyManifestJsonTypeInfo.Default.PetalModProxyManifest;

	[JsonPropertyName("dll_file_name")]
	public string? DllFileName
	{
		get;
		set;
	}

	[JsonPropertyName("proxy_type_full_name")]
	public string? ProxyTypeFullName
	{
		get;
		set;
	}
}

[JsonSerializable(typeof(PetalModProxyManifest))]
[JsonSourceGenerationOptions(WriteIndented = true)]
internal partial class PetalModProxyManifestJsonTypeInfo : JsonSerializerContext
{

}