using System;
using System.Dynamic;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Infrastructure.Common.Extensions
{
    public static class JsonExtensions
    {
        public static string ToJson(this object obj)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var result = JsonSerializer.Serialize(obj, jsonOptions);
            return result;
        }
    }
}
