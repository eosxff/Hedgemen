using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Petal.Framework.Persistence;

public class DictionaryJsonConverter<TKey, TValue, TKeyConverter, TValueConverter>
	where TKeyConverter : JsonConverter<TKey>
	where TValueConverter : JsonConverter<TValue>
{

}
