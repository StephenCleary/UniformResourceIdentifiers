namespace Nito.UniformResourceIdentifiers.Implementation.Components
{
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
        /// The normalized host.
        /// </summary>
        public string Value { get; }
    }
}