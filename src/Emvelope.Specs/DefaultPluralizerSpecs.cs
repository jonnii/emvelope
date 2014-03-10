using Machine.Fakes;
using Machine.Specifications;

namespace Emvelope.Specs
{
    public class DefaultPluralizerSpecs
    {
        [Subject(typeof(DefaultPluralizer))]
        public class when_pluralizing : WithSubject<DefaultPluralizer>
        {
            It should_pluralize = () =>
                Subject.Pluralize("Cat").ShouldEqual("Cats");

            It should_pluralize_when_ending_with_y = () =>
                Subject.Pluralize("Battery").ShouldEqual("Batteries");
        }
    }
}
