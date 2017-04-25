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
            BuilderUtil.ApplyUriReference(this, uri);
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

        /// <summary>
        /// Builds the unknown URI instance.
        /// </summary>
        public GenericUniformResourceIdentifier Build() => new GenericUniformResourceIdentifier(_scheme, _userInfo, _host, _port, _pathSegments.Value, _query, _fragment);

        GenericUniformResourceIdentifierBuilder IBuilderWithUserInfo<GenericUniformResourceIdentifierBuilder>.WithUserInfo(string userInfo)
        {
            _userInfo = userInfo;
            return this;
        }

        /// <summary>
        /// Applies the host portion of the authority to this builder, overwriting any existing host.
        /// </summary>
        /// <param name="host">The host. May be <c>null</c> or the empty string.</param>
        public GenericUniformResourceIdentifierBuilder WithHost(string host)
        {
            _host = host;
            return this;
        }

        /// <summary>
        /// Applies the port portion of the authority to this builder, overwriting any existing port.
        /// </summary>
        /// <param name="port">The port. May be <c>null</c> or the empty string.</param>
        public GenericUniformResourceIdentifierBuilder WithPort(string port)
        {
            _port = port;
            return this;
        }

        /// <summary>
        /// Applies the path to this builder, overwriting any existing path. This method does not automatically prefix a forward slash to the resulting path.
        /// </summary>
        /// <param name="pathSegments">The path segments.</param>
        public GenericUniformResourceIdentifierBuilder WithPrefixlessPathSegments(IEnumerable<string> pathSegments)
        {
            _pathSegments.Set(pathSegments);
            return this;
        }

        /// <summary>
        /// Applies the query string to this builder, overwriting any existing query.
        /// </summary>
        /// <param name="query">The query.</param>
        public GenericUniformResourceIdentifierBuilder WithQuery(string query)
        {
            _query = query;
            return this;
        }

        /// <summary>
        /// Applies the fragment string to this builder, overwriting any existing fragment.
        /// </summary>
        /// <param name="fragment">The fragment.</param>
        public GenericUniformResourceIdentifierBuilder WithFragment(string fragment)
        {
            _fragment = fragment;
            return this;
        }
    }
}
