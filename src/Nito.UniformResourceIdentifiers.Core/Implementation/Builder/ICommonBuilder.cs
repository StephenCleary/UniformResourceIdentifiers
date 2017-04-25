namespace Nito.UniformResourceIdentifiers.Implementation.Builder
{
    /// <summary>
    /// A builder with common options.
    /// </summary>
    /// <typeparam name="T">The type of the builder.</typeparam>
    public interface ICommonBuilder<out T> : IBuilderWithUserInfo<T>, IBuilderWithHost<T>, IBuilderWithPort<T>, IBuilderWithPath<T>, IBuilderWithQuery<T>, IBuilderWithFragment<T>
    {
    }
}