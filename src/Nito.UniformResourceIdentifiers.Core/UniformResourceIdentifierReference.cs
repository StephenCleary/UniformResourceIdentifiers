using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nito.UniformResourceIdentifiers.Implementation;

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// Utility methods for URI references.
    /// </summary>
    public static class UniformResourceIdentifierReference
    {
        /// <summary>
        /// Parses a URI reference. If the URI's scheme is not registered, then this will return an instance of <see cref="GenericUniformResourceIdentifier"/>.
        /// </summary>
        /// <param name="uriReference">The URI reference to parse.</param>
        public static IUniformResourceIdentifierReference Parse(string uriReference)
        {
            Parser.ParseUriReference(uriReference, out string scheme, out string userInfo, out string host, out string port,
                out IReadOnlyList<string> pathSegments, out string query, out string fragment);
            return Factories.Create(scheme, userInfo, host, port, pathSegments, query, fragment);
        }
    }
}
