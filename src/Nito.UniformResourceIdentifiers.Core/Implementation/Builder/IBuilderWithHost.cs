namespace Nito.UniformResourceIdentifiers.Implementation.Builder
{
    /// <summary>
    /// A builder that allows specifying a host.
    /// </summary>
    /// <typeparam name="T">The type of the builder.</typeparam>
    public interface IBuilderWithHost<out T>
    {
        /// <summary>
        /// Applies the host portion of the authority to this builder, overwriting any existing host.
        /// </summary>
        /// <param name="host">The host. May be <c>null</c> or the empty string.</param>
        T WithHost(string? host);
    }
}