namespace Emvelope
{
    public class DefaultPluralizer : IPluralizer
    {
        public string Pluralize(string name)
        {
            if (name.EndsWith("y"))
            {
                return name.Substring(0, name.Length - 1) + "ies";
            }

            return string.Concat(name, "s");
        }
    }
}