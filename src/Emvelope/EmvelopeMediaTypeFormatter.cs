using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Emvelope.MediaTypeFormatters;

namespace Emvelope
{
    public class EmvelopeMediaTypeFormatter : MediaTypeFormatter
    {
        private readonly JsonMediaTypeFormatter jsonMediaTypeFormatter;

        private readonly EnvelopeMediaTypeFormatter envelopeMediaTypeFormatter;

        public EmvelopeMediaTypeFormatter()
            : this(new DefaultPluralizer())
        {
        }

        public EmvelopeMediaTypeFormatter(IPluralizer pluralizer)
        {
            jsonMediaTypeFormatter = new JsonMediaTypeFormatter();
            envelopeMediaTypeFormatter = new EnvelopeMediaTypeFormatter(pluralizer);
        }

        public override bool CanReadType(Type type)
        {
            return true;
        }

        public override bool CanWriteType(Type type)
        {
            return true;
        }

        public override MediaTypeFormatter GetPerRequestFormatterInstance(Type type, HttpRequestMessage request, MediaTypeHeaderValue mediaType)
        {
            var pairs = request.GetQueryNameValuePairs();
            if (pairs.Any(p => p.Key == "envelope" && p.Value == "false"))
            {
                return jsonMediaTypeFormatter;
            }

            return envelopeMediaTypeFormatter;
        }

        public void AddMetaProvider(IMetaProvider provider)
        {
            envelopeMediaTypeFormatter.AddMetaProvider(provider);
        }
    }
}
