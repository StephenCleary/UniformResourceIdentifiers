using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nito.UniformResourceIdentifiers.Builder;
using Nito.UniformResourceIdentifiers.Helpers;

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// Extension methods for builders.
    /// </summary>
    public static class BuilderExtensions
    {
        /// <summary>
        /// Applies the port portion of the authority to this builder, overwriting any existing port.
        /// </summary>
        /// <param name="this">The builder.</param>
        /// <param name="port">The port.</param>
        public static T WithPort<T>(this IBuilderWithPort<T> @this, uint port) => @this.WithPort(port.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Applies the path to this builder, overwriting any existing path. This method does not automatically prefix a forward slash to the resulting path.
        /// </summary>
        /// <param name="this">The builder.</param>
        /// <param name="segments">The path segments.</param>
        public static T WithPrefixlessPathSegments<T>(this IBuilderWithPath<T> @this, params string[] segments) =>
            @this.WithPrefixlessPathSegments(segments);

        /// <summary>
        /// Applies the path to this builder, overwriting any existing path. This method automatically prefixes a forward slash to the resulting path.
        /// </summary>
        /// <param name="this">The builder.</param>
        /// <param name="segments">The path segments.</param>
        public static T WithPathSegments<T>(this IBuilderWithPath<T> @this, IEnumerable<string> segments)
        {
            if (segments == null)
                throw new ArgumentNullException(nameof(segments));
            return @this.WithPrefixlessPathSegments(Enumerable.Repeat("", 1).Concat(segments));
        }

        /// <summary>
        /// Applies the path to this builder, overwriting any existing path. This method automatically prefixes a forward slash to the resulting path.
        /// </summary>
        /// <param name="this">The builder.</param>
        /// <param name="segments">The path segments.</param>
        public static T WithPathSegments<T>(this IBuilderWithPath<T> @this, params string[] segments) => @this.WithPathSegments((IEnumerable<string>)segments);

        /// <summary>
        /// Applies the query string to this builder, overwriting any existing query.
        /// </summary>
        /// <param name="this">The builder.</param>
        /// <param name="values">The query values to encode.</param>
        public static T WithQueryValues<T>(this IBuilderWithQuery<T> @this, IEnumerable<KeyValuePair<string, string>> values) => @this.WithQuery(Util.FormUrlEncode(values));

        // TODO: Fragment path segment support? Other query/fragment support?
    }
}
