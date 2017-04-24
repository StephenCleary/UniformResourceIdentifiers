namespace Nito.UniformResourceIdentifiers.Builder
{
    /// <summary>
    /// A builder that allows specifying a port.
    /// </summary>
    /// <typeparam name="T">The type of the builder.</typeparam>
    public interface IBuilderWithPort<out T>
    {
        /// <summary>
        /// Applies the port portion of the authority to this builder, overwriting any existing port.
        /// </summary>
        /// <param name="port">The port. May be <c>null</c> or the empty string.</param>
        T WithPort(string port);
    }
}