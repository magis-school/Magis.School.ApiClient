using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Magis.School.ApiClient.Serialization
{
    internal class JsonSettings : JsonSerializerSettings
    {
        internal JsonSettings()
        {
            ApplyTo(this);
        }

        internal static void ApplyTo(JsonSerializerSettings existingSettings)
        {
            if (existingSettings == null)
                throw new ArgumentNullException(nameof(existingSettings));

            existingSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            existingSettings.Formatting = Formatting.Indented;
            existingSettings.TypeNameHandling = TypeNameHandling.None;
            existingSettings.NullValueHandling = NullValueHandling.Include;
        }
    }
}
