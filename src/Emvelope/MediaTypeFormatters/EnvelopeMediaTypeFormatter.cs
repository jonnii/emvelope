using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Emvelope.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Emvelope.MediaTypeFormatters
{
    public class EnvelopeMediaTypeFormatter : JsonMediaTypeFormatter
    {
        private readonly ConcurrentDictionary<Type, Type> envelopeTypeCache =
            new ConcurrentDictionary<Type, Type>();

        private readonly ConcurrentDictionary<Type, bool> shouldEnvelopeCache =
            new ConcurrentDictionary<Type, bool>();

        public EnvelopeMediaTypeFormatter()
        {
            SerializerSettings.ContractResolver = new SnakeCasePropertyNamesContractResolver();
            SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
            SerializerSettings.Converters.Add(new EnvelopeJsonConverter());
            SerializerSettings.Converters.Add(new WhiteSpaceTrimStringConverter());
        }

        public override Task WriteToStreamAsync(
            Type type,
            object value,
            Stream writeStream,
            HttpContent content,
            TransportContext transportContext)
        {
            // the object value is a more reliable source for the type, but if it's not available 
            // (because the controller returns null for example), fallback to using the type the 
            // formatter thinks it should be formatting...
            var serializedType = value != null
                ? value.GetType()
                : type;

            var shouldEnvelope = shouldEnvelopeCache.GetOrAdd(serializedType, ShouldEnvelope);

            var innerValue = shouldEnvelope
                ? new EnvelopeWrite(value)
                : value;

            return base.WriteToStreamAsync(type, innerValue, writeStream, content, transportContext);
        }

        public override Task<object> ReadFromStreamAsync(
            Type type,
            Stream readStream,
            HttpContent content,
            IFormatterLogger formatterLogger)
        {
            var shouldEnvelope = shouldEnvelopeCache.GetOrAdd(type, ShouldEnvelope);

            var innerType = shouldEnvelope
                ? envelopeTypeCache.GetOrAdd(type, t => typeof(EnvelopeRead<>).MakeGenericType(t))
                : type;

            return base.ReadFromStreamAsync(innerType, readStream, content, formatterLogger);
        }

        public bool ShouldEnvelope(Type type)
        {
            if (type.IsArray)
            {
                var elementType = type.GetElementType();

                if (elementType == typeof(string))
                {
                    return false;
                }

                return !elementType.IsValueType;
            }

            if (type == typeof(string))
            {
                return false;
            }

            if (type.IsValueType)
            {
                return false;
            }

            if (IsAnonymousType(type))
            {
                return false;
            }

            if (type == typeof(object))
            {
                return false;
            }

            return true;
        }

        private static bool IsAnonymousType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                   && type.IsGenericType && type.Name.Contains("AnonymousType")
                   && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                   && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }
    }
}
