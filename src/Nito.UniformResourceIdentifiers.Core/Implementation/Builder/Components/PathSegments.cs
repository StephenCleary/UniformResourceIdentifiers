using System;
using System.Collections.Generic;

namespace Nito.UniformResourceIdentifiers.Implementation.Builder.Components
{
    /// <summary>
    /// Holds the path segments for a builder.
    /// </summary>
    public sealed class PathSegments
    {
        /// <summary>
        /// Updates the path segments to a new value. 
        /// </summary>
        /// <param name="pathSegments">The new path segments. This enumerable may safely refer to the existing value.</param>
        public void Set(IEnumerable<string> pathSegments)
        {
            if (pathSegments == null)
                throw new ArgumentNullException(nameof(pathSegments));
            Value = new List<string>(pathSegments);
        }

        /// <summary>
        /// The path segments. This is never <c>null</c>, but may be empty.
        /// </summary>
        public IReadOnlyList<string> Value { get; private set; } = new List<string>();
    }
}
