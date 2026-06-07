namespace Munters.Giphy.Client;

public record GiphyItem(
    string Type,
    string Id,
    Uri Url,
    string Slug,
    Uri BitlyGifUrl,
    Uri BitlyUrl,
    Uri EmbedUrl,
    string Username,
    string Source,
    string Title,
    string Rating,
    Uri ContentUrl,
    string SourceTld,
    Uri SourcePostUrl,
    int IsSticker,
    string ImportDatetime,
    string TrendingDatetime,
    Dictionary<string, GiphyImageInfo> Images
);