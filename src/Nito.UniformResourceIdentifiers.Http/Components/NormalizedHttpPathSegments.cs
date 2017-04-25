using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nito.UniformResourceIdentifiers.Helpers;

namespace Nito.UniformResourceIdentifiers.Components
{
    /// <summary>
    /// Container for a normalized path value.
    /// </summary>
    public struct NormalizedHttpPathSegments
    {
        private readonly NormalizedPathSegments _pathSegments;

        /// <summary>
        /// Validates and noramlizes a path.
        /// </summary>
        public NormalizedHttpPathSegments(IEnumerable<string> pathSegments, string userInfo, string host, string port)
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
                    if (segment != "")
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
