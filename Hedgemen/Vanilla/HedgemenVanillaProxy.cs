using System;
using Optional;
using Petal.Framework.Modding.New;

namespace Hgm.Vanilla;

public sealed class HedgemenVanillaProxy : IPetalModProxy
{
	private static HedgemenVanillaProxy? _Instance;

	public static HedgemenVanillaProxy InstanceOrThrow
		=> (_Instance is not null) ? _Instance : throw new NullReferenceException(nameof(_Instance));

	public static Option<HedgemenVanillaProxy> Instance
		=> _Instance is not null ? Option.Some(_Instance) : Option.None<HedgemenVanillaProxy>();

	public IHomebrew ProvideHomebrew()
		=> new VanillaHomebrew();
}

public sealed class VanillaHomebrew : IHomebrew
{

}