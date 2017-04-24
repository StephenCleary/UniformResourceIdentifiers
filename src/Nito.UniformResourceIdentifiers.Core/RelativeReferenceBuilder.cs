using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nito.UniformResourceIdentifiers.BuilderComponents;
using Nito.UniformResourceIdentifiers.Helpers;

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// Builder for relative references.
    /// </summary>
    public sealed class RelativeReferenceBuilder : ICommonBuilder<RelativeReferenceBuilder>
    {
        private string _userInfo, _host, _port, _query, _fragment;
        private readonly PathSegments _pathSegments = new PathSegments();

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
            BuilderUtils.ApplyUriReference(this, relativeReference);
        }

        /// <summary>
        /// Parses a relative reference string and constructs a builder.
        /// </summary>
        /// <param name="relativeReference">The relative reference used to set the builder's initial values.</param>
        public RelativeReferenceBuilder(string relativeReference)
        {
            BuilderUtils.ApplyUriReference(this, relativeReference, null);
        }

        /// <summary>
        /// Builds the relative reference.
        /// </summary>
        public RelativeReference Build() => new RelativeReference(_userInfo, _host, _port, _pathSegments.Value, _query, _fragment);

        RelativeReferenceBuilder IBuilderWithUserInfo<RelativeReferenceBuilder>.WithUserInfo(string userInfo)
        {
            _userInfo = userInfo;
            return this;
        }

        /// <summary>
        /// Applies the host portion of the authority to this builder, overwriting any existing host.
        /// </summary>
        /// <param name="host">The host. May be <c>null</c> or the empty string.</param>
        public RelativeReferenceBuilder WithHost(string host)
        {
            _host = host;
            return this;
        }

        /// <summary>
        /// Applies the port portion of the authority to this builder, overwriting any existing port.
        /// </summary>
        /// <param name="port">The port. May be <c>null</c> or the empty string.</param>
        public RelativeReferenceBuilder WithPort(string port)
        {
            _port = port;
            return this;
        }

        /// <summary>
        /// Applies the path to this builder, overwriting any existing path. This method does not automatically prefix a forward slash to the resulting path.
        /// </summary>
        /// <param name="pathSegments">The path segments.</param>
        public RelativeReferenceBuilder WithPrefixlessPathSegments(IEnumerable<string> pathSegments)
        {
            _pathSegments.Set(pathSegments);
            return this;
        }

        /// <summary>
        /// Applies the query string to this builder, overwriting any existing query.
        /// </summary>
        /// <param name="query">The query.</param>
        public RelativeReferenceBuilder WithQuery(string query)
        {
            _query = query;
            return this;
        }

        /// <summary>
        /// Applies the fragment string to this builder, overwriting any existing fragment.
        /// </summary>
        /// <param name="fragment">The fragment.</param>
        public RelativeReferenceBuilder WithFragment(string fragment)
        {
            _fragment = fragment;
            return this;
        }
    }
}
