using System;

namespace Nito.UniformResourceIdentifiers.Implementation.Components
{
    /// <summary>
    /// Container for a normalized authority name.
    /// </summary>
    public struct NormalizedAuthorityName
    {
        /// <summary>
        /// Normalizes an authority name.
        /// </summary>
        public NormalizedAuthorityName(string authorityName)
        {
            if (authorityName == null)
                throw new ArgumentNullException(nameof(authorityName));
            if (authorityName == "")
                throw new ArgumentException("Authority name is required.", nameof(authorityName));
            if (!TagUtil.IsValidAuthorityName(authorityName))
                throw new ArgumentException("Invalid authority name " + authorityName, nameof(authorityName));
            Value = authorityName;
        }

        /// <summary>
        /// The normalized authority name.
        /// </summary>
        public string Value { get; }
    }
}