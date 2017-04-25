namespace Nito.UniformResourceIdentifiers.Implementation.Builder
{
    /// <summary>
    /// A builder that allows specifying user info.
    /// </summary>
    /// <typeparam name="T">The type of the builder.</typeparam>
    public interface IBuilderWithUserInfo<out T>
    {
        /// <summary>
        /// Applies the user information portion of the authority to this builder, overwriting any existing user information. Note that user information is deprecated for most schemes, since it is inherently insecure.
        /// </summary>
        /// <param name="userInfo">The user information. May be <c>null</c> or the empty string.</param>
        T WithUserInfo(string userInfo);
    }
}