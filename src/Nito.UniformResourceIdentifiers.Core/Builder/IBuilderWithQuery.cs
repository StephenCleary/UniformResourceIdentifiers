namespace Nito.UniformResourceIdentifiers.Builder
{
    /// <summary>
    /// A builder that allows specifying a query string.
    /// </summary>
    /// <typeparam name="T">The type of the builder.</typeparam>
    public interface IBuilderWithQuery<out T>
    {
        /// <summary>
        /// Applies the query string to this builder, overwriting any existing query.
        /// </summary>
        /// <param name="query">The query.</param>
        T WithQuery(string query);
    }
}