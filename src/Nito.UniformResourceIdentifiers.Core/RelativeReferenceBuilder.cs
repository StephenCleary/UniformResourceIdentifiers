using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nito.UniformResourceIdentifiers.Helpers;

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// Builder for relative references.
    /// </summary>
    public sealed class RelativeReferenceBuilder : UniformResourceIdentifierBuilderBase<RelativeReferenceBuilder>
    {
        /// <summary>
        /// Constructs an empty builder.
        /// </summary>
        public RelativeReferenceBuilder() { }

        /// <summary>
        /// Constructs a builder from an existing relative reference.
        /// </summary>
        /// <param name="relativeReference">The relative reference used to set the builder's initial values.</param>
        public RelativeReferenceBuilder(RelativeReference relativeReference)
        {
            ApplyUriReference(relativeReference);
        }

        /// <summary>
        /// Parses a relative reference string and constructs a builder.
        /// </summary>
        /// <param name="relativeReference">The relative reference used to set the builder's initial values.</param>
        public RelativeReferenceBuilder(string relativeReference)
        {
            ApplyUriReference(relativeReference, null);
        }

        /// <summary>
        /// Builds the relative reference.
        /// </summary>
        public RelativeReference Build() => new RelativeReference(UserInfo, Host, Port, PathSegments, Query, Fragment);
    }
}
