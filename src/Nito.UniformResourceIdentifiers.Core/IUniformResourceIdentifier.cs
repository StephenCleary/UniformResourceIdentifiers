using System;
using System.Collections.Generic;
using System.Text;

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// A URI. For a given scheme, if some members of this type are not ever used, then they should be implemented implicitly.
    /// </summary>
    public interface IUniformResourceIdentifier : IUniformResourceIdentifierReference,
        IEquatable<IUniformResourceIdentifier>, IComparable, IComparable<IUniformResourceIdentifier>
    {
        /// <summary>
        /// Gets the scheme of this URI, e.g., "http". This can be <c>null</c> if there is no scheme. If not <c>null</c>, then this is always a valid scheme; it can never be the empty string.
        /// </summary>
        string Scheme { get; }

        /// <summary>
        /// Resolves a relative URI against this URI.
        /// </summary>
        /// <param name="relativeUri">The relative URI to resolve.</param>
        IUniformResourceIdentifier Resolve(RelativeReference relativeUri);

        /// <summary>
        /// Resolves a reference URI against this URI.
        /// </summary>
        /// <param name="referenceUri">The reference URI to resolve.</param>
        IUniformResourceIdentifier Resolve(IUniformResourceIdentifierReference referenceUri);
    }
}
