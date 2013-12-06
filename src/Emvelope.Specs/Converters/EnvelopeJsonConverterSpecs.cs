using System.Collections.Generic;
using Emvelope.Converters;
using Machine.Fakes;
using Machine.Specifications;

namespace Emvelope.Specs.Converters
{
    public class EnvelopeJsonConverterSpecs
    {
        [Subject(typeof(EnvelopeJsonConverter))]
        public class when_getting_root_key : WithSubject<EnvelopeJsonConverter>
        {
            It should_get_root_key_for_singular_item = () =>
                Subject.GetEnvelopePropertyName(typeof(Cat)).ShouldEqual("Cat");

            It should_get_root_key_for_array = () =>
                Subject.GetEnvelopePropertyName(typeof(Cat[])).ShouldEqual("Cats");

            It should_get_root_key_for_list = () =>
                Subject.GetEnvelopePropertyName(typeof(List<Cat>)).ShouldEqual("Cats");
        }

        public class Cat
        {

        }
    }
}