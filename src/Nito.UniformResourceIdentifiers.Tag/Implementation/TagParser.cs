using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Nito.UniformResourceIdentifiers.Implementation
{
    /// <summary>
    /// Utility methods for parsing TAG URIs.
    /// </summary>
    public static class TagParser
    {
        private static bool CoarseParse(string uri, out string authorityName, out int year, out int? month, out int? day, out string specific, out string fragment)
        {
            authorityName = specific = fragment = null;
            year = 0;
            month = day = null;
            if (!uri.StartsWith("tag:"))
                return false;
            var entityEndIndex = uri.IndexOf(':', 4);
            if (entityEndIndex == -1)
                return false;
            var authorityNameEndIndex = uri.IndexOf(',', 4, entityEndIndex - 4);
            if (authorityNameEndIndex == -1)
                return false;
            authorityName = uri.Substring(4, authorityNameEndIndex - 4);
            if (!TagUtil.TryParseDate(uri, authorityNameEndIndex + 1, entityEndIndex - (authorityNameEndIndex + 1), out year, out month, out day))
                return false;
            var fragmentDelimiterIndex = uri.IndexOf('#', entityEndIndex + 1);
            specific = fragmentDelimiterIndex == -1 ? uri.Substring(entityEndIndex + 1) : uri.Substring(entityEndIndex + 1, fragmentDelimiterIndex - (entityEndIndex + 1));
            if (fragmentDelimiterIndex != -1)
                fragment = uri.Substring(fragmentDelimiterIndex + 1);
            return true;
        }

        /// <summary>
        /// Parses a TAG URI.
        /// </summary>
        /// <param name="uri">The URI to parse.</param>
        /// <param name="authorityName">On return, contains the authority name. This is never <c>null</c> or the empty string.</param>
        /// <param name="year">On return, contains the date year.</param>
        /// <param name="month">On return, contains the date month. May be <c>null</c>.</param>
        /// <param name="day">On return, contains the date day. May be <c>null</c>.</param>
        /// <param name="specific">On return, contains the specific string. This is never <c>null</c>, but may be the empty string.</param>
        /// <param name="fragment">On return, contains the fragment string. May be <c>null</c> or the empty string.</param>
        public static void Parse(string uri, out string authorityName, out int year, out int? month, out int? day, out string specific, out string fragment)
        {
            // Unescape unreserved characters; this is always a safe operation, and only needs to be done once because "%%" is not a valid input anyway.
            uri = Util.DecodeUnreserved(uri);

            // Coarse-parse it into sections.
            if (!CoarseParse(uri, out authorityName, out year, out month, out day, out specific, out fragment))
                throw new FormatException($"Invalid URI reference \"{uri}\".");

            // Decode and verify each one.

            authorityName = Parser.PercentDecode(authorityName, TagUtil.AuthorityNameCharIsSafe, "authority name", uri);
            if (authorityName == "")
                throw new FormatException($"Empty authority name in URI reference \"{uri}\".");
            specific = Parser.PercentDecode(specific, TagUtil.SpecificCharIsSafe, "specific", uri);
            if (fragment != null)
                fragment = Parser.PercentDecode(fragment, Util.FragmentCharIsSafe, "fragment", uri);
        }
    }
}
