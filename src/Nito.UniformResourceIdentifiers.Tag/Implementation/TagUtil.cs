using Nito.UniformResourceIdentifiers.Implementation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Nito.UniformResourceIdentifiers.Implementation
{
    /// <summary>
    /// Utilities for implementing TAG URIs.
    /// </summary>
    public static class TagUtil
    {
        private static readonly Regex DateRegex = new Regex(@"^([0-9]{4})(?:-([0-9]{2})(?:-([0-9]{2}))?)?$", RegexOptions.CultureInvariant);

        /// <summary>
        /// Attempts to parse a date string.
        /// </summary>
        /// <param name="date">The string to parse.</param>
        /// <param name="offset">The offset of <paramref name="date"/> to start parsing.</param>
        /// <param name="length">The length of <paramref name="date"/> to end parsing.</param>
        /// <param name="year">On return, contains the four-digit year.</param>
        /// <param name="month">On return, contains the two-digit month. May be <c>null</c>.</param>
        /// <param name="day">On return, contains the two-digit day. May be <c>null</c>.</param>
        public static bool TryParseDate(string date, int offset, int length, out int year, out int? month, out int? day)
        {
            year = 0;
            month = day = null;
            var dateParts = DateRegex.Match(date, offset, length);
            if (!dateParts.Success)
                return false;
            if (!int.TryParse(dateParts.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out year))
                return false;
            if (dateParts.Groups[2].Value != "")
            {
                if (!int.TryParse(dateParts.Groups[2].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var value))
                    return false;
                month = value;
            }
            if (dateParts.Groups[3].Value != "")
            {
                if (!int.TryParse(dateParts.Groups[3].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var value))
                    return false;
                day = value;
            }
            return true;
        }

        // RFC4151 (2.1) is too limited in the characters allowed for an email (as noted in the errata).
        // However, the fix proposed by the errata (as of 2017-04-27) is too lenient (allowing invalid URI characters).
        // RFC4151 (2.1) states "Future standards efforts may allow use of other authority names following syntax that is disjoint from this syntax. To allow
        //   for such developments, software that processes tags MUST NOT reject them on the grounds that they are outside the syntax defined above."
        // which is, of course, not possible in practice. One cannot parse a syntax that is undefined.
        // RFC4151 (2.1) recommends limiting percent-encoding to the `specific`/`fragment` portions: "tags SHOULD NOT be minted with percent-encoded parts.
        //   However, the tag syntax does allow percent-encoded characters in the "pchar" elements..."
        // In this case, I believe the best option is to allow percent-encoding in the `authorityName` field, in spite of the SHOULD NOT recommendation.
        // Since `authorityName` is (part of) a path segment (RFC3986 (3)), and since ":" is used as a delimiter after `taggingEntity`, this implementation uses:
        //   authorityName = segment-nz-nc
        // In other words, `authorityName` must be a non-zero-length segment without ":".
        // This is identical to:
        //   authorityName = 1*( pct-encoded / unreserved / sub-delims / "@" )
        /// <summary>
        /// A delegate for determining whether an authority name character is safe (does not require encoding).
        /// </summary>
        public static readonly Func<byte, bool> AuthorityNameCharIsSafe = x => Util.IsUnreserved(x) || Util.IsSubcomponentDelimiter(x) || x == '@';

        /// <summary>
        /// A delegate for determining whether a specific character is safe (does not require encoding).
        /// </summary>
        public static readonly Func<byte, bool> SpecificCharIsSafe = Util.QueryCharIsSafe;

        /// <summary>
        /// Formats the URI components as a complete string. Components are assumed to be already validated.
        /// </summary>
        public static string ToString(string authorityName, int year, int? month, int? day, string specific, string fragment)
        {
            var sb = new StringBuilder();
            sb.Append(TagUniformResourceIdentifier.TagScheme);
            sb.Append(':');
            sb.Append(Util.PercentEncode(authorityName, AuthorityNameCharIsSafe));
            sb.Append(',');
            sb.Append(year.ToString("D4", CultureInfo.InvariantCulture));
            if (month != null)
            {
                sb.Append('-');
                sb.Append(month.Value.ToString("D2", CultureInfo.InvariantCulture));
                if (day != null)
                {
                    sb.Append('-');
                    sb.Append(day.Value.ToString("D2", CultureInfo.InvariantCulture));
                }
            }
            sb.Append(':');
            sb.Append(Util.PercentEncode(specific, SpecificCharIsSafe));
            if (fragment != null)
            {
                sb.Append('#');
                sb.Append(Util.PercentEncode(fragment, Util.FragmentCharIsSafe));
            }
            return sb.ToString();
        }
    }
}
