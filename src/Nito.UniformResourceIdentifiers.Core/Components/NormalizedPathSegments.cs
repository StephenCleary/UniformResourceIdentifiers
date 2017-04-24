using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nito.UniformResourceIdentifiers.Components
{
    /// <summary>
    /// Container for a normalized path value.
    /// </summary>
    public struct NormalizedPathSegments
    {
        /// <summary>
        /// Validates and noramlizes a path.
        /// </summary>
        public NormalizedPathSegments(IEnumerable<string> pathSegments, string userInfo, string host, string port)
        {
            if (pathSegments == null)
                throw new ArgumentNullException(nameof(pathSegments));
            var segments = pathSegments.ToList();
            if (segments.Any(x => x == null))
                throw new ArgumentException("Path contains null segments", nameof(pathSegments));
            if (userInfo != null || host != null || port != null)
            {
                if (!string.IsNullOrEmpty(segments.FirstOrDefault()))
                    throw new ArgumentException("URI with authority must have an absolute path", nameof(pathSegments));
            }
            Value = segments;
        }

        /// <summary>
        /// The noramlized path.
        /// </summary>
        public IReadOnlyList<string> Value { get; }
    }
}
