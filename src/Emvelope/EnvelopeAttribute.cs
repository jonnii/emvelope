using System;

namespace Emvelope
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class EnvelopeAttribute : Attribute
    {
        public EnvelopeAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
