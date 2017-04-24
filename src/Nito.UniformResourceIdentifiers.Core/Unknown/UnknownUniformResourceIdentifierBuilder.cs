using System;
using System.Collections.Generic;
using Nito.UniformResourceIdentifiers.BuilderComponents;
using Nito.UniformResourceIdentifiers.Helpers;

namespace Nito.UniformResourceIdentifiers.Unknown
{
    /// <summary>
    /// A URI builder for URIs with unknown schemes.
    /// </summary>
    public sealed class UnknownUniformResourceIdentifierBuilder : ICommonBuilder<UnknownUniformResourceIdentifierBuilder>
    {
        private string _scheme, _userInfo, _host, _port, _query, _fragment;
        private readonly PathSegments _pathSegments = new PathSegments();

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
            BuilderUtils.ApplyUriReference(this, uri);
        }

        /// <summary>
        /// Parses a URI string and constructs a builder.
        /// </summary>
        /// <param name="uri">The URI used to set the builder's initial values.</param>
        public UnknownUniformResourceIdentifierBuilder(string uri)
        {
            WithScheme(BuilderUtils.ApplyUriReference(this, uri));
        }

        /// <summary>
        /// Applies the scheme to this builder, overwriting any existing scheme.
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        public UnknownUniformResourceIdentifierBuilder WithScheme(string scheme)
        {
            _scheme = scheme;
            return this;
        }

        /// <summary>
        /// Builds the unknown URI instance.
        /// </summary>
        public UnknownUniformResourceIdentifier Build() => new UnknownUniformResourceIdentifier(_scheme, _userInfo, _host, _port, _pathSegments.Value, _query, _fragment);

        UnknownUniformResourceIdentifierBuilder IBuilderWithUserInfo<UnknownUniformResourceIdentifierBuilder>.WithUserInfo(string userInfo)
        {
            _userInfo = userInfo;
            return this;
        }

        /// <summary>
        /// Applies the host portion of the authority to this builder, overwriting any existing host.
        /// </summary>
        /// <param name="host">The host. May be <c>null</c> or the empty string.</param>
        public UnknownUniformResourceIdentifierBuilder WithHost(string host)
        {
            _host = host;
            return this;
        }

        /// <summary>
        /// Applies the port portion of the authority to this builder, overwriting any existing port.
        /// </summary>
        /// <param name="port">The port. May be <c>null</c> or the empty string.</param>
        public UnknownUniformResourceIdentifierBuilder WithPort(string port)
        {
            _port = port;
            return this;
        }

        /// <summary>
        /// Applies the path to this builder, overwriting any existing path. This method does not automatically prefix a forward slash to the resulting path.
        /// </summary>
        /// <param name="pathSegments">The path segments.</param>
        public UnknownUniformResourceIdentifierBuilder WithPrefixlessPathSegments(IEnumerable<string> pathSegments)
        {
            _pathSegments.Set(pathSegments);
            return this;
        }

        /// <summary>
        /// Applies the query string to this builder, overwriting any existing query.
        /// </summary>
        /// <param name="query">The query.</param>
        public UnknownUniformResourceIdentifierBuilder WithQuery(string query)
        {
            _query = query;
            return this;
        }

        /// <summary>
        /// Applies the fragment string to this builder, overwriting any existing fragment.
        /// </summary>
        /// <param name="fragment">The fragment.</param>
        public UnknownUniformResourceIdentifierBuilder WithFragment(string fragment)
        {
            _fragment = fragment;
            return this;
        }
    }
}
