using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nito.UniformResourceIdentifiers.BuilderComponents;
using Nito.UniformResourceIdentifiers.Helpers;

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// A URI builder for HTTPS URIs.
    /// </summary>
    public sealed class HttpsUniformResourceIdentifierBuilder : ICommonBuilder<HttpsUniformResourceIdentifierBuilder>
    {
        private string _host, _port, _query, _fragment;
        private readonly PathSegments _pathSegments = new PathSegments();

        /// <summary>
        /// Constructs an empty builder.
        /// </summary>
        public HttpsUniformResourceIdentifierBuilder() { }

        /// <summary>
        /// Constructs a builder from an existing HTTPS URI.
        /// </summary>
        /// <param name="uri">The URI used to set the builder's initial values.</param>
        public HttpsUniformResourceIdentifierBuilder(HttpsUniformResourceIdentifier uri)
        {
            BuilderUtils.ApplyUriReference(this, uri);
        }

        /// <summary>
        /// Parses a URI string and constructs a builder.
        /// </summary>
        /// <param name="uri">The URI used to set the builder's initial values.</param>
        public HttpsUniformResourceIdentifierBuilder(string uri)
        {
            BuilderUtils.ApplyUriReference(this, uri, HttpsUniformResourceIdentifier.HttpsScheme);
        }

        HttpsUniformResourceIdentifierBuilder IBuilderWithUserInfo<HttpsUniformResourceIdentifierBuilder>.WithUserInfo(string userInfo)
        {
            if (userInfo != null)
                throw new InvalidOperationException("HTTP/HTTPS URIs can no longer have UserInfo portions, as of RFC7230.");
            return this;
        }

        /// <inheritdoc />
        public HttpsUniformResourceIdentifierBuilder WithHost(string host)
        {
            _host = host;
            return this;
        }

        /// <inheritdoc />
        public HttpsUniformResourceIdentifierBuilder WithPort(string port)
        {
            _port = port;
            return this;
        }

        /// <inheritdoc />
        public HttpsUniformResourceIdentifierBuilder WithPrefixlessPathSegments(IEnumerable<string> pathSegments)
        {
            _pathSegments.Set(pathSegments);
            return this;
        }

        /// <inheritdoc />
        public HttpsUniformResourceIdentifierBuilder WithQuery(string query)
        {
            _query = query;
            return this;
        }

        /// <inheritdoc />
        public HttpsUniformResourceIdentifierBuilder WithFragment(string fragment)
        {
            _fragment = fragment;
            return this;
        }

        /// <summary>
        /// Builds the HTTPS URI instance.
        /// </summary>
        public HttpsUniformResourceIdentifier Build() => new HttpsUniformResourceIdentifier(_host, _port, _pathSegments.Value, _query, _fragment);
    }
}
