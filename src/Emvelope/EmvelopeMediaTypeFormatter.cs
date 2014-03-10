﻿using System;
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

        private readonly EnvelopeJsonMediaTypeFormatter envelopeJsonMediaTypeFormatter;

        public EmvelopeMediaTypeFormatter()
            : this(new DefaultPluralizer())
        {
        }

        public EmvelopeMediaTypeFormatter(IPluralizer pluralizer)
        {
            jsonMediaTypeFormatter = new JsonMediaTypeFormatter();
            envelopeJsonMediaTypeFormatter = new EnvelopeJsonMediaTypeFormatter(pluralizer);

            // we should probably take these from the registered inner media type formatters
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
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

            return envelopeJsonMediaTypeFormatter;
        }

        public void AddMetaProvider(IMetaProvider provider)
        {
            envelopeJsonMediaTypeFormatter.AddMetaProvider(provider);
        }
    }
}
