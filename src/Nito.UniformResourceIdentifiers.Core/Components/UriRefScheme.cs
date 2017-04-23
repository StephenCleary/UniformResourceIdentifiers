using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Nito.UniformResourceIdentifiers.Helpers.Util;

namespace Nito.UniformResourceIdentifiers.Components
{
    /// <summary>
    /// Container for a normalized scheme value.
    /// </summary>
    public struct NormalizedScheme
    {
        /// <summary>
        /// Validates and normalizes a scheme value.
        /// </summary>
        public NormalizedScheme(string scheme)
        {
            if (scheme != null && !IsValidScheme(scheme))
                throw new ArgumentException("Invalid scheme " + scheme, nameof(scheme));
            Value = scheme?.ToLowerInvariant();
        }

        /// <summary>
        /// The normalized scheme.
        /// </summary>
        public string Value { get; }
    }

    /// <summary>
    /// Container for a normalized host value.
    /// </summary>
    public struct NormalizedHost
    {
        /// <summary>
        /// Normalizes a host value.
        /// </summary>
        public NormalizedHost(string host)
        {
            Value = host?.ToLowerInvariant();
        }

        /// <summary>
        /// The noramlized host.
        /// </summary>
        public string Value { get; }
    }

    /// <summary>
    /// Container for a normalized port value.
    /// </summary>
    public struct NormalizedPort
    {
        /// <summary>
        /// Validates and noramlizes a port value.
        /// </summary>
        public NormalizedPort(string port)
        {
            if (port != null && !IsValidPort(port))
                throw new ArgumentException("Invalid port " + port, nameof(port));
            Value = NormalizePort(port);
        }

        /// <summary>
        /// The noramlized port.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Removes leading zeroes from the port string, but leaves one if the port is 0.
        /// </summary>
        /// <param name="port">The port string.</param>
        private static string NormalizePort(string port)
        {
            if (string.IsNullOrEmpty(port))
                return port;
            var result = port.TrimStart('0');
            return result == "" ? "0" : result;
        }
    }

    /// <summary>
    /// Container for a normalized path value.
    /// </summary>
    public struct NormalizedPathSegments
    {
        /// <summary>
        /// Validates and noramlizes a path.
        /// </summary>
        public NormalizedPathSegments(IEnumerable<string> pathSegments, string userInfo, string host, string port)
        {
            if (pathSegments == null)
                throw new ArgumentNullException(nameof(pathSegments));
            var segments = pathSegments.ToList();
            if (segments.Any(x => x == null))
                throw new ArgumentException("Path contains null segments", nameof(pathSegments));
            if (userInfo != null || host != null || port != null)
            {
                if (!string.IsNullOrEmpty(segments.FirstOrDefault()))
                    throw new ArgumentException("URI with authority must have an absolute path", nameof(pathSegments));
            }
            Value = segments;
        }

        /// <summary>
        /// The noramlized path.
        /// </summary>
        public IReadOnlyList<string> Value { get; }
    }
}
