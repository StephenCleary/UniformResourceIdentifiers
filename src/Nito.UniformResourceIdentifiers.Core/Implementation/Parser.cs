using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Nito.UniformResourceIdentifiers.Implementation
{
    /// <summary>
    /// Provides utility methods for parsing URIs.
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// See Appendix B, except we make use of a non-capturing group for the scheme.
        /// </summary>
        // scheme (group 1) = (?:([^:/?#]+):)?
        // authority (groups 2-3) = (//([^/?#]*))?
        // path (group 4) = ([^?#]*)
        // query (groups 5-6) = (\?([^#]*))?
        // fragment (groups 7-8) = (#(.*))?
        private static readonly Regex UriReferenceRegex = new Regex(@"^(?:([^:/?#]+):)?(//([^/?#]*))?([^?#]*)(\?([^#]*))?(#(.*))?$", RegexOptions.CultureInvariant);

        /// <summary>
        /// Breaks a URI reference into its components, without any decoding or verification of them.
        /// </summary>
        /// <param name="uriReference">The string to parse.</param>
        /// <param name="scheme">On return, contains the scheme. May be <c>null</c>, but cannot be the empty string.</param>
        /// <param name="authority">On return, contains the authority. May be <c>null</c> or the empty string.</param>
        /// <param name="path">On return, contains the path. May be the empty string, but cannot be <c>null</c>.</param>
        /// <param name="query">On return, contains the query. May be <c>null</c> or the empty string.</param>
        /// <param name="fragment">On return, contains the fragment. May be <c>null</c> or the empty string.</param>
        public static bool TryCoarseParseUriReference(string uriReference, out string? scheme, out string? authority, out string path, out string? query, out string? fragment)
        {
            path = "";
            scheme = authority = query = fragment = null;
            var match = UriReferenceRegex.Match(uriReference);
            if (!match.Success)
                return false;
            scheme = match.Groups[1].Value.Length == 0 ? null : match.Groups[1].Value;
            authority = match.Groups[2].Value.Length == 0 ? null : match.Groups[3].Value;
            path = match.Groups[4].Value;
            query = match.Groups[5].Value.Length == 0 ? null : match.Groups[6].Value;
            fragment = match.Groups[7].Value.Length == 0 ? null : match.Groups[8].Value;
            return true;
        }

        /// <summary>
        /// Breaks an authority into its components, without any decoding or verification of them.
        /// </summary>
        /// <param name="authority">The authority. May be <c>null</c> or the empty string.</param>
        /// <param name="userInfo">On return, contains the user info. May be <c>null</c> or the empty string.</param>
        /// <param name="host">On return, contains the host. May be <c>null</c> or the empty string.</param>
        /// <param name="port">On return, contains the port. May be <c>null</c> or the empty string.</param>
        public static void CoarseParseAuthority(string? authority, out string? userInfo, out string? host, out string? port)
        {
            userInfo = host = port = null;
            if (authority == null)
                return;

            // Strip off the UserInfo, which can never contain '@'.
            var atDelimiter = authority.IndexOf('@');
            if (atDelimiter != -1)
            {
                userInfo = authority.Substring(0, atDelimiter);
                authority = authority.Substring(atDelimiter + 1);
            }

            // Strip off the Port.
            int colonDelimiter;
            if (authority.StartsWith("[", StringComparison.Ordinal))
            {
                // If the host starts with '[', then it can contain ':', so we have to first find the ']' and only look for a ':' after that.
                var closeBracketIndex = authority.LastIndexOf(']');
                if (closeBracketIndex == -1)
                    closeBracketIndex = authority.Length - 1;
                colonDelimiter = authority.IndexOf(':', closeBracketIndex + 1);
            }
            else
            {
                // If the host doesn't start with '[', then it can't contain ':'.
                colonDelimiter = authority.LastIndexOf(':');
            }
            if (colonDelimiter != -1)
            {
                port = authority.Substring(colonDelimiter + 1);
                authority = authority.Substring(0, colonDelimiter);
            }

            // All that remains is the Host.
            host = authority;
        }

        /// <summary>
        /// Breaks a URI reference into its components. Performs decoding and verification of them.
        /// </summary>
        /// <param name="uriReference">The string to parse.</param>
        /// <param name="scheme">On return, contains the scheme. May be <c>null</c>, but cannot be the empty string.</param>
        /// <param name="userInfo">On return, contains the user info. May be <c>null</c> or the empty string.</param>
        /// <param name="host">On return, contains the host. May be <c>null</c> or the empty string.</param>
        /// <param name="port">On return, contains the port. May be <c>null</c> or the empty string.</param>
        /// <param name="pathSegments">On return, contains the path segments. May be empty, but cannot be <c>null</c>.</param>
        /// <param name="query">On return, contains the query. May be <c>null</c> or the empty string.</param>
        /// <param name="fragment">On return, contains the fragment. May be <c>null</c> or the empty string.</param>
        public static void ParseUriReference(string uriReference, out string? scheme, out string? userInfo, out string? host, out string? port, out IReadOnlyList<string> pathSegments, out string? query, out string? fragment)
        {
            // Unescape unreserved characters; this is always a safe operation, and only needs to be done once because "%%" is not a valid input anyway.
            uriReference = Utility.DecodeUnreserved(uriReference);

            // Coarse-parse it into sections.
            var isUri = TryCoarseParseUriReference(uriReference, out scheme, out var authority, out var path, out query, out fragment);
            CoarseParseAuthority(authority, out userInfo, out host, out port);
            if (!isUri)
                throw new FormatException($"Invalid URI reference \"{uriReference}\".");

            // Decode and verify each one.

            if (scheme != null && !Utility.IsValidScheme(scheme))
                throw new FormatException($"Invalid scheme \"{scheme}\" in URI reference \"{uriReference}\".");
            if (userInfo != null)
                userInfo = PercentDecode(userInfo, Utility.UserInfoCharIsSafe, "user info", uriReference);
            if (host != null)
                host = Utility.HostIsIpAddress(host) ? host : PercentDecode(host, Utility.HostRegNameCharIsSafe, "host", uriReference);
            if (port != null && !Utility.IsValidPort(port))
                throw new FormatException($"Invalid port \"{port}\" in URI reference \"{uriReference}\".");
            pathSegments = path.Split('/').Select(x => PercentDecode(x, Utility.PathSegmentCharIsSafe, "path segment", uriReference)).ToList();
            if (query != null)
                query = PercentDecode(query, Utility.QueryCharIsSafe, "query", uriReference);
            if (fragment != null)
                fragment = PercentDecode(fragment, Utility.FragmentCharIsSafe, "fragment", uriReference);
        }

        /// <summary>
        /// Breaks a URI reference into its components. Performs decoding and verification of them.
        /// </summary>
        /// <param name="uriReference">The string to parse.</param>
        public static UriParseResult ParseUriReference(string uriReference)
        {
            var result = new UriParseResult();
            ParseUriReference(uriReference, out result.Scheme, out result.UserInfo, out result.Host, out result.Port, out result.PathSegments,
                out result.Query, out result.Fragment);
            return result;
        }

        /// <summary>
        /// Validates and percent-decodes a given string.
        /// </summary>
        /// <param name="value">The string to decode.</param>
        /// <param name="isSafe">A function defining which characters do not need percent-encoding.</param>
        /// <param name="part">The name of the part of the URI. This is used for a detailed exception message.</param>
        /// <param name="uriReference">The full URI being parsed. This is used for a detailed exception message.</param>
        public static string PercentDecode(string value, Func<byte, bool> isSafe, string part, string uriReference)
        {
            try
            {
                return Utility.PercentDecode(value, isSafe);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Invalid {part} \"{value}\" in URI reference \"{uriReference}\".", ex);
            }
        }
    }
}
