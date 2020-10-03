using System.Collections.Generic;
using System.Linq;

namespace Nito.UniformResourceIdentifiers.Implementation.Components
{
    /// <summary>
    /// Container for a normalized path value.
    /// </summary>
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public struct NormalizedHttpPathSegments
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
        private readonly NormalizedPathSegments _pathSegments;

        /// <summary>
        /// Validates and normalizes a path.
        /// </summary>
        public NormalizedHttpPathSegments(IEnumerable<string> pathSegments, string? userInfo, string? host, string? port)
        {
            _pathSegments = new NormalizedPathSegments(NormalizePath(pathSegments), userInfo, host, port);
        }

        private static IEnumerable<string> NormalizePath(IEnumerable<string> pathSegments)
        {
            var first = true;
            foreach (var segment in pathSegments ?? Enumerable.Empty<string>())
            {
                if (first)
                {
                    if (segment.Length != 0)
                        yield return "";
                    yield return segment;
                    first = false;
                }
                else
                    yield return segment;
            }
            if (first)
                yield return "";
        }

        /// <summary>
        /// The normalized path.
        /// </summary>
        public IReadOnlyList<string> Value => _pathSegments.Value;
    }
}
