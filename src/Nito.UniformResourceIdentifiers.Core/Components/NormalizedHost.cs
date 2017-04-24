namespace Nito.UniformResourceIdentifiers.Components
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
        /// The noramlized host.
        /// </summary>
        public string Value { get; }
    }
}