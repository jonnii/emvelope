using Emvelope.Integration.Models;

namespace Emvelope.Integration
{
    public class PagingMetaProvider : IMetaProvider
    {
        public bool Wants(object content)
        {
            return content is IPagedEnumerable;
        }

        public object GetMeta(object instance)
        {
            return GetMeta((IPagedEnumerable)instance);
        }

        public object GetMeta(IPagedEnumerable instance)
        {
            return new
            {
                instance.Page,
                instance.TotalPages,
                instance.ItemsPerPage
            };
        }
    }
}