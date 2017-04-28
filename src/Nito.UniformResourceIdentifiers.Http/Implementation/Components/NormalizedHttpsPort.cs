namespace Nito.UniformResourceIdentifiers.Implementation.Components
{
    /// <summary>
    /// Container for a normalized port value.
    /// </summary>
    public struct NormalizedHttpsPort
    {
        private readonly NormalizedPort _port;

        /// <summary>
        /// Normalizes a host value.
        /// </summary>
        public NormalizedHttpsPort(string port)
        {
            _port = new NormalizedPort(NormalizePort(port));
        }

        private static string NormalizePort(string port) => port == "" || port == "443" ? null : port;

        /// <summary>
        /// The normalized port.
        /// </summary>
        public string Value => _port.Value;
    }
}