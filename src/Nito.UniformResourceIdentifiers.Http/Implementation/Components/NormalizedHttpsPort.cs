namespace Nito.UniformResourceIdentifiers.Implementation.Components
{
    /// <summary>
    /// Container for a normalized port value.
    /// </summary>
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public struct NormalizedHttpsPort
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
        private readonly NormalizedPort _port;

        /// <summary>
        /// Normalizes a host value.
        /// </summary>
        public NormalizedHttpsPort(string? port)
        {
            _port = new NormalizedPort(NormalizePort(port));
        }

        private static string? NormalizePort(string? port) => string.IsNullOrEmpty(port) || port == "443" ? null : port;

        /// <summary>
        /// The normalized port.
        /// </summary>
        public string? Value => _port.Value;
    }
}