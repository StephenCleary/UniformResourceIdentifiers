﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// An immutable URI reference (either a URI or a relative reference). If the URI reference is not a relative reference, then it is also normalized as much as possible.
    /// </summary>
    public interface IUniformResourceIdentifierReference
    {
        /// <summary>
        /// Gets the user info portion of the authority of this URI, e.g., "username:password". This can be <c>null</c> if there is no user info, or an empty string if the user info is empty.
        /// </summary>
        string? UserInfo { get; }

        /// <summary>
        /// Gets the host portion of the authority of this URI, e.g., "www.example.com". This can be <c>null</c> if there is no host, or an empty string if the host is empty.
        /// </summary>
        string? Host { get; }

        /// <summary>
        /// Gets the port portion of the authority of this URI, e.g., "8080". This can be <c>null</c> if there is no port, or an empty string if the port is empty. Any string returned from this property is a numeric string.
        /// </summary>
        string? Port { get; }

        /// <summary>
        /// Gets the path segments of the URI, e.g., { "", "folder", "subfolder", "file.jpg" }. This can never be <c>null</c>, but it can be empty. Note that for some schemes, it is common for the first path segment to be the empty string to generate an initial forward-slash.
        /// </summary>
        IReadOnlyList<string> PathSegments { get; }

        /// <summary>
        /// Gets the query of the URI, e.g., "q=test&amp;page=4". This can be <c>null</c> if there is no query, or an empty string if the query is empty.
        /// </summary>
        string? Query { get; }

        /// <summary>
        /// Gets the fragment of the URI, e.g., "anchor-1". This can be <c>null</c> if there is no fragment, or an empty string if the fragment is empty.
        /// </summary>
        string? Fragment { get; }

        /// <summary>
        /// Gets the URI as a complete string, e.g., "http://username:password@www.example.com:8080/folder/subfolder/file.jpg?q=test&amp;page=4#anchor-1". This is never <c>null</c> or an empty string.
        /// </summary>
        string ToString(); // => Utility.ToString(Scheme, UserInfo, Host, Port, PathSegments, Query, Fragment);
    }
}
