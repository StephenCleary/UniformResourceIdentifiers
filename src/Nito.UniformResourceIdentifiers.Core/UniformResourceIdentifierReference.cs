using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nito.UniformResourceIdentifiers.Helpers;

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// Utility methods for URI references.
    /// </summary>
    public static class UniformResourceIdentifierReference
    {
        /// <summary>
        /// Parses a URI reference. If the URI's scheme is not registered, then this will return an instance of <see cref="Unknown.UnknownUniformResourceIdentifier"/>.
        /// </summary>
        /// <param name="uriReference">The URI reference to parse.</param>
        public static IUniformResourceIdentifierReference Parse(string uriReference)
        {
            string scheme, userInfo, host, port, query, fragment;
            IReadOnlyList<string> pathSegments;
            Parser.ParseUriReference(uriReference, out scheme, out userInfo, out host, out port, out pathSegments, out query, out fragment);
            return Factories.Create(scheme, userInfo, host, port, pathSegments, query, fragment);
        }
    }
}
