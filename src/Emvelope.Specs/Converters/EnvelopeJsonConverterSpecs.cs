using System.Collections.Generic;
using System.IO;
using Emvelope.Converters;
using Emvelope.MediaTypeFormatters;
using Machine.Specifications;
using Machine.Specifications.Model;
using Newtonsoft.Json;

namespace Emvelope.Specs.Converters
{
    [Subject(typeof(EnvelopeJsonConverter))]
    class EnvelopeJsonConverterSpecs
    {
        Establish context = () =>
            subject = new EnvelopeJsonConverter(new DefaultPluralizer());

        static EnvelopeJsonConverter subject;

        class when_getting_root_key
        {
            It should_get_root_key_for_singular_item = () =>
                subject.GetEnvelopePropertyName(typeof(Cat)).ShouldEqual("Cat");

            It should_get_root_key_for_array = () =>
                subject.GetEnvelopePropertyName(typeof(Cat[])).ShouldEqual("Cats");

            It should_get_root_key_for_list = () =>
                subject.GetEnvelopePropertyName(typeof(List<Cat>)).ShouldEqual("Cats");

            It should_get_root_key_for_item_with_attribute_override = () =>
                subject.GetEnvelopePropertyName(typeof(BestPet)).ShouldEqual("Dog");

            It should_get_root_key_for_array_item_with_attribute_override = () =>
                subject.GetEnvelopePropertyName(typeof(BestPet[])).ShouldEqual("Dogs");
        }

        class when_reading_with_snake_case_envelope
        {
            Establish context = () =>
            {
                reader = new JsonTextReader(new StringReader("{super_hero: {name: 'bob'}}"));
                model = new SuperHero();
            };

            Because of = () =>
                model = (SuperHero)subject.ReadJson(reader, typeof(EnvelopeRead<SuperHero>), new SuperHero(), new JsonSerializer());

            It should_observation = () =>
                model.Name.ShouldEqual("bob");

            private static JsonTextReader reader;

            private static SuperHero model;
        }

        class when_reading_with_camcel_case_envelope
        {
            Establish context = () =>
            {
                reader = new JsonTextReader(new StringReader("{superHero: {name: 'bob'}}"));
                model = new SuperHero();
            };

            Because of = () =>
                model = (SuperHero)subject.ReadJson(reader, typeof(EnvelopeRead<SuperHero>), new SuperHero(), new JsonSerializer());

            It should_observation = () =>
                model.Name.ShouldEqual("bob");

            private static JsonTextReader reader;

            private static SuperHero model;
        }

        public class Cat
        {
        }

        [Envelope("Dog")]
        public class BestPet
        {
        }

        public class SuperHero
        {
            public string Name { get; set; }
        }
    }
}