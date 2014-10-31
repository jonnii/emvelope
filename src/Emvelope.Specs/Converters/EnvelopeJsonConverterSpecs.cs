using System.Collections.Generic;
using Emvelope.Converters;
using Machine.Specifications;

namespace Emvelope.Specs.Converters
{
    public class EnvelopeJsonConverterSpecs
    {
        [Subject(typeof(EnvelopeJsonConverter))]
        public class when_getting_root_key : with_converter
        {
            It should_get_root_key_for_singular_item = () =>
                Subject.GetEnvelopePropertyName(typeof(Cat)).ShouldEqual("Cat");

            It should_get_root_key_for_array = () =>
                Subject.GetEnvelopePropertyName(typeof(Cat[])).ShouldEqual("Cats");

            It should_get_root_key_for_list = () =>
                Subject.GetEnvelopePropertyName(typeof(List<Cat>)).ShouldEqual("Cats");

            It should_get_root_key_for_item_with_attribute_override = () =>
                Subject.GetEnvelopePropertyName(typeof(BestPet)).ShouldEqual("Dog");

            It should_get_root_key_for_array_item_with_attribute_override = () =>
                Subject.GetEnvelopePropertyName(typeof(BestPet[])).ShouldEqual("Dogs");
        }

        public class with_converter
        {
            Establish context = () =>
                Subject = new EnvelopeJsonConverter(
                    new DefaultPluralizer());

            protected static EnvelopeJsonConverter Subject;
        }

        public class Cat
        {

        }

        [Envelope("Dog")]
        public class BestPet
        {

        }
    }
}