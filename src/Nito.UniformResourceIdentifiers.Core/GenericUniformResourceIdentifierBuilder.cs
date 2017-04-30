using System.Collections.Generic;
using Nito.UniformResourceIdentifiers.Implementation;
using Nito.UniformResourceIdentifiers.Implementation.Builder;
using Nito.UniformResourceIdentifiers.Implementation.Builder.Components;

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// A URI builder for URIs with unknown schemes.
    /// </summary>
    public sealed class GenericUniformResourceIdentifierBuilder : ICommonBuilder<GenericUniformResourceIdentifierBuilder>
    {
        private string _scheme, _userInfo, _host, _port, _query, _fragment;
        private readonly PathSegments _pathSegments = new PathSegments();

        /// <summary>
        /// Constructs an empty builder.
        /// </summary>
        public GenericUniformResourceIdentifierBuilder() { }

        /// <summary>
        /// Constructs a builder from an existing URI.
        /// </summary>
        /// <param name="uri">The URI used to set the builder's initial values.</param>
        public GenericUniformResourceIdentifierBuilder(GenericUniformResourceIdentifier uri)
        {
            BuilderUtil.ApplyUriReference(this, uri).WithScheme(uri.Scheme);
        }

        /// <summary>
        /// Parses a URI string and constructs a builder.
        /// </summary>
        /// <param name="uri">The URI used to set the builder's initial values.</param>
        public GenericUniformResourceIdentifierBuilder(string uri)
        {
            WithScheme(BuilderUtil.ApplyUriReference(this, uri));
        }

        /// <summary>
        /// Applies the scheme to this builder, overwriting any existing scheme.
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        public GenericUniformResourceIdentifierBuilder WithScheme(string scheme)
        {
            _scheme = scheme;
            return this;
        }

        GenericUniformResourceIdentifierBuilder IBuilderWithUserInfo<GenericUniformResourceIdentifierBuilder>.WithUserInfo(string userInfo)
        {
            _userInfo = userInfo;
            return this;
        }

        /// <inheritdoc />
        public GenericUniformResourceIdentifierBuilder WithHost(string host)
        {
            _host = host;
            return this;
        }

        /// <inheritdoc />
        public GenericUniformResourceIdentifierBuilder WithPort(string port)
        {
            _port = port;
            return this;
        }

        /// <inheritdoc />
        public GenericUniformResourceIdentifierBuilder WithPrefixlessPathSegments(IEnumerable<string> pathSegments)
        {
            _pathSegments.Set(pathSegments);
            return this;
        }

        /// <inheritdoc />
        public GenericUniformResourceIdentifierBuilder WithQuery(string query)
        {
            _query = query;
            return this;
        }

        /// <inheritdoc />
        public GenericUniformResourceIdentifierBuilder WithFragment(string fragment)
        {
            _fragment = fragment;
            return this;
        }

        /// <summary>
        /// Builds the unknown URI instance.
        /// </summary>
        public GenericUniformResourceIdentifier Build() => new GenericUniformResourceIdentifier(_scheme, _userInfo, _host, _port, _pathSegments.Value, _query, _fragment);
    }
}
