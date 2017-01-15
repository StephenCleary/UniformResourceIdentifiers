using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nito.UniformResourceIdentifiers.Helpers;

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// A URI builder for HTTPS URIs.
    /// </summary>
    public sealed class HttpsUniformResourceIdentifierBuilder : HttpUniformResourceIdentifierBuilderBase<HttpsUniformResourceIdentifierBuilder>
    {
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
            ApplyUriReference(uri);
        }

        /// <summary>
        /// Parses a URI string and constructs a builder.
        /// </summary>
        /// <param name="uri">The URI used to set the builder's initial values.</param>
        public HttpsUniformResourceIdentifierBuilder(string uri)
        {
            ApplyUriReference(uri, HttpsUniformResourceIdentifier.HttpsScheme);
        }

        /// <summary>
        /// Builds the HTTPS URI instance.
        /// </summary>
        public HttpsUniformResourceIdentifier Build() => new HttpsUniformResourceIdentifier(Host, Port, PathSegments, Query, Fragment);
    }
}
