using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.UniformResourceIdentifiers.Helpers;
// ReSharper disable VirtualMemberNeverOverridden.Global

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// An immutable relative reference.
    /// </summary>
    /// <remarks>
    /// <para>The <see cref="UniformResourceIdentifierReference.Scheme"/> property is always <c>null</c>.</para>
    /// </remarks>
    public sealed class RelativeReference : UniformResourceIdentifierReference
    {
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
            : base(null, userInfo, host, port, NormalizePath(userInfo, host, port, pathSegments), query, fragment)
        {
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

        /// <summary>
        /// Converts to a relative <see cref="Uri"/>.
        /// </summary>
        public override Uri ToUri() => new Uri(Uri, UriKind.Relative);

        /// <summary>
        /// Parses a relative URI.
        /// </summary>
        /// <param name="relativeUri">The relative URI to parse.</param>
        public new static RelativeReference Parse(string relativeUri)
        {
            var result = UniformResourceIdentifierReference.Parse(relativeUri) as RelativeReference;
            if (result == null)
                throw new InvalidOperationException($"URI is not a relative reference: {relativeUri}");
            return result;
        }
    }
}