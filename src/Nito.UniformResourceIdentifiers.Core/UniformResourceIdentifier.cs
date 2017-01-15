using System;
using System.Collections.Generic;
using Nito.Comparers;
using Nito.Comparers.Util;
using Nito.UniformResourceIdentifiers.Helpers;
using static Nito.UniformResourceIdentifiers.Helpers.Util;
// ReSharper disable VirtualMemberNeverOverridden.Global

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// An immutable, normalized URI.
    /// </summary>
    /// <remarks>
    /// <para>The <see cref="UniformResourceIdentifierReference.Scheme"/> property is always valid and never <c>null</c>.</para>
    /// </remarks>
    public abstract class UniformResourceIdentifier : UniformResourceIdentifierReference, IEquatable<UniformResourceIdentifier>, IComparable, IComparable<UniformResourceIdentifier>
    {
        /// <summary>
        /// Gets the default comparer for this type.
        /// </summary>
        public static IFullComparer<UniformResourceIdentifier> DefaultComparer { get; }

        static UniformResourceIdentifier()
        {
            // URIs are compared by (lowercase) scheme (ordinally).
            // If two URIs have the same scheme, then they are compared by their SchemeSpecificComparerProxy properties.
            // Unless overridden by a scheme, the SchemeSpecificComparerProxy is a GenericComparerProxy instance that defines its comparison as follows:
            //   (lowercase) Host (ordinally), Port (numerically), UserInfo (ordinally), Path Segments (lexicographically ordinally), Query (ordinally), and Fragment (ordinally).
            // This generic scheme comparison is also exposed to URI scheme-specific implementations as GenericComparer.
            DefaultComparer = ComparerBuilder.For<UniformResourceIdentifier>()
                .OrderBy(x => x.Scheme, StringComparer.Ordinal).ThenBy(x => x.SchemeSpecificComparerProxy);
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
        /// <para>Since <see cref="UniformResourceIdentifierReference.Scheme"/> and <see cref="UniformResourceIdentifierReference.Host"/> are always stored lowercased, and all fields are stored in their decoded form, all URI instances are case normalized (see 6.2.2.1).</para>
        /// <para>Since unreserved characters are percent-unencoded during parsing, all URI instances are also percent-encoding normalized (see 6.2.2.2).</para>
        /// <para>Since path segments are normalized when creating URIs, all URI instances are also path segment normalized (see 6.2.2.3).</para>
        /// </remarks>
        protected UniformResourceIdentifier(string scheme, string userInfo, string host, string port, IEnumerable<string> pathSegments, string query, string fragment)
            : base(scheme, userInfo, host, port, RemoveDotSegments(pathSegments), query, fragment)
        {
            if (scheme == null)
                throw new ArgumentNullException(nameof(scheme));
            SchemeSpecificComparerProxy = new GenericComparerProxy(this);
        }

        /// <summary>
        /// Creates a factory delegate for use by derived classes to implement relative/reference resolution.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the builder for the derived URI type.</typeparam>
        /// <typeparam name="TUri">The type of the derived URI.</typeparam>
        /// <param name="builderFactory">A factory for the builder.</param>
        /// <param name="build">Calls the <c>Build</c> method of the builder.</param>
        protected static DelegateFactory<TUri> CreateFactory<TBuilder, TUri>(Func<TBuilder> builderFactory, Func<TBuilder, TUri> build)
            where TBuilder : UniformResourceIdentifierBuilderBase<TBuilder>
        {
            return (userInfo, host, port, pathSegments, query, fragment) =>
                build(builderFactory().WithUserInfo(userInfo).WithHost(host).WithPort(port).WithPrefixlessPathSegments(pathSegments).WithQuery(query).WithFragment(fragment));
        }

        /// <summary>
        /// Gets or sets a proxy object used for comparison. By default, this is set to an object that uses <see cref="GenericComparer"/> to compare URIs.
        /// </summary>
        protected virtual object SchemeSpecificComparerProxy { get; }

        /// <summary>
        /// The generic URI comparer, which is used to compare URIs if their schemes match and the scheme does not override <see cref="SchemeSpecificComparerProxy"/>.
        /// </summary>
        protected static IFullComparer<UniformResourceIdentifier> GenericComparer { get; } = ComparerBuilder.For<UniformResourceIdentifier>()
            .OrderBy(x => x.Host, StringComparer.Ordinal)
            .ThenBy(x => x.Port) // TODO: numeric comparer
            .ThenBy(x => x.UserInfo, StringComparer.Ordinal)
            .ThenBy(x => x.PathSegments, ComparerBuilder.For<string>().OrderBy(x => x, StringComparer.Ordinal).Sequence())
            .ThenBy(x => x.Query, StringComparer.Ordinal)
            .ThenBy(x => x.Fragment, StringComparer.Ordinal);

        /// <inheritdoc />
        public override int GetHashCode() => ComparableImplementations.ImplementGetHashCode(DefaultComparer, this);

        /// <inheritdoc />
        public override bool Equals(object obj) => ComparableImplementations.ImplementEquals(DefaultComparer, (object)this, obj);

        /// <inheritdoc />
        public bool Equals(UniformResourceIdentifier other) => ComparableImplementations.ImplementEquals(DefaultComparer, this, other);

        int IComparable.CompareTo(object obj) => ComparableImplementations.ImplementCompareTo(DefaultComparer, this, obj);
        /// <inheritdoc />
        public int CompareTo(UniformResourceIdentifier other) => ComparableImplementations.ImplementCompareTo(DefaultComparer, this, other);

        /// <summary>
        /// Converts to an absolute <see cref="Uri"/>.
        /// </summary>
        public override Uri ToUri() => new Uri(Uri, UriKind.Absolute);

        /// <summary>
        /// Parses a URI.
        /// </summary>
        /// <param name="uri">The URI to parse.</param>
        public new static UniformResourceIdentifier Parse(string uri)
        {
            var result = UniformResourceIdentifierReference.Parse(uri) as UniformResourceIdentifier;
            if (result == null)
                throw new InvalidOperationException($"URI reference is not a URI: {uri}");
            return result;
        }

        private sealed class GenericComparerProxy : ComparableBase<GenericComparerProxy>
        {
            private readonly UniformResourceIdentifier _uri;

            public GenericComparerProxy(UniformResourceIdentifier uri)
            {
                _uri = uri;
            }

            static GenericComparerProxy()
            {
                DefaultComparer = ComparerBuilder.For<GenericComparerProxy>().OrderBy(x => x._uri, GenericComparer);
            }
        }
    }
}
