using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nito.UniformResourceIdentifiers.Implementation;
using Nito.UniformResourceIdentifiers.Implementation.Builder;
using Nito.UniformResourceIdentifiers.Implementation.Builder.Components;

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// A URI builder for HTTP URIs.
    /// </summary>
    public sealed class HttpUniformResourceIdentifierBuilder : ICommonBuilder<HttpUniformResourceIdentifierBuilder>
    {
        private string? _host, _port, _query, _fragment;
        private readonly PathSegments _pathSegments = new PathSegments();

        /// <summary>
        /// Constructs an empty builder.
        /// </summary>
        public HttpUniformResourceIdentifierBuilder() { }

        /// <summary>
        /// Constructs a builder from an existing HTTP URI.
        /// </summary>
        /// <param name="uri">The URI used to set the builder's initial values.</param>
        public HttpUniformResourceIdentifierBuilder(HttpUniformResourceIdentifier uri)
        {
            BuilderUtil.ApplyUriReference(this, uri);
        }

        /// <summary>
        /// Parses a URI string and constructs a builder.
        /// </summary>
        /// <param name="uri">The URI used to set the builder's initial values.</param>
        public HttpUniformResourceIdentifierBuilder(string uri)
        {
            BuilderUtil.ApplyUriReference(this, uri, HttpUniformResourceIdentifier.HttpScheme);
        }

        HttpUniformResourceIdentifierBuilder IBuilderWithUserInfo<HttpUniformResourceIdentifierBuilder>.WithUserInfo(string? userInfo)
        {
            if (userInfo != null)
                throw new InvalidOperationException("HTTP/HTTPS URIs can no longer have UserInfo portions, as of RFC7230.");
            return this;
        }

        /// <inheritdoc />
        public HttpUniformResourceIdentifierBuilder WithHost(string? host)
        {
            _host = host;
            return this;
        }

        /// <inheritdoc />
        public HttpUniformResourceIdentifierBuilder WithPort(string? port)
        {
            _port = port;
            return this;
        }

        /// <inheritdoc />
        public HttpUniformResourceIdentifierBuilder WithPrefixlessPathSegments(IEnumerable<string> pathSegments)
        {
            _pathSegments.Set(pathSegments);
            return this;
        }

        /// <inheritdoc />
        public HttpUniformResourceIdentifierBuilder WithQuery(string? query)
        {
            _query = query;
            return this;
        }

        /// <inheritdoc />
        public HttpUniformResourceIdentifierBuilder WithFragment(string? fragment)
        {
            _fragment = fragment;
            return this;
        }

        /// <summary>
        /// Builds the HTTP URI instance.
        /// </summary>
        public HttpUniformResourceIdentifier Build() => new HttpUniformResourceIdentifier(_host, _port, _pathSegments.Value, _query, _fragment);
    }
}
