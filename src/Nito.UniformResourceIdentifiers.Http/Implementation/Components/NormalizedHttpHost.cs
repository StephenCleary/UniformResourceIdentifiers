using System;

namespace Nito.UniformResourceIdentifiers.Implementation.Components
{
    /// <summary>
    /// Container for a normalized host value.
    /// </summary>
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public struct NormalizedHttpHost
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
        private readonly NormalizedHost _host;

        /// <summary>
        /// Normalizes a host value.
        /// </summary>
        public NormalizedHttpHost(string? host)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));
            if (host.Length == 0)
                throw new ArgumentException("Host is required.", nameof(host));
            _host = new NormalizedHost(host);
        }

        /// <summary>
        /// The normalized host.
        /// </summary>
        public string? Value => _host.Value;
    }
}