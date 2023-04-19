using System.Text;
using System.Text.Json;

namespace Vayosoft.Utilities
{
    public static class JsonExtensions
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        /// <summary>
        /// Deserialize object from json with JsonNet
        /// </summary>
        /// <typeparam name="T">Type of the deserialized object</typeparam>
        /// <param name="json">json string</param>
        /// <returns>deserialized object</returns>
        public static T FromJson<T>(this string json)
        {
            return JsonSerializer.Deserialize<T>(json, Options);
        }

        /// <summary>
        /// Serialize object to json with JsonNet
        /// </summary>
        /// <param name="obj">object to serialize</param>
        /// <returns>json string</returns>
        public static string ToJson(this object obj, bool disablePolicy = false, bool disableIndent = true)
        {
            JsonSerializerOptions o = null;
            if (disablePolicy || disableIndent)
            {
                o = new JsonSerializerOptions();
                if (disablePolicy)
                    o.PropertyNamingPolicy = null;

                if (disableIndent)
                {
                    o.WriteIndented = false;
                    //o.fo
                }
            }

            return JsonSerializer.Serialize(obj, o);
        }
        
        /// <summary>
        /// Serialize object to json with JsonNet
        /// </summary>
        /// <param name="obj">object to serialize</param>
        /// <returns>json string</returns>
        public static StringContent ToJsonStringContent(this object obj)
        {
            return new StringContent(obj.ToJson(), Encoding.UTF8, "application/json");
        }
    }
}
