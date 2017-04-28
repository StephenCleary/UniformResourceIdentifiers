using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nito.UniformResourceIdentifiers.Implementation;
using Nito.UniformResourceIdentifiers.Implementation.Builder;
using Nito.UniformResourceIdentifiers.Implementation.Builder.Components;

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
            BuilderUtil.ApplyUriReference(this, relativeReference);
        }

        /// <summary>
        /// Parses a relative reference string and constructs a builder.
        /// </summary>
        /// <param name="relativeReference">The relative reference used to set the builder's initial values.</param>
        public RelativeReferenceBuilder(string relativeReference)
        {
            BuilderUtil.ApplyUriReference(this, relativeReference, null);
        }

        RelativeReferenceBuilder IBuilderWithUserInfo<RelativeReferenceBuilder>.WithUserInfo(string userInfo)
        {
            _userInfo = userInfo;
            return this;
        }

        /// <inheritdoc />
        public RelativeReferenceBuilder WithHost(string host)
        {
            _host = host;
            return this;
        }

        /// <inheritdoc />
        public RelativeReferenceBuilder WithPort(string port)
        {
            _port = port;
            return this;
        }

        /// <inheritdoc />
        public RelativeReferenceBuilder WithPrefixlessPathSegments(IEnumerable<string> pathSegments)
        {
            _pathSegments.Set(pathSegments);
            return this;
        }

        /// <inheritdoc />
        public RelativeReferenceBuilder WithQuery(string query)
        {
            _query = query;
            return this;
        }

        /// <inheritdoc />
        public RelativeReferenceBuilder WithFragment(string fragment)
        {
            _fragment = fragment;
            return this;
        }

        /// <summary>
        /// Builds the relative reference.
        /// </summary>
        public RelativeReference Build() => new RelativeReference(_userInfo, _host, _port, _pathSegments.Value, _query, _fragment);
    }
}
