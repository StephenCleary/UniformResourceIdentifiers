﻿using System;
using System.Collections.Generic;
using Nito.Comparers;
using Nito.Comparers.Util;
using Nito.UniformResourceIdentifiers.Implementation;
using Nito.UniformResourceIdentifiers.Implementation.Components;

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// A URI with an unrecognized scheme. This URI is normalized as an arbitrary URI, but will not be fully normalized with respect to its scheme.
    /// </summary>
    public sealed class GenericUniformResourceIdentifier : ComparableBase<GenericUniformResourceIdentifier>, IUniformResourceIdentifier
    {
        private readonly NormalizedHost _host;
        private readonly NormalizedPathSegments _pathSegments;
        private readonly NormalizedPort _port;
        private readonly Utility.DelegateFactory<GenericUniformResourceIdentifier> _factory;

        static GenericUniformResourceIdentifier()
        {
            DefaultComparer = UniformResourceIdentifier.Comparer;
        }

        /// <summary>
        /// Constructs a new URI instance.
        /// </summary>
        /// <param name="scheme">The scheme. This is converted to lowercase. Must be a valid scheme as defined by <see cref="Utility.IsValidScheme"/>.</param>
        /// <param name="userInfo">The user information portion of the authority, if any. This may be <c>null</c> to indicate no user info, or the empty string to indicate empty user info.</param>
        /// <param name="host">The host name portion of the authority, if any. This is converted to lowercase. This may be <c>null</c> to indicate no host name, or the empty string to indicate an empty host name.</param>
        /// <param name="port">The port portion of the authority, if any. This may be <c>null</c> to indicate no port, or the empty string to indicate an empty port. This must be <c>null</c>, the empty string, or a valid port as defined by <see cref="Utility.IsValidPort"/>.</param>
        /// <param name="pathSegments">The path segments. Dot segments are normalized. May not be <c>null</c>, neither may any element be <c>null</c>.</param>
        /// <param name="query">The query. This may be <c>null</c> to indicate no query, or the empty string to indicate an empty query.</param>
        /// <param name="fragment">The fragment. This may be <c>null</c> to indicate no fragment, or the empty string to indicate an empty fragment.</param>
        public GenericUniformResourceIdentifier(string scheme, string? userInfo, string? host, string? port, IEnumerable<string> pathSegments, string? query, string? fragment)
        {
            _ = scheme ?? throw new ArgumentNullException(nameof(scheme));
            if (!Utility.IsValidScheme(scheme))
                throw new ArgumentException("Invalid scheme " + scheme, nameof(scheme));
            Scheme = scheme.ToLowerInvariant();
            UserInfo = userInfo;
            _host = new NormalizedHost(host);
            _port = new NormalizedPort(port);
            _pathSegments = new NormalizedPathSegments(pathSegments, UserInfo, _host.Value, _port.Value);
            Query = query;
            Fragment = fragment;

            _factory = CreateFactory(scheme);
        }

        private static Utility.DelegateFactory<GenericUniformResourceIdentifier> CreateFactory(string scheme)
        {
            return (userInfo, host, port, pathSegments, query, fragment) =>
                BuilderUtil.ApplyUriReference(new GenericUniformResourceIdentifierBuilder().WithScheme(scheme), userInfo, host, port, pathSegments, query, fragment).Build();
        }

        /// <inheritdoc />
        public string Scheme { get; }

        /// <inheritdoc />
        public string? UserInfo { get; }

        /// <inheritdoc />
        public string? Host => _host.Value;

        /// <inheritdoc />
        public string? Port => _port.Value;

        /// <inheritdoc />
        public IReadOnlyList<string> PathSegments => _pathSegments.Value;

        /// <inheritdoc />
        public string? Query { get; }

        /// <inheritdoc />
        public string? Fragment { get; }

        /// <inheritdoc />
        bool IEquatable<IUniformResourceIdentifier>.Equals(IUniformResourceIdentifier other) => ComparableImplementations.ImplementEquals(DefaultComparer, this, other);

        /// <inheritdoc />
        int IComparable<IUniformResourceIdentifier>.CompareTo(IUniformResourceIdentifier other) => ComparableImplementations.ImplementCompareTo(DefaultComparer, this, other);

        /// <inheritdoc />
        public override string ToString() => Utility.ToString(Scheme, null, Host, Port, PathSegments, Query, Fragment);

        /// <summary>
        /// Resolves a relative URI against this URI.
        /// </summary>
        /// <param name="relativeUri">The relative URI to resolve.</param>
        public GenericUniformResourceIdentifier Resolve(RelativeReference relativeUri) => Utility.Resolve(this, relativeUri, _factory);

        IUniformResourceIdentifier IUniformResourceIdentifier.Resolve(RelativeReference relativeUri) => Resolve(relativeUri);

        /// <inheritdoc />
        public IUniformResourceIdentifier Resolve(IUniformResourceIdentifierReference referenceUri) => Utility.Resolve(this, referenceUri, _factory);

        /// <summary>
        /// Parses a URI.
        /// </summary>
        /// <param name="uri">The URI to parse.</param>
        public static GenericUniformResourceIdentifier Parse(string uri) => new GenericUniformResourceIdentifierBuilder(uri).Build();
    }
}
