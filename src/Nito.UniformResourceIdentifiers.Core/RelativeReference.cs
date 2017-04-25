using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.UniformResourceIdentifiers.Implementation;
using Nito.UniformResourceIdentifiers.Implementation.Components;

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// An immutable relative reference.
    /// </summary>
    public sealed class RelativeReference : IUniformResourceIdentifierReference
    {
        private readonly NormalizedHost _host;
        private readonly NormalizedPort _port;
        private readonly NormalizedPathSegments _pathSegments;
        private readonly string _userInfo;

        /// <summary>
        /// Constructs a new relative reference.
        /// </summary>
        /// <param name="userInfo">The user information portion of the authority, if any. This may be <c>null</c> to indicate no user info, or the empty string to indicate empty user info.</param>
        /// <param name="host">The host name portion of the authority, if any. This is converted to lowercase. This may be <c>null</c> to indicate no host name, or the empty string to indicate an empty host name.</param>
        /// <param name="port">The port portion of the authority, if any. This may be <c>null</c> to indicate no port, or the empty string to indicate an empty port. This must be <c>null</c>, the empty string, or a valid port as defined by <see cref="Util.IsValidPort"/>.</param>
        /// <param name="pathSegments">The path segments. Dot segments are normalized. May not be <c>null</c>, neither may any element be <c>null</c>.</param>
        /// <param name="query">The query. This may be <c>null</c> to indicate no query, or the empty string to indicate an empty query.</param>
        /// <param name="fragment">The fragment. This may be <c>null</c> to indicate no fragment, or the empty string to indicate an empty fragment.</param>
        public RelativeReference(string userInfo, string host, string port, IEnumerable<string> pathSegments, string query, string fragment)
        {
            _userInfo = userInfo;
            _host = new NormalizedHost(host);
            _port = new NormalizedPort(port);
            _pathSegments = new NormalizedPathSegments(NormalizePath(_userInfo, _host.Value, _port.Value, pathSegments), _userInfo, _host.Value, _port.Value, dotNormalize: false);
            Query = query;
            Fragment = fragment;
        }

        private static IEnumerable<string> NormalizePath(string userInfo, string host, string port, IEnumerable<string> pathSegments)
        {
            if (userInfo != null || host != null || port != null)
                return pathSegments;
            return NormalizePathWithoutAuthority(pathSegments);
        }

        private static IEnumerable<string> NormalizePathWithoutAuthority(IEnumerable<string> pathSegments)
        { 
            var first = true;
            foreach (var segment in pathSegments)
            {
                if (first)
                {
                    if (segment.Contains(":"))
                        yield return ".";
                    first = false;
                }
                yield return segment;
            }
        }

        string IUniformResourceIdentifierReference.UserInfo => _userInfo;

        /// <summary>
        /// Gets the host portion of the authority of this URI, e.g., "www.example.com". This can be <c>null</c> if there is no host, or an empty string if the host is empty.
        /// </summary>
        public string Host => _host.Value;

        /// <summary>
        /// Gets the port portion of the authority of this URI, e.g., "8080". This can be <c>null</c> if there is no port, or an empty string if the port is empty. Any string returned from this property is a numeric string.
        /// </summary>
        public string Port => _port.Value;

        /// <summary>
        /// Gets the path segments of the URI, e.g., { "", "folder", "subfolder", "file.jpg" }. This can never be <c>null</c>, but it can be empty. Note that for some schemes, it is common for the first path segment to be the empty string to generate an initial forward-slash.
        /// </summary>
        public IReadOnlyList<string> PathSegments => _pathSegments.Value;

        /// <summary>
        /// Returns <c>true</c> if the path is absolute (i.e., starts with a forward-slash).
        /// </summary>
        public bool PathIsAbsolute => Util.PathIsAbsolute(PathSegments);

        /// <summary>
        /// Gets the query of the URI, e.g., "q=test&amp;page=4". This can be <c>null</c> if there is no query, or an empty string if the query is empty.
        /// </summary>
        public string Query { get; }

        /// <summary>
        /// Gets the fragment of the URI, e.g., "anchor-1". This can be <c>null</c> if there is no fragment, or an empty string if the fragment is empty.
        /// </summary>
        public string Fragment { get; }

        /// <summary>
        /// Gets the URI as a complete string, e.g., "http://username:password@www.example.com:8080/folder/subfolder/file.jpg?q=test&amp;page=4#anchor-1". This is never <c>null</c> or an empty string.
        /// </summary>
        public string ToUriString() => Util.ToString(null, _userInfo, Host, Port, PathSegments, Query, Fragment);

        /// <summary>
        /// Gets the URI as a complete string without the deprecated <see cref="IUniformResourceIdentifierReference.UserInfo"/> portion, e.g., "http://www.example.com:8080/folder/subfolder/file.jpg?q=test&amp;page=4#anchor-1". This is never <c>null</c> or an empty string.
        /// </summary>
        public override string ToString() => Util.ToString(null, null, Host, Port, PathSegments, Query, Fragment);

        /// <summary>
        /// Converts to a relative <see cref="Uri"/>.
        /// </summary>
        public Uri ToUri() => new Uri(ToUriString(), UriKind.Relative);

        /// <summary>
        /// Parses a relative URI.
        /// </summary>
        /// <param name="relativeUri">The relative URI to parse.</param>
        public static RelativeReference Parse(string relativeUri) => new RelativeReferenceBuilder(relativeUri).Build();
    }
}