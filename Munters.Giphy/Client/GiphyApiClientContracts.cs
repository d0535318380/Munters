using System.Text.Json.Serialization;
using Munters.Giphy.Models;
using Refit;

// ReSharper disable ClassNeverInstantiated.Global

namespace Munters.Giphy.Client;


public abstract record ApiRequestBase(int Limit = 100, int Offset = 0);

public abstract record ApiResponseBase(MetaItem Meta, PaginationItem Pagination);

public sealed record MetaItem(string ResponseId, int Status, string Msg);
public sealed record PaginationItem(int TotalCount, int Count, int Offset);

public record TrendingRequest : ApiRequestBase
{
    public static readonly TrendingRequest Default = new();
};
   
public record SearchRequest(
    [property: AliasAs("q")] string Text) : ApiRequestBase;

#pragma warning disable CA1819
public record SearchResponse(MetaItem Meta, PaginationItem Pagination, GiphyItem[] Data) : ApiResponseBase(Meta, Pagination);
#pragma warning restore CA1819



