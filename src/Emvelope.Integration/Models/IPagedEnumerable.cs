namespace Emvelope.Integration.Models
{
    public interface IPagedEnumerable
    {
        int Page { get; }

        int ItemsPerPage { get; }

        int TotalPages { get; }
    }
}