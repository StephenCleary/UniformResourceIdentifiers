using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// Builder for relative references.
    /// </summary>
    public sealed class RelativeReferenceBuilder : UniformResourceIdentifierBuilder<RelativeReferenceBuilder>
    {
        /// <summary>
        /// Builds the relative reference.
        /// </summary>
        public RelativeReference Build() => new RelativeReference(UserInfo, Host, Port, PathSegments, Query, Fragment);
    }
}
