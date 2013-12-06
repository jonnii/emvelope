using System;
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
        [Subject(typeof(EnvelopeMediaTypeFormatter))]
        public class in_general : WithSubject<EnvelopeMediaTypeFormatter>
        {
            It should_read_reference_types_wrapped = () =>
                Subject.ShouldEnvelope(typeof(Model)).ShouldBeTrue();

            It should_read_reference_type_arrays_wrapped = () =>
                Subject.ShouldEnvelope(typeof(Model[])).ShouldBeTrue();

            It should_not_read_string_arrays_wrapped = () =>
                Subject.ShouldEnvelope(typeof(string[])).ShouldBeFalse();

            It should_not_read_string_wrapped = () =>
                Subject.ShouldEnvelope(typeof(string)).ShouldBeFalse();

            It should_not_read_int_wrapped = () =>
                Subject.ShouldEnvelope(typeof(int)).ShouldBeFalse();

            It should_not_read_value_type_arrays_wrapped = () =>
                Subject.ShouldEnvelope(typeof(DateTime[])).ShouldBeFalse();

            It should_not_read_anonymous_type_wrapped = () =>
                Subject.ShouldEnvelope(new { foo = "zomg" }.GetType()).ShouldBeFalse();

            It should_not_read_object_wrapped = () =>
                Subject.ShouldEnvelope(typeof(object)).ShouldBeFalse();
        }

        [Subject(typeof(EnvelopeMediaTypeFormatter))]
        public class when_serializing : WithSubject<EnvelopeMediaTypeFormatter>
        {
            Because of = () =>
                serialized = Serialize(Subject, new Model { Name = "Fribble" });

            It should_serialize_with_root_element = () =>
                serialized.ShouldEqual(@"{""model"":{""name"":""Fribble""}}");

            static string serialized;
        }

        [Subject(typeof(EnvelopeMediaTypeFormatter))]
        public class when_serializing_anonymous_type : WithSubject<EnvelopeMediaTypeFormatter>
        {
            Because of = () =>
                serialized = Serialize(Subject, new { cake = new { Name = "Chocolate" } });

            It should_serialize_with_root_element = () =>
                serialized.ShouldEqual(@"{""cake"":{""name"":""Chocolate""}}");

            static string serialized;
        }

        [Subject(typeof(EnvelopeMediaTypeFormatter))]
        public class when_round_tripping : WithSubject<EnvelopeMediaTypeFormatter>
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
