﻿using System;
using System.Collections.Generic;

namespace Nito.UniformResourceIdentifiers.Implementation
{
    /// <summary>
    /// A factory registry for URI schemes.
    /// </summary>
    public static class Factories
    {
        private delegate IUniformResourceIdentifierReference FactoryDelegate(string? userInfo, string? host, string? port, IEnumerable<string> pathSegments, string? query, string? fragment);
        private static readonly Dictionary<string, FactoryDelegate> _factories = new Dictionary<string, FactoryDelegate>();

        /// <summary>
        /// Registers a factory for a scheme, overwriting any existing factory for that scheme.
        /// </summary>
        /// <typeparam name="T">The type of the URI.</typeparam>
        /// <param name="scheme">The scheme. This must be a valid scheme, as defined by <see cref="Utility.IsValidScheme"/>.</param>
        /// <param name="factory">The factory method for URIs following this scheme. This method must perform any scheme-specific validation.</param>
        public static void RegisterSchemeFactory<T>(string scheme, Utility.DelegateFactory<T> factory)
            where T : IUniformResourceIdentifier
        {
            if (scheme == null || !Utility.IsValidScheme(scheme))
                throw new ArgumentException("Invalid scheme " + scheme, nameof(scheme));

            lock (_factories)
            {
                _factories[scheme] = (userInfo, host, port, pathSegments, query, fragment) => factory(userInfo, host, port, pathSegments, query, fragment);
            }
        }

        /// <summary>
        /// Creates a URI or relative URI from its components. If the scheme is not registered, returns an instance of <see cref="GenericUniformResourceIdentifier"/>.
        /// </summary>
        /// <param name="scheme">The scheme. This may be <c>null</c>. If this is not <c>null</c>, then it must be a valid scheme.</param>
        /// <param name="userInfo">The user information.</param>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="pathSegments">The path segments.</param>
        /// <param name="query">The query string.</param>
        /// <param name="fragment">The fragment string.</param>
        public static IUniformResourceIdentifierReference Create(string? scheme, string? userInfo, string? host, string? port, IEnumerable<string> pathSegments, string? query, string? fragment)
        {
            if (scheme == null)
                return new RelativeReference(userInfo, host, port, pathSegments, query, fragment);
            FactoryDelegate factory;
            lock (_factories)
            {
                _factories.TryGetValue(scheme, out factory);
            }
            if (factory == null)
                return BuilderUtil.ApplyUriReference(new GenericUniformResourceIdentifierBuilder().WithScheme(scheme), userInfo, host, port, pathSegments, query, fragment).Build();
            else
                return factory(userInfo, host, port, pathSegments, query, fragment);
        }
    }
}
