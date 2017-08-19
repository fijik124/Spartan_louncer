using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using _11thLauncher.Models;

namespace _11thLauncher.Converters
{
    public class JsonLaunchParameterConverter : JsonConverter
    {
        private LaunchParameter Create(Type objectType, JObject jsonObject)
        {
            var typeName = (ParameterType) (int) jsonObject["Type"];
            switch (typeName)
            {
                case ParameterType.Boolean:
                    return new LaunchParameter();
                case ParameterType.Selection:
                    return new SelectionParameter();
                case ParameterType.Text:
                    return new TextParameter();
                case ParameterType.Numerical:
                    return new NumericalParameter();
                default: return null;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(LaunchParameter).IsAssignableFrom(objectType);
        }

        public override bool CanWrite => false;

        public override object ReadJson(JsonReader reader, Type objectType,
            object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var target = Create(objectType, jsonObject);
            serializer.Populate(jsonObject.CreateReader(), target);
            return target;
        }

        public override void WriteJson(JsonWriter writer, object value,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
