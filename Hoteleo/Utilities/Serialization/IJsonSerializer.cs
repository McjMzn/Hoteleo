using System.Text.Json;

namespace Hoteleo.Utilities.Serialization
{
    internal interface IJsonSerializer
    {
        public string Serialize(object toSerialize);

        public T Deserialize<T>(string json);
    }

    internal class JsonSerializer : IJsonSerializer
    {
        private JsonSerializerOptions _serializerOptions;

        public JsonSerializer()
        {
            _serializerOptions = new JsonSerializerOptions
            {
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            _serializerOptions.Converters.Add(new DateTimeConverter("yyyyMMdd"));
        }

        public T Deserialize<T>(string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(json, _serializerOptions);
        }

        public string Serialize(object toSerialize)
        {
            return System.Text.Json.JsonSerializer.Serialize(toSerialize, _serializerOptions);
        }
    }
}
