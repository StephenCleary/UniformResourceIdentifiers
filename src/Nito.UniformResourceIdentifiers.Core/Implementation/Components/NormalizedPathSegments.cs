using System;
using System.Collections.Generic;
using System.Linq;

namespace Nito.UniformResourceIdentifiers.Implementation.Components
{
    /// <summary>
    /// Container for a normalized path value.
    /// </summary>
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public struct NormalizedPathSegments
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
        /// <summary>
        /// Validates and normalizes a path.
        /// </summary>
        public NormalizedPathSegments(IEnumerable<string> pathSegments, string? userInfo, string? host, string? port, bool dotNormalize = true)
        {
            _ = pathSegments ?? throw new ArgumentNullException(nameof(pathSegments));
            var segments = pathSegments.ToList();
            if (segments.Any(x => x == null))
                throw new ArgumentException("Path contains null segments", nameof(pathSegments));
            if (userInfo != null || host != null || port != null)
            {
                if (!string.IsNullOrEmpty(segments.FirstOrDefault()))
                    throw new ArgumentException("URI with authority must have an absolute path", nameof(pathSegments));
            }
            Value = dotNormalize ? Utility.RemoveDotSegments(segments) : segments;
        }

        /// <summary>
        /// The normalized path.
        /// </summary>
        public IReadOnlyList<string> Value { get; }
    }
}
