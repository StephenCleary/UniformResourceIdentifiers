using System;
using System.Collections.Generic;
using System.Text;

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// A URI. For a given scheme, if some members of this type are not ever used, then they should be implemented implicitly.
    /// </summary>
    public interface IUniformResourceIdentifier : IUniformResourceIdentifierReference
    {
    }
}
