using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nito.UniformResourceIdentifiers.Implementation;
using Nito.UniformResourceIdentifiers.Implementation.Builder;

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Applies the port portion of the authority to this builder, overwriting any existing port.
        /// </summary>
        /// <param name="this">The builder.</param>
        /// <param name="port">The port.</param>
        public static T WithPort<T>(this IBuilderWithPort<T> @this, uint port)
        {
            _ = @this ?? throw new ArgumentNullException(nameof(@this));
            return @this.WithPort(port.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Applies the path to this builder, overwriting any existing path. This method does not automatically prefix a forward slash to the resulting path.
        /// </summary>
        /// <param name="this">The builder.</param>
        /// <param name="segments">The path segments.</param>
        public static T WithPrefixlessPathSegments<T>(this IBuilderWithPath<T> @this, params string[] segments)
        {
            _ = @this ?? throw new ArgumentNullException(nameof(@this));
            return @this.WithPrefixlessPathSegments(segments);
        }

        /// <summary>
        /// Applies the path to this builder, overwriting any existing path. This method automatically prefixes a forward slash to the resulting path.
        /// </summary>
        /// <param name="this">The builder.</param>
        /// <param name="segments">The path segments.</param>
        public static T WithPathSegments<T>(this IBuilderWithPath<T> @this, IEnumerable<string> segments)
        {
            _ = @this ?? throw new ArgumentNullException(nameof(@this));
            _ = segments ?? throw new ArgumentNullException(nameof(segments));
            return @this.WithPrefixlessPathSegments(Enumerable.Repeat("", 1).Concat(segments));
        }

        /// <summary>
        /// Applies the path to this builder, overwriting any existing path. This method automatically prefixes a forward slash to the resulting path.
        /// </summary>
        /// <param name="this">The builder.</param>
        /// <param name="segments">The path segments.</param>
        public static T WithPathSegments<T>(this IBuilderWithPath<T> @this, params string[] segments) => @this.WithPathSegments((IEnumerable<string>)segments);

        /// <summary>
        /// Converts to a <see cref="Uri"/>.
        /// </summary>
        public static Uri ToUri(this IUniformResourceIdentifier @this)
        {
            _ = @this ?? throw new ArgumentNullException(nameof(@this));
            return new Uri(@this.ToString(), UriKind.Absolute);
        }

        /// <summary>
        /// Converts to a <see cref="Uri"/>.
        /// </summary>
        public static Uri ToUri(this IUniformResourceIdentifierReference @this)
        {
            if (@this is RelativeReference relativeUri)
                return relativeUri.ToUri();
            return ((IUniformResourceIdentifier) @this).ToUri();
        }
    }
}
