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
    public sealed class HttpUniformResourceIdentifierBuilder : HttpUniformResourceIdentifierBuilderBase
    {
        /// <summary>
        /// Builds the HTTP URI instance.
        /// </summary>
        public HttpUniformResourceIdentifier Build() => new HttpUniformResourceIdentifier(Host, Port, PathSegments, Query, Fragment);
    }
}
