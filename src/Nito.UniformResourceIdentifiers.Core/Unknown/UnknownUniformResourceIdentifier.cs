using System;
using System.Collections.Generic;
using Nito.UniformResourceIdentifiers.Helpers;

namespace Nito.UniformResourceIdentifiers.Unknown
{
    /// <summary>
    /// A URI with an unrecognized scheme. This URI is normalized as an arbitrary URI, but will not be fully normalized with respect to its scheme.
    /// </summary>
    public sealed class UnknownUniformResourceIdentifier : UniformResourceIdentifier
    {
        private readonly Util.DelegateFactory<UnknownUniformResourceIdentifier> _factory;

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
        public UnknownUniformResourceIdentifier(string scheme, string userInfo, string host, string port, IEnumerable<string> pathSegments, string query, string fragment)
            : base(scheme, userInfo, host, port, pathSegments, query, fragment)
        {
            _factory = CreateFactory(() => new UnknownUniformResourceIdentifierBuilder().WithScheme(Scheme), x => x.Build());
        }

        /// <summary>
        /// Resolves a relative URI against this URI.
        /// </summary>
        /// <param name="relativeUri">The relative URI to resolve.</param>
        public new UnknownUniformResourceIdentifier Resolve(RelativeReference relativeUri) => Util.Resolve(this, relativeUri, _factory);

        /// <summary>
        /// Resolves a reference URI against this URI.
        /// </summary>
        /// <param name="referenceUri">The reference URI to resolve.</param>
        public override IUniformResourceIdentifier Resolve(IUniformResourceIdentifierReference referenceUri) => Util.Resolve(this, referenceUri, _factory);

        /// <summary>
        /// Parses a URI.
        /// </summary>
        /// <param name="uri">The URI to parse.</param>
        public new static UnknownUniformResourceIdentifier Parse(string uri)
        {
            return new UnknownUniformResourceIdentifierBuilder(uri).Build();
        }
    }
}
