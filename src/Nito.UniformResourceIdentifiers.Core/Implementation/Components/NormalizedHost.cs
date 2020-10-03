namespace Nito.UniformResourceIdentifiers.Implementation.Components
{
    /// <summary>
    /// Container for a normalized host value.
    /// </summary>
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public struct NormalizedHost
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
        /// <summary>
        /// Normalizes a host value.
        /// </summary>
        public NormalizedHost(string? host)
        {
            Value = host?.ToLowerInvariant();
        }

        /// <summary>
        /// The normalized host.
        /// </summary>
        public string? Value { get; }
    }
}