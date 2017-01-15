using System;
using Nito.UniformResourceIdentifiers.Helpers;

namespace Nito.UniformResourceIdentifiers.Unknown
{
    /// <summary>
    /// A URI builder for URIs with unknown schemes.
    /// </summary>
    public sealed class UnknownUniformResourceIdentifierBuilder : UniformResourceIdentifierBuilderBase<UnknownUniformResourceIdentifierBuilder>
    {
        /// <summary>
        /// Constructs an empty builder.
        /// </summary>
        public UnknownUniformResourceIdentifierBuilder() { }

        /// <summary>
        /// Constructs a builder from an existing URI.
        /// </summary>
        /// <param name="uri">The URI used to set the builder's initial values.</param>
        public UnknownUniformResourceIdentifierBuilder(UnknownUniformResourceIdentifier uri)
        {
            ApplyUriReference(uri);
        }

        /// <summary>
        /// Parses a URI string and constructs a builder.
        /// </summary>
        /// <param name="uri">The URI used to set the builder's initial values.</param>
        public UnknownUniformResourceIdentifierBuilder(string uri)
        {
            WithScheme(ApplyUriReference(uri));
        }

        /// <summary>
        /// The URI scheme. May not be <c>null</c>. Must be a valid scheme according to <see cref="Util.IsValidScheme"/>.
        /// </summary>
        private string Scheme { get; set; }

        /// <summary>
        /// Applies the scheme to this builder, overwriting any existing scheme.
        /// </summary>
        /// <param name="scheme">The scheme. May not be <c>null</c>. Must be a valid scheme according to <see cref="Util.IsValidScheme"/>.</param>
        public UnknownUniformResourceIdentifierBuilder WithScheme(string scheme)
        {
            if (scheme == null || !Util.IsValidScheme(scheme))
                throw new ArgumentException("Invalid scheme " + scheme, nameof(scheme));
            Scheme = scheme;
            return this;
        }

        /// <summary>
        /// Builds the unknown URI instance.
        /// </summary>
        public UnknownUniformResourceIdentifier Build() => new UnknownUniformResourceIdentifier(Scheme, UserInfo, Host, Port, PathSegments, Query, Fragment);
    }
}
