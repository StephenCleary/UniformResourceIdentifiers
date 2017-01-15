using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Nito.UniformResourceIdentifiers.Helpers.Util;

namespace Nito.UniformResourceIdentifiers.Helpers
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
        public static bool TryCoarseParseUriReference(string uriReference, out string scheme, out string authority, out string path, out string query, out string fragment)
        {
            scheme = authority = path = query = fragment = null;
            var match = UriReferenceRegex.Match(uriReference);
            if (!match.Success)
                return false;
            scheme = match.Groups[1].Value == "" ? null : match.Groups[1].Value;
            authority = match.Groups[2].Value == "" ? null : match.Groups[3].Value;
            path = match.Groups[4].Value;
            query = match.Groups[5].Value == "" ? null : match.Groups[6].Value;
            fragment = match.Groups[7].Value == "" ? null : match.Groups[8].Value;
            return true;
        }

        /// <summary>
        /// Breaks an authority into its components, without any decoding or verification of them.
        /// </summary>
        /// <param name="authority">The authority. May be <c>null</c> or the empty string.</param>
        /// <param name="userInfo">On return, contains the user info. May be <c>null</c> or the empty string.</param>
        /// <param name="host">On return, contains the host. May be <c>null</c> or the empty string.</param>
        /// <param name="port">On return, contains the port. May be <c>null</c> or the empty string.</param>
        public static void CoarseParseAuthority(string authority, out string userInfo, out string host, out string port)
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
            if (authority.StartsWith("["))
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
        public static void ParseUriReference(string uriReference, out string scheme, out string userInfo, out string host, out string port, out IReadOnlyList<string> pathSegments, out string query, out string fragment)
        {
            pathSegments = null;

            // Unescape unreserved characters; this is always a safe operation, and only needs to be done once because "%%" is not a valid input anyway.
            uriReference = DecodeUnreserved(uriReference);

            // Coarse-parse it into sections.
            string authority, path;
            var isUri = TryCoarseParseUriReference(uriReference, out scheme, out authority, out path, out query, out fragment);
            CoarseParseAuthority(authority, out userInfo, out host, out port);
            if (!isUri)
                throw new InvalidOperationException($"Invalid URI reference \"{uriReference}\".");

            // Decode and verify each one.

            if (scheme != null && !IsValidScheme(scheme))
                throw new InvalidOperationException($"Invalid scheme \"{scheme}\" in URI reference \"{uriReference}\".");
            if (userInfo != null)
                userInfo = PercentDecode(userInfo, UserInfoCharIsSafe, "user info", uriReference);
            if (host != null)
                host = HostIsIpAddress(host) ? host : PercentDecode(host, HostRegNameCharIsSafe, "host", uriReference);
            if (port != null && !IsValidPort(port))
                throw new InvalidOperationException($"Invalid port \"{port}\" in URI reference \"{uriReference}\".");
            pathSegments = path.Split('/').Select(x => PercentDecode(x, PathSegmentCharIsSafe, "path segment", uriReference)).ToList();
            if (query != null)
                query = PercentDecode(query, QueryCharIsSafe, "query", uriReference);
            if (fragment != null)
                fragment = PercentDecode(fragment, FragmentCharIsSafe, "fragment", uriReference);
        }

        private static string PercentDecode(string value, Func<byte, bool> isSafe, string part, string uriReference)
        {
            try
            {
                return Util.PercentDecode(value, isSafe);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Invalid {part} \"{value}\" in URI reference \"{uriReference}\".", ex);
            }
        }
    }
}
