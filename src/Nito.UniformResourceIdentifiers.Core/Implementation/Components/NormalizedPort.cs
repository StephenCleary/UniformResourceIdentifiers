using System;

namespace Nito.UniformResourceIdentifiers.Implementation.Components
{
    /// <summary>
    /// Container for a normalized port value.
    /// </summary>
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public struct NormalizedPort
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
        /// <summary>
        /// Validates and normalizes a port value.
        /// </summary>
        public NormalizedPort(string? port)
        {
            if (port != null && !Utility.IsValidPort(port))
                throw new ArgumentException("Invalid port " + port, nameof(port));
            Value = NormalizePort(port);
        }

        /// <summary>
        /// The normalized port.
        /// </summary>
        public string? Value { get; }

        /// <summary>
        /// Removes leading zeroes from the port string, but leaves one if the port is 0.
        /// </summary>
        /// <param name="port">The port string.</param>
        private static string? NormalizePort(string? port)
        {
            if (string.IsNullOrEmpty(port))
                return port;
            var result = port!.TrimStart('0');
            return result.Length == 0 ? "0" : result;
        }
    }
}