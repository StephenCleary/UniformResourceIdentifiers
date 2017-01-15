using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nito.UniformResourceIdentifiers.Helpers;

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// A URI builder for HTTP URIs.
    /// </summary>
    public sealed class HttpUniformResourceIdentifierBuilder : HttpUniformResourceIdentifierBuilderBase<HttpUniformResourceIdentifierBuilder>
    {
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
            ApplyUriReference(uri);
        }

        /// <summary>
        /// Parses a URI string and constructs a builder.
        /// </summary>
        /// <param name="uri">The URI used to set the builder's initial values.</param>
        public HttpUniformResourceIdentifierBuilder(string uri)
        {
            ApplyUriReference(uri, HttpUniformResourceIdentifier.HttpScheme);
        }

        /// <summary>
        /// Builds the HTTP URI instance.
        /// </summary>
        public HttpUniformResourceIdentifier Build() => new HttpUniformResourceIdentifier(Host, Port, PathSegments, Query, Fragment);
    }
}
