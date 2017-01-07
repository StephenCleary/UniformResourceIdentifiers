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
    public sealed class HttpsUniformResourceIdentifierBuilder : HttpUniformResourceIdentifierBuilderBase
    {
        /// <summary>
        /// Builds the HTTPS URI instance.
        /// </summary>
        public HttpsUniformResourceIdentifier Build() => new HttpsUniformResourceIdentifier(Host, Port, PathSegments, Query, Fragment);
    }
}
