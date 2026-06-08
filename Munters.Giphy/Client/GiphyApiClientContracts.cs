using Refit;

// ReSharper disable ClassNeverInstantiated.Global

namespace Munters.Giphy.Client;

public abstract record ApiRequestBase(
    [property: AliasAs("limit")] int Limit = 100,
    [property: AliasAs("offset")] int Offset = 0)
{
};

public abstract record ApiResponseBase(MetaItem Meta, PaginationItem Pagination)
{
    public bool IsSinglePage => Pagination.TotalCount == Pagination.Count;
    public bool HasPages => Pagination.TotalCount > Pagination.Count;
}

public sealed record MetaItem(string ResponseId, int Status, string Msg);

public sealed record PaginationItem(int TotalCount, int Count, int Offset)
{
    public int PagesCount => (int)Math.Ceiling((double)TotalCount / Count);
}

public record TrendingRequest(
    int Offset = 0,
    int Limit = 100) : ApiRequestBase(Limit, Offset)
{
    public static readonly TrendingRequest Default = new();
}

public record SearchRequest(
    [property: AliasAs("q")] string Text,
    int Offset = 0,
    int Limit = 100)
    : ApiRequestBase(Limit, Offset);

#pragma warning disable CA1819
public record SearchResponse(MetaItem Meta, PaginationItem Pagination, GiphyItem[] Data)
    : ApiResponseBase(Meta, Pagination)
{
};
#pragma warning restore CA1819