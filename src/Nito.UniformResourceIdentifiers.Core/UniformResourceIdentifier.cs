using System;
using System.Collections.Generic;
using System.Linq;
using Nito.Comparers;
using Nito.Comparers.Util;
using Nito.UniformResourceIdentifiers.Builder;
using Nito.UniformResourceIdentifiers.Helpers;
using static Nito.UniformResourceIdentifiers.Helpers.Util;

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// URI utils.
    /// </summary>
    public static class UniformResourceIdentifierEx
    {
        /// <summary>
        /// A numeric string comparer, capable of comparing numeric strings of any length. This comparer assumes its operands only consist of the digits 0-9 and have leading zeroes removed (this is true of the <see cref="IUniformResourceIdentifierReference.Port"/> values.
        /// </summary>
        public static IFullComparer<string> NumericStringComparer { get; } = ComparerBuilder.For<string>()
            .OrderBy(x => x.Length)
            .ThenBy(x => x, StringComparer.Ordinal);

        /// <summary>
        /// The generic URI comparer, which is used to compare URIs if their schemes match and the scheme does not implement <see cref="IUniformResourceIdentifierWithCustomComparison"/>.
        /// </summary>
        private static IFullComparer<IUniformResourceIdentifier> GenericComparer { get; } = ComparerBuilder.For<IUniformResourceIdentifier>()
            .OrderBy(x => x.Host, StringComparer.Ordinal)
            .ThenBy(x => x.Port, NumericStringComparer)
            .ThenBy(x => x.UserInfo, StringComparer.Ordinal)
            .ThenBy(x => x.PathSegments, ComparerBuilder.For<string>().OrderBy(x => x, StringComparer.Ordinal).Sequence())
            .ThenBy(x => x.Query, StringComparer.Ordinal)
            .ThenBy(x => x.Fragment, StringComparer.Ordinal);

        /// <summary>
        /// Allows comparing any URIs.
        /// </summary>
        /// <remarks>
        /// <para>URIs are compared by (lowercase) scheme (ordinally).</para>
        /// <para>If two URIs have the same scheme, then they are compared by their SchemeSpecificComparerProxy properties.</para>
        /// <para>Unless overridden by a scheme, the SchemeSpecificComparerProxy is a GenericComparerProxy instance that defines its comparison as follows: (lowercase) Host (ordinally), Port (numerically), UserInfo (ordinally), Path Segments (lexicographically ordinally), Query (ordinally), and Fragment (ordinally).</para>
        /// </remarks>
        public static IFullComparer<IUniformResourceIdentifier> Comparer { get; } = ComparerBuilder.For<IUniformResourceIdentifier>()
            .OrderBy(x => x.Scheme, StringComparer.Ordinal)
            .ThenBy(x => (x as IUniformResourceIdentifierWithCustomComparison)?.SchemeSpecificComparerProxy)
            .ThenBy(x => (x as IUniformResourceIdentifierWithCustomComparison)?.SchemeSpecificComparerProxy != null ? null : x, GenericComparer);

        /// <summary>
        /// Parses a URI. If the URI's scheme is not registered, then this will return an instance of <see cref="GenericUniformResourceIdentifier"/>.
        /// </summary>
        /// <param name="uri">The URI to parse.</param>
        public static IUniformResourceIdentifier Parse(string uri)
        {
            var result = UniformResourceIdentifierReference.Parse(uri) as IUniformResourceIdentifier;
            if (result == null)
                throw new InvalidOperationException($"URI reference is not a URI: {uri}");
            return result;
        }
    }

    /// <summary>
    /// An immutable, normalized URI.
    /// </summary>
    public abstract class UniformResourceIdentifier : ComparableBase<UniformResourceIdentifier>, IUniformResourceIdentifier
    {
        static UniformResourceIdentifier()
        {
            DefaultComparer = UniformResourceIdentifierEx.Comparer;
        }

        /// <summary>
        /// Constructs a new URI instance.
        /// </summary>
        /// <param name="scheme">The scheme. This is converted to lowercase. Must be a valid scheme as defined by <see cref="Util.IsValidScheme"/>.</param>
        /// <param name="userInfo">The user information portion of the authority, if any. This may be <c>null</c> to indicate no user info, or the empty string to indicate empty user info.</param>
        /// <param name="host">The host name portion of the authority, if any. This is converted to lowercase. This may be <c>null</c> to indicate no host name, or the empty string to indicate an empty host name.</param>
        /// <param name="port">The port portion of the authority, if any. This may be <c>null</c> to indicate no port, or the empty string to indicate an empty port. This must be <c>null</c>, the empty string, or a valid port as defined by <see cref="Util.IsValidPort"/>.</param>
        /// <param name="pathSegments">The path segments. Dot segments are normalized. May not be <c>null</c>, neither may any element be <c>null</c>.</param>
        /// <param name="query">The query. This may be <c>null</c> to indicate no query, or the empty string to indicate an empty query.</param>
        /// <param name="fragment">The fragment. This may be <c>null</c> to indicate no fragment, or the empty string to indicate an empty fragment.</param>
        /// <remarks>
        /// <para>All URIs are normalized. If your scheme has scheme-specific normalization rules (see 6.2.3), they should be applied at the end of your constructor.</para>
        /// <para>Since <see cref="IUniformResourceIdentifier.Scheme"/> and <see cref="IUniformResourceIdentifierReference.Host"/> are always stored lowercased, and all fields are stored in their decoded form, all URI instances are case normalized (see 6.2.2.1).</para>
        /// <para>Since unreserved characters are percent-unencoded during parsing, all URI instances are also percent-encoding normalized (see 6.2.2.2).</para>
        /// <para>Since path segments are normalized when creating URIs, all URI instances are also path segment normalized (see 6.2.2.3).</para>
        /// </remarks>
        protected UniformResourceIdentifier(string scheme, string userInfo, string host, string port, IEnumerable<string> pathSegments, string query, string fragment)
        {
            if (scheme == null)
                throw new ArgumentNullException(nameof(scheme));
            if (pathSegments == null)
                throw new ArgumentNullException(nameof(pathSegments));
            if (!IsValidScheme(scheme))
                throw new ArgumentException("Invalid scheme " + scheme, nameof(scheme));
            if (port != null && !IsValidPort(port))
                throw new ArgumentException("Invalid port " + port, nameof(port));
            var segments = pathSegments.ToList();
            if (segments.Any(x => x == null))
                throw new ArgumentException("Path contains null segments", nameof(pathSegments));
            segments = RemoveDotSegments(segments);
            if (userInfo != null || host != null || port != null)
            {
                if (!string.IsNullOrEmpty(segments.FirstOrDefault()))
                    throw new ArgumentException("URI with authority must have an absolute path", nameof(pathSegments));
            }

            Scheme = scheme?.ToLowerInvariant();
            UserInfo = userInfo;
            Host = host?.ToLowerInvariant();
            Port = NormalizePort(port);
            PathSegments = segments;
            Query = query;
            Fragment = fragment;
        }

        /// <summary>
        /// Removes leading zeroes from the port string, but leaves one if the port is 0.
        /// </summary>
        /// <param name="port">The port string.</param>
        private static string NormalizePort(string port)
        {
            if (string.IsNullOrEmpty(port))
                return port;
            var result = port.TrimStart('0');
            return result == "" ? "0" : result;
        }

        /// <summary>
        /// Creates a factory delegate for use by derived classes to implement relative/reference resolution.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the builder for the derived URI type.</typeparam>
        /// <typeparam name="TUri">The type of the derived URI.</typeparam>
        /// <param name="builderFactory">A factory for the builder.</param>
        /// <param name="build">Calls the <c>Build</c> method of the builder.</param>
        protected static DelegateFactory<TUri> CreateFactory<TBuilder, TUri>(Func<TBuilder> builderFactory, Func<TBuilder, TUri> build)
            where TBuilder : ICommonBuilder<TBuilder>
        {
            return (userInfo, host, port, pathSegments, query, fragment) =>
                build(builderFactory().WithUserInfo(userInfo).WithHost(host).WithPort(port).WithPrefixlessPathSegments(pathSegments).WithQuery(query).WithFragment(fragment));
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
        /// Returns <c>true</c> if the authority is defined. Note that it is possible (though unusual) for the authority to be defined as the empty string.
        /// </summary>
        public bool AuthorityIsDefined => UserInfo != null || Host != null || Port != null;

        /// <summary>
        /// Gets the path segments of the URI, e.g., { "", "folder", "subfolder", "file.jpg" }. This can never be <c>null</c>, but it can be empty. Note that for some schemes, it is common for the first path segment to be the empty string to generate an initial forward-slash.
        /// </summary>
        public virtual IReadOnlyList<string> PathSegments { get; }

        /// <summary>
        /// Returns <c>true</c> if the path is empty.
        /// </summary>
        public bool PathIsEmpty => Util.PathIsEmpty(PathSegments);

        /// <summary>
        /// Returns <c>true</c> if the path is absolute (i.e., starts with a forward-slash).
        /// </summary>
        public bool PathIsAbsolute => Util.PathIsAbsolute(PathSegments);

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
        public string Uri => Util.ToString(Scheme, UserInfo, Host, Port, PathSegments, Query, Fragment);

        /// <summary>
        /// Gets the URI as a complete string without the deprecated <see cref="UserInfo"/> portion, e.g., "http://www.example.com:8080/folder/subfolder/file.jpg?q=test&amp;page=4#anchor-1". This is never <c>null</c> or an empty string.
        /// </summary>
        public override string ToString() => Util.ToString(Scheme, null, Host, Port, PathSegments, Query, Fragment);

        /// <inheritdoc />
        public bool Equals(IUniformResourceIdentifier other) => ComparableImplementations.ImplementEquals(DefaultComparer, this, other);

        /// <inheritdoc />
        public int CompareTo(IUniformResourceIdentifier other) => ComparableImplementations.ImplementCompareTo(DefaultComparer, this, other);

        /// <summary>
        /// Converts to an absolute <see cref="Uri"/>.
        /// </summary>
        public virtual Uri ToUri() => new Uri(Uri, UriKind.Absolute);

        /// <summary>
        /// Resolves a relative URI against this URI. If this base URI's scheme is not registered, then this will return an instance of <see cref="GenericUniformResourceIdentifier"/>.
        /// </summary>
        /// <param name="relativeUri">The relative URI to resolve.</param>
        public virtual IUniformResourceIdentifier Resolve(RelativeReference relativeUri) => Util.Resolve(this, relativeUri,
            (info, host, port, segments, query, fragment) => (IUniformResourceIdentifier)Factories.Create(Scheme, info, host, port, segments, query, fragment));

        /// <summary>
        /// Resolves a reference URI against this URI. If this base URI's scheme is not registered and <paramref name="referenceUri"/> is a <see cref="RelativeReference"/>, then this will return an instance of <see cref="GenericUniformResourceIdentifier"/>.
        /// </summary>
        /// <param name="referenceUri">The reference URI to resolve.</param>
        public virtual IUniformResourceIdentifier Resolve(IUniformResourceIdentifierReference referenceUri) => Util.Resolve(this, referenceUri,
            (info, host, port, segments, query, fragment) => (IUniformResourceIdentifier)Factories.Create(Scheme, info, host, port, segments, query, fragment));

        /// <summary>
        /// Parses a URI. If the URI's scheme is not registered, then this will return an instance of <see cref="GenericUniformResourceIdentifier"/>.
        /// </summary>
        /// <param name="uri">The URI to parse.</param>
        public static UniformResourceIdentifier Parse(string uri)
        {
            var result = UniformResourceIdentifierReference.Parse(uri) as UniformResourceIdentifier;
            if (result == null)
                throw new InvalidOperationException($"URI reference is not a URI: {uri}");
            return result;
        }
    }
}
