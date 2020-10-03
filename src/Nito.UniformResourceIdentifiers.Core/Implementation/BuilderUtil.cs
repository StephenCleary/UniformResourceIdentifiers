using System;
using System.Collections.Generic;
using Nito.UniformResourceIdentifiers.Implementation.Builder;

namespace Nito.UniformResourceIdentifiers.Implementation
{
    /// <summary>
    /// Utility methods for builders.
    /// </summary>
    public static class BuilderUtil
    {
        /// <summary>
        /// Applies a URI reference into this builder.
        /// </summary>
        /// <param name="builder">The builder to modify.</param>
        /// <param name="userInfo">The user information. May be <c>null</c> or the empty string.</param>
        /// <param name="host">The host. May be <c>null</c> or the empty string.</param>
        /// <param name="port">The port. May be <c>null</c> or the empty string.</param>
        /// <param name="pathSegments">The path segments. May not be <c>null</c>.</param>
        /// <param name="query">The query. May be <c>null</c> or the empty string.</param>
        /// <param name="fragment">The fragment. May be <c>null</c> or the empty string.</param>
        public static T ApplyUriReference<T>(T builder, string? userInfo, string? host, string? port, IEnumerable<string> pathSegments, string? query, string? fragment)
            where T : ICommonBuilder<T> =>
            builder.WithUserInfo(userInfo).WithHost(host).WithPort(port).WithPrefixlessPathSegments(pathSegments).WithQuery(query).WithFragment(fragment);

        /// <summary>
        /// Deconstructs a URI reference into this builder.
        /// </summary>
        /// <param name="builder">The builder to modify.</param>
        /// <param name="uri">The URI reference to deconstruct.</param>
        public static T ApplyUriReference<T>(T builder, IUniformResourceIdentifierReference uri)
            where T : ICommonBuilder<T>
        {
            _ = uri ?? throw new ArgumentNullException(nameof(uri));
            return ApplyUriReference(builder, uri.UserInfo, uri.Host, uri.Port, uri.PathSegments, uri.Query, uri.Fragment);
        }

        /// <summary>
        /// Parses and deconstructs a URI reference into this builder. Returns the scheme of the URI reference, if any.
        /// </summary>
        /// <param name="builder">The builder to modify.</param>
        /// <param name="uri">The URI reference to deconstruct.</param>
        public static string? ApplyUriReference<T>(T builder, string uri)
            where T : ICommonBuilder<T>
        {
            Parser.ParseUriReference(uri, out var scheme, out var userInfo, out var host, out var port, out var pathSegements,
                out var query, out var fragment);
            ApplyUriReference(builder, userInfo, host, port, pathSegements, query, fragment);
            return scheme;
        }

        /// <summary>
        /// Parses and deconstructs a URI reference into this builder, and verifies that the URI scheme matches what was expected.
        /// </summary>
        /// <param name="builder">The builder to modify.</param>
        /// <param name="uri">The URI reference to deconstruct.</param>
        /// <param name="expectedScheme">The expected URI scheme. May be <c>null</c> to allow any URI scheme.</param>
        public static void ApplyUriReference<T>(T builder, string uri, string? expectedScheme)
            where T : ICommonBuilder<T>
        {
            var scheme = ApplyUriReference(builder, uri);
            if (scheme != expectedScheme && expectedScheme != null)
                throw new ArgumentException($"URI scheme \"{scheme}\" does not match expected scheme \"{expectedScheme ?? ""}\" in URI \"{uri}\".", nameof(uri));
        }
    }
}