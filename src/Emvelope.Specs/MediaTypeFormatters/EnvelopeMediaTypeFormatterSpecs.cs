using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using Emvelope.MediaTypeFormatters;
using Machine.Fakes;
using Machine.Specifications;

namespace Emvelope.Specs.MediaTypeFormatters
{
    public class EnvelopeMediaTypeFormatterSpecs
    {
        [Subject(typeof(EnvelopeJsonMediaTypeFormatter))]
        public class in_general : WithSubject<EnvelopeJsonMediaTypeFormatter>
        {
            It should_envelope_reference_types = () =>
                Subject.ShouldEnvelope(typeof(Model)).ShouldBeTrue();

            It should_envelope_reference_type_arrays = () =>
                Subject.ShouldEnvelope(typeof(Model[])).ShouldBeTrue();

            It should_envelope_reference_type_enumerables = () =>
                Subject.ShouldEnvelope(typeof(IEnumerable<Model>)).ShouldBeTrue();

            It should_envelope_structs = () =>
                Subject.ShouldEnvelope(typeof(AnnoyingStruct)).ShouldBeTrue();

            It should_envelope_struct_arrays = () =>
                Subject.ShouldEnvelope(typeof(AnnoyingStruct[])).ShouldBeTrue();

            It should_not_envelope_string_arrays = () =>
                Subject.ShouldEnvelope(typeof(string[])).ShouldBeFalse();

            It should_not_envelope_strings = () =>
                Subject.ShouldEnvelope(typeof(string)).ShouldBeFalse();

            It should_not_envelope_ints = () =>
                Subject.ShouldEnvelope(typeof(int)).ShouldBeFalse();

            It should_not_envelope_date_time_arrays = () =>
                Subject.ShouldEnvelope(typeof(DateTime[])).ShouldBeFalse();

            It should_not_envelope_date_times = () =>
                Subject.ShouldEnvelope(typeof(DateTime)).ShouldBeFalse();

            It should_not_envelope_decimals = () =>
                Subject.ShouldEnvelope(typeof(decimal)).ShouldBeFalse();

            It should_not_envelope_decimal_arrays = () =>
                Subject.ShouldEnvelope(typeof(decimal[])).ShouldBeFalse();

            It should_not_envelope_anonymous_types = () =>
                Subject.ShouldEnvelope(new { foo = "zomg" }.GetType()).ShouldBeFalse();

            It should_not_envelope_object = () =>
                Subject.ShouldEnvelope(typeof(object)).ShouldBeFalse();

            It should_not_envelope_primitive_enumerables = () =>
                Subject.ShouldEnvelope(typeof(IEnumerable<int>)).ShouldBeFalse();

            It should_not_envelope_nullable_types = () =>
                Subject.ShouldEnvelope(typeof(int?)).ShouldBeFalse();

            It should_not_envelope_enumerables = () =>
                Subject.ShouldEnvelope(typeof(IEnumerable)).ShouldBeFalse();
        }

        [Subject(typeof(EnvelopeJsonMediaTypeFormatter))]
        public class when_serializing : WithSubject<EnvelopeJsonMediaTypeFormatter>
        {
            Because of = () =>
                serialized = Serialize(Subject, new Model { Name = "Fribble" });

            It should_serialize_with_root_element = () =>
                serialized.ShouldEqual(@"{""model"":{""name"":""Fribble""}}");

            static string serialized;
        }

        [Subject(typeof(EnvelopeJsonMediaTypeFormatter))]
        public class when_serializing_anonymous_type : WithSubject<EnvelopeJsonMediaTypeFormatter>
        {
            Because of = () =>
                serialized = Serialize(Subject, new { cake = new { Name = "Chocolate" } });

            It should_serialize_with_root_element = () =>
                serialized.ShouldEqual(@"{""cake"":{""name"":""Chocolate""}}");

            static string serialized;
        }

        [Subject(typeof(EnvelopeJsonMediaTypeFormatter))]
        public class when_round_tripping : WithSubject<EnvelopeJsonMediaTypeFormatter>
        {
            Because of = () =>
            {
                var serialized = Serialize(Subject, new Model { Name = "     Fribble      " });
                deserialized = Deserialize<Model>(Subject, serialized);
            };

            It should_deserialize_with_whitespace_trimmed = () =>
                deserialized.Name.ShouldEqual("Fribble");

            static Model deserialized;
        }

        public class Model
        {
            public string Name { get; set; }
        }

        public struct AnnoyingStruct
        {
            public string Name { get; set; }
        }

        private static string Serialize<T>(MediaTypeFormatter formatter, T value)
        {
            var stream = new MemoryStream();
            var content = new StreamContent(stream);

            formatter.WriteToStreamAsync(typeof(T), value, stream, content, null).Wait();

            stream.Position = 0;

            return content.ReadAsStringAsync().Result;
        }

        private static T Deserialize<T>(MediaTypeFormatter formatter, string str) where T : class
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            writer.Write(str);
            writer.Flush();

            stream.Position = 0;

            return formatter.ReadFromStreamAsync(typeof(T), stream, null, null).Result as T;
        }
    }

}
