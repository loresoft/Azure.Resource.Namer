using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace AzureNamer.Shared.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Truncates the specified text.
    /// </summary>
    /// <param name="text">The text to truncate.</param>
    /// <param name="keep">The number of characters to keep.</param>
    /// <param name="ellipsis">The ellipsis string to use when truncating. (Default ...)</param>
    /// <returns>
    /// A truncate string.
    /// </returns>
    public static string Truncate(this string? text, int keep, string ellipsis = "...")
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        if (string.IsNullOrEmpty(ellipsis))
            ellipsis = string.Empty;

        if (text.Length <= keep)
            return text;

        if (text.Length <= keep + ellipsis.Length || keep < ellipsis.Length)
            return text.Substring(0, keep);

        return string.Concat(text.Substring(0, keep - ellipsis.Length), ellipsis);
    }

    /// <summary>
    /// Indicates whether the specified String object is null or an empty string
    /// </summary>
    /// <param name="item">A String reference</param>
    /// <returns>
    ///     <c>true</c> if is null or empty; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? item)
    {
        return string.IsNullOrEmpty(item);
    }

    /// <summary>
    /// Indicates whether a specified string is null, empty, or consists only of white-space characters
    /// </summary>
    /// <param name="item">A String reference</param>
    /// <returns>
    ///      <c>true</c> if is null or empty; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? item)
    {
        if (item == null)
            return true;

        for (int i = 0; i < item.Length; i++)
            if (!char.IsWhiteSpace(item[i]))
                return false;

        return true;
    }

    /// <summary>
    /// Determines whether the specified string is not <see cref="IsNullOrEmpty"/>.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>
    ///   <c>true</c> if the specified <paramref name="value"/> is not <see cref="IsNullOrEmpty"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasValue([NotNullWhen(true)] this string? value)
    {
        return !string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// Appends a copy of the specified string followed by the default line terminator to the end of the StringBuilder object.
    /// </summary>
    /// <param name="sb">The StringBuilder instance to append to.</param>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static StringBuilder AppendLine(this StringBuilder sb, string format, params object[] args)
    {
        sb.AppendFormat(format, args);
        sb.AppendLine();
        return sb;
    }

    /// <summary>
    /// Appends a copy of the specified string if <paramref name="condition"/> is met.
    /// </summary>
    /// <param name="sb">The StringBuilder instance to append to.</param>
    /// <param name="text">The string to append.</param>
    /// <param name="condition">The condition delegate to evaluate. If condition is null, String.IsNullOrWhiteSpace method will be used.</param>
    public static StringBuilder AppendIf(this StringBuilder sb, string text, Func<string, bool> condition = null)
    {
        var c = condition ?? (s => !string.IsNullOrWhiteSpace(s));

        if (c(text))
            sb.Append(text);

        return sb;
    }

    /// <summary>
    /// Appends a copy of the specified string if <paramref name="condition"/> is met.
    /// </summary>
    /// <param name="sb">The StringBuilder instance to append to.</param>
    /// <param name="text">The string to append.</param>
    /// <param name="condition">The condition.</param>
    public static StringBuilder AppendIf(this StringBuilder sb, string text, bool condition)
    {
        if (condition)
            sb.Append(text);

        return sb;
    }

    /// <summary>
    /// Appends a copy of the specified string followed by the default line terminator if <paramref name="condition"/> is met.
    /// </summary>
    /// <param name="sb">The StringBuilder instance to append to.</param>
    /// <param name="text">The string to append.</param>
    /// <param name="condition">The condition delegate to evaluate. If condition is null, String.IsNullOrWhiteSpace method will be used.</param>
    public static StringBuilder AppendLineIf(this StringBuilder sb, string text, Func<string, bool> condition = null)
    {
        var c = condition ?? (s => !string.IsNullOrWhiteSpace(s));

        if (c(text))
            sb.AppendLine(text);

        return sb;
    }

    public static string ToTitle(this string text)
    {
        if (string.IsNullOrWhiteSpace(text) || text.Length < 2)
            return text;

        var words = Regex.Matches(text, "([A-Z][a-z]*)|([0-9]+)");
        return string.Join(" ", words.Select(w => w.Value));
    }
}
