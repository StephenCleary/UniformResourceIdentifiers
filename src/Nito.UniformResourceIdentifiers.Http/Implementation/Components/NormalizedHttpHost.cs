using System;

namespace Nito.UniformResourceIdentifiers.Implementation.Components
{
    /// <summary>
    /// Container for a normalized host value.
    /// </summary>
    public struct NormalizedHttpHost
    {
        private readonly NormalizedHost _host;

        /// <summary>
        /// Normalizes a host value.
        /// </summary>
        public NormalizedHttpHost(string host)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));
            if (host == "")
                throw new ArgumentException("Host is required.", nameof(host));
            _host = new NormalizedHost(host);
        }

        /// <summary>
        /// The normalized host.
        /// </summary>
        public string Value => _host.Value;
    }
}