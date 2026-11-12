using System.Text.RegularExpressions;

namespace Market.Infrastructure.Common;

/// <summary>
/// Jednostavno čišćenje HTML-a iz WYSIWYG editora prije spremanja u bazu (zaštita od XSS-a).
/// Uklanja: script/style/iframe/object/embed tagove, "on..." atribute (onclick, onerror...)
/// i "javascript:" linkove. Frontend (Angular [innerHTML]) dodatno čisti HTML pri prikazu,
/// pa imamo dvostruku zaštitu.
/// </summary>
public static class HtmlSanitizer
{
    private static readonly Regex DangerousBlocks = new(
        @"<\s*(script|style|iframe|object|embed)[^>]*>.*?<\s*/\s*\1\s*>",
        RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

    private static readonly Regex DangerousSingleTags = new(
        @"<\s*/?\s*(script|style|iframe|object|embed|link|meta)[^>]*>",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex EventAttributes = new(
        @"\s+on\w+\s*=\s*(""[^""]*""|'[^']*'|[^\s>]+)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex JavascriptUrls = new(
        @"(href|src)\s*=\s*([""']?)\s*javascript:[^""'>\s]*\2",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static string Sanitize(string? html)
    {
        if (string.IsNullOrWhiteSpace(html)) return string.Empty;

        var clean = DangerousBlocks.Replace(html, string.Empty);
        clean = DangerousSingleTags.Replace(clean, string.Empty);
        clean = EventAttributes.Replace(clean, string.Empty);
        clean = JavascriptUrls.Replace(clean, "$1=\"#\"");
        return clean.Trim();
    }
}
