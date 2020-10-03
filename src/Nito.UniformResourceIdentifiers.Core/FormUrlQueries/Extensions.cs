using System;
using System.Collections.Generic;
using System.Text;
using Nito.UniformResourceIdentifiers.Implementation;
using Nito.UniformResourceIdentifiers.Implementation.Builder;

namespace Nito.UniformResourceIdentifiers.FormUrlQueries
{
    /// <summary>
    /// Provides helper methods for treating the <c>Query</c> as <c>application/x-www-form-urlencoded</c> values.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Parses the query of the URI as a series of name/value pairs, e.g., "q=test&amp;page=4". This can be <c>null</c> if there is no query, or an empty collection if the query is empty. Names can be empty but never <c>null</c>; values can be <c>null</c> (if there is no <c>=</c>) or empty.
        /// </summary>
        public static IEnumerable<KeyValuePair<string, string>>? QueryValues(this IUniformResourceIdentifierReference @this)
        {
            _ = @this ?? throw new ArgumentNullException(nameof(@this));
            return @this.Query == null ? null : Utility.FormUrlDecodeValues(@this.Query);
        }

        /// <summary>
        /// Applies the query string to this builder, overwriting any existing query.
        /// </summary>
        /// <param name="this">The builder.</param>
        /// <param name="values">The query values to encode.</param>
        public static T WithQueryValues<T>(this IBuilderWithQuery<T> @this, IEnumerable<KeyValuePair<string, string>> values)
        {
            _ = @this ?? throw new ArgumentNullException(nameof(@this));
            return @this.WithQuery(Utility.FormUrlEncode(values));
        }

        // TODO: Fragment path segment support? Other query/fragment support?

    }
}
