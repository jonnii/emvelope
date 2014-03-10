using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        private readonly EnvelopeJsonConverter envelopeConverter;

        public EnvelopeMediaTypeFormatter(IPluralizer pluralizer)
        {
            envelopeConverter = new EnvelopeJsonConverter(pluralizer);

            SerializerSettings.ContractResolver = new SnakeCasePropertyNamesContractResolver();
            SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
            SerializerSettings.Converters.Add(new WhiteSpaceTrimStringConverter());

            SerializerSettings.Converters.Add(envelopeConverter);
        }

        public void AddMetaProvider(IMetaProvider provider)
        {
            envelopeConverter.AddMetaProvider(provider);
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
            if (type == typeof(object))
            {
                return false;
            }

            if (type == typeof(IEnumerable))
            {
                return false;
            }

            var innerType = GetInnerType(type);

            if (innerType == typeof(string))
            {
                return false;
            }

            if (innerType == typeof(DateTime))
            {
                return false;
            }

            if (innerType == typeof(decimal))
            {
                return false;
            }

            if (innerType.IsPrimitive)
            {
                return false;
            }

            if (IsAnonymousType(innerType))
            {
                return false;
            }

            return true;
        }

        private Type GetInnerType(Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }

            var underlying = Nullable.GetUnderlyingType(type);
            if (underlying != null)
            {
                return underlying;
            }

            if (type.IsGenericType
                && typeof(IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
            {
                return type.GetGenericArguments()[0];
            }

            return type;
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
