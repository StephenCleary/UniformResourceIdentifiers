using System;
using System.Collections.Generic;
using System.Text;

namespace Nito.UniformResourceIdentifiers.Implementation
{
    /// <summary>
    /// The validated, decoded results of parsing a URI.
    /// </summary>
    public sealed class UriParseResult
    {
        /// <summary>
        /// The scheme. This can be <c>null</c> if there is no scheme.
        /// </summary>
        public string Scheme;

        /// <summary>
        /// The user info portion of the authority. This can be <c>null</c> if there is no user info, or an empty string if the user info is empty.
        /// </summary>
        public string UserInfo;

        /// <summary>
        /// The host portion of the authority. This can be <c>null</c> if there is no host, or an empty string if the host is empty.
        /// </summary>
        public string Host;

        /// <summary>
        /// The port portion of the authority. This can be <c>null</c> if there is no port, or an empty string if the port is empty.
        /// </summary>
        public string Port;

        /// <summary>
        /// The path segments. This can never be <c>null</c>, but it can be empty.
        /// </summary>
        public IReadOnlyList<string> PathSegments;

        /// <summary>
        /// The query. This can be <c>null</c> if there is no query, or an empty string if the query is empty.
        /// </summary>
        public string Query;

        /// <summary>
        /// The fragment. This can be <c>null</c> if there is no fragment, or an empty string if the fragment is empty.
        /// </summary>
        public string Fragment;
    }
}
