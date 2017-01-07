using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nito.UniformResourceIdentifiers.Helpers;
using static Nito.UniformResourceIdentifiers.Helpers.Util;
// ReSharper disable VirtualMemberNeverOverridden.Global

// TODO: Parsing - performed by builder or (derived) URI types. The generic URI parser should only live in Util. Possibly also have a factory, with registration???
// TODO: Relative URIs

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// An immutable, normalized URI reference.
    /// </summary>
    public abstract class UniformResourceIdentifierReference
    {
        /// <summary>
        /// Constructs a new URI reference.
        /// </summary>
        /// <param name="scheme">The scheme, if any. This is converted to lowercase. This must be <c>null</c> or a valid scheme as defined by <see cref="Util.IsValidScheme"/>.</param>
        /// <param name="userInfo">The user information portion of the authority, if any. This may be <c>null</c> to indicate no user info, or the empty string to indicate empty user info.</param>
        /// <param name="host">The host name portion of the authority, if any. This is converted to lowercase. This may be <c>null</c> to indicate no host name, or the empty string to indicate an empty host name.</param>
        /// <param name="port">The port portion of the authority, if any. This may be <c>null</c> to indicate no port, or the empty string to indicate an empty port. This must be <c>null</c>, the empty string, or a valid port as defined by <see cref="Util.IsValidPort"/>.</param>
        /// <param name="pathSegments">The path segments. Dot segments are normalized. May not be <c>null</c>, neither may any element be <c>null</c>.</param>
        /// <param name="query">The query. This may be <c>null</c> to indicate no query, or the empty string to indicate an empty query.</param>
        /// <param name="fragment">The fragment. This may be <c>null</c> to indicate no fragment, or the empty string to indicate an empty fragment.</param>
        protected UniformResourceIdentifierReference(string scheme, string userInfo, string host, string port, IEnumerable<string> pathSegments, string query, string fragment)
        {
            if (pathSegments == null)
                throw new ArgumentNullException(nameof(pathSegments));
            if (scheme != null && !IsValidScheme(scheme))
                throw new ArgumentException("Invalid scheme " + scheme, nameof(scheme));
            if (port != null && !IsValidPort(port))
                throw new ArgumentException("Invalid port " + port, nameof(port));
            var segments = RemoveDotSegments(pathSegments);
            if (segments.Any(x => x == null))
                throw new ArgumentException("Path contains null segments", nameof(pathSegments));
            if (userInfo != null || host != null || port != null)
            {
                if (!string.IsNullOrEmpty(segments.FirstOrDefault()))
                    throw new ArgumentException("URI with authority must have an absolute path", nameof(pathSegments));
            }

            Scheme = scheme?.ToLowerInvariant();
            UserInfo = userInfo;
            Host = host?.ToLowerInvariant();
            Port = port;
            PathSegments = segments;
            Query = query;
            Fragment = fragment;
        }

        /// <summary>
        /// Gets the scheme of this URI, e.g., "http". This can be <c>null</c> if there is no scheme. If not <c>null</c>, then this is always a valid scheme; it can never be the empty string.
        /// </summary>
        public virtual string Scheme { get; }

        /// <summary>
        /// Gets the user info portion of the authority of this URI, e.g., "username:password". This can be <c>null</c> if there is no user info, or an empty string if the user info is empty.
        /// </summary>
        public virtual string UserInfo { get; }

        /// <summary>
        /// Gets the host portion of the authority of this URI, e.g., "www.example.com". This can be <c>null</c> if there is no host, or an empty string if the host is empty.
        /// </summary>
        public virtual string Host { get; }

        /// <summary>
        /// Gets the port portion of the authority of this URI, e.g., "8080". This can be <c>null</c> if there is no port, or an empty string if the port is empty. Any string returned from this property is a numeric string.
        /// </summary>
        public virtual string Port { get; }

        /// <summary>
        /// Gets the path segments of the URI, e.g., { "", "folder", "subfolder", "file.jpg" }. This can never be <c>null</c>, but it can be empty. Note that for some schemes, it is common for the first path segment to be the empty string to generate an initial forward-slash.
        /// </summary>
        public virtual IReadOnlyList<string> PathSegments { get; }

        /// <summary>
        /// Gets the query of the URI, e.g., "q=test&amp;page=4". This can be <c>null</c> if there is no query, or an empty string if the query is empty.
        /// </summary>
        public virtual string Query { get; }

        /// <summary>
        /// Gets the fragment of the URI, e.g., "anchor-1". This can be <c>null</c> if there is no fragment, or an empty string if the fragment is empty.
        /// </summary>
        public virtual string Fragment { get; }

        /// <summary>
        /// Gets the URI as a complete string, e.g., "http://username:password@www.example.com:8080/folder/subfolder/file.jpg?q=test&amp;page=4#anchor-1". This is never <c>null</c> or an empty string.
        /// </summary>
        public string Uri => ToString(Scheme, UserInfo, Host, Port, PathSegments, Query, Fragment);

        /// <summary>
        /// Gets the URI as a complete string without the deprecated <see cref="UserInfo"/> portion, e.g., "http://www.example.com:8080/folder/subfolder/file.jpg?q=test&amp;page=4#anchor-1". This is never <c>null</c> or an empty string.
        /// </summary>
        public override string ToString() => ToString(Scheme, null, Host, Port, PathSegments, Query, Fragment);

        private static string ToString(string scheme, string userInfo, string host, string port, IEnumerable<string> pathSegments, string query, string fragment)
        {
            // See 5.3, except Authority is split up into (UserInfo, Host, Port).
            var sb = new StringBuilder();
            if (scheme != null)
            {
                sb.Append(scheme);
                sb.Append(':');
            }
            if (userInfo != null || host != null || port != null)
                sb.Append("//");
            if (userInfo != null)
            {
                sb.Append(PercentEncodeUserInfo(userInfo));
                sb.Append('@');
            }
            if (host != null)
                sb.Append(PercentEncodeHost(host));
            if (port != null)
            {
                sb.Append(':');
                sb.Append(port);
            }
            sb.Append(PercentEncdePathSegments(pathSegments));
            if (query != null)
            {
                sb.Append('?');
                sb.Append(PercentEncodeQuery(query));
            }
            if (fragment != null)
            {
                sb.Append('#');
                sb.Append(PercentEncodeFragment(fragment));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts to a <see cref="Uri"/>.
        /// </summary>
        public abstract Uri ToUri();
    }
}
