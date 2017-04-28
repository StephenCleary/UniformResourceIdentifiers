using System.Collections.Generic;

namespace Nito.UniformResourceIdentifiers.Implementation.Builder
{
    /// <summary>
    /// A builder that allows specifying path segments.
    /// </summary>
    /// <typeparam name="T">The type of the builder.</typeparam>
    public interface IBuilderWithPath<out T>
    {
        /// <summary>
        /// Applies the path to this builder, overwriting any existing path. This method does not automatically prefix a forward slash to the resulting path.
        /// </summary>
        /// <param name="pathSegments">The path segments. May not be <c>null</c>.</param>
        T WithPrefixlessPathSegments(IEnumerable<string> pathSegments);
    }
}