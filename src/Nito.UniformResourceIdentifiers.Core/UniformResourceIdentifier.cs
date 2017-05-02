using System;
using System.Collections.Generic;
using System.Linq;
using Nito.Comparers;
using Nito.UniformResourceIdentifiers.Implementation;

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// URI utils.
    /// </summary>
    public static class UniformResourceIdentifier
    {
        /// <summary>
        /// The generic URI comparer, which is used to compare URIs if their schemes match and the scheme does not implement <see cref="IUniformResourceIdentifierWithCustomComparison"/>.
        /// </summary>
        private static IFullComparer<IUniformResourceIdentifier> GenericComparer { get; } = ComparerBuilder.For<IUniformResourceIdentifier>()
            .OrderBy(x => x.Host, StringComparer.Ordinal)
            .ThenBy(x => x.Port, Util.NumericStringComparer)
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
                throw new ArgumentException($"URI reference is not a URI: {uri}");
            return result;
        }
    }
}
