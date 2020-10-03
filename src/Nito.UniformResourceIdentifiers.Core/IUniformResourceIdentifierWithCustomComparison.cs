using System;
using System.Collections.Generic;
using System.Text;

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// A URI representing a scheme with custom comparison logic.
    /// </summary>
    public interface IUniformResourceIdentifierWithCustomComparison: IUniformResourceIdentifier
    {
        /// <summary>
        /// Gets a proxy object used for comparison. If this is <c>null</c>, then a generic URI comparison is performed.
        /// </summary>
        object? SchemeSpecificComparerProxy { get; }
    }
}
