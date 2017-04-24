namespace Nito.UniformResourceIdentifiers.Builder
{
    /// <summary>
    /// A builder that allows specifying a fragment string.
    /// </summary>
    /// <typeparam name="T">The type of the builder.</typeparam>
    public interface IBuilderWithFragment<out T>
    {
        /// <summary>
        /// Applies the fragment string to this builder, overwriting any existing fragment.
        /// </summary>
        /// <param name="fragment">The fragment.</param>
        T WithFragment(string fragment);
    }
}