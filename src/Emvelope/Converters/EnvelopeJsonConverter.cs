using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Emvelope.MediaTypeFormatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Emvelope.Converters
{
    public class EnvelopeJsonConverter : JsonConverter
    {
        private readonly ConcurrentDictionary<Type, string> envelopePropertyNameCache =
            new ConcurrentDictionary<Type, string>();

        public override bool CanConvert(Type objectType)
        {
            return typeof(IEnvelope).IsAssignableFrom(objectType);
        }

        public override object ReadJson(
            JsonReader reader,
            Type envelope,
            object existingValue,
            JsonSerializer serializer)
        {
            var innerEnvelopeType = envelope.GetGenericArguments()[0];

            var envelopePropertyName = envelopePropertyNameCache.GetOrAdd(
                innerEnvelopeType,
                GetEnvelopePropertyName);

            var rootElementName = SnakeCase(envelopePropertyName);

            var json = JObject.Load(reader);
            var inner = json[rootElementName];

            if (inner == null)
            {
                return null;
            }

            using (var innerReader = inner.CreateReader())
            {
                return serializer.Deserialize(innerReader, innerEnvelopeType);
            }
        }

        private string SnakeCase(string name)
        {
            return Regex.Replace(name, "([a-z])([A-Z])", "$1_$2").ToLower();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var envelope = (EnvelopeWrite)value;

            var inner = FormatEnvelope(envelope.Value);

            serializer.Serialize(writer, inner);
        }

        private object FormatEnvelope(object content)
        {
            //var paged = content as IPaged;

            var envelopePropertyName = envelopePropertyNameCache.GetOrAdd(
                content.GetType(),
                GetEnvelopePropertyName);

            var dict = new Dictionary<object, object>
            {
                { envelopePropertyName, content } 
            };

            //if (paged != null)
            //{
            //    dict.Add("Meta", new
            //    {
            //        paged.Page,
            //        paged.ItemsPerPage,
            //        paged.TotalPages
            //    });
            //}

            return dict;
        }

        public string GetEnvelopePropertyName(Type type)
        {
            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                return Pluralize(elementType.Name);
            }

            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                if (!type.GetGenericArguments().Any())
                {
                    return Pluralize(type.Name);
                }

                var arg = type.GetGenericArguments()[0];
                return Pluralize(arg.Name);
            }

            return type.Name;
        }

        private string Pluralize(string name)
        {
            return string.Concat(name, "s");
        }
    }
}