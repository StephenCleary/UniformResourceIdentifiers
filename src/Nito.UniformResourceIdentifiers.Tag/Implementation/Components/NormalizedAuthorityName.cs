using System;

namespace Nito.UniformResourceIdentifiers.Implementation.Components
{
    /// <summary>
    /// Container for a normalized authority name.
    /// </summary>
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public struct NormalizedAuthorityName
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
        /// <summary>
        /// Normalizes an authority name.
        /// </summary>
        public NormalizedAuthorityName(string? authorityName)
        {
            if (authorityName == null)
                throw new ArgumentNullException(nameof(authorityName));
            if (authorityName.Length == 0)
                throw new ArgumentException("Authority name is required.", nameof(authorityName));
            Value = authorityName;
        }

        /// <summary>
        /// The normalized authority name.
        /// </summary>
        public string Value { get; }
    }
}