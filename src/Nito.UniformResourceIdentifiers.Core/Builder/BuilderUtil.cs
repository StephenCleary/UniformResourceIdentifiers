using System;
using System.Collections.Generic;
using Nito.UniformResourceIdentifiers.Helpers;

namespace Nito.UniformResourceIdentifiers.Builder
{
    /// <summary>
    /// Utility methods for builders.
    /// </summary>
    public static class BuilderUtil
    {
        /// <summary>
        /// Deconstructs a URI reference into this builder.
        /// </summary>
        /// <param name="builder">The builder to modify.</param>
        /// <param name="uri">The URI reference to deconstruct.</param>
        public static T ApplyUriReference<T>(T builder, IUniformResourceIdentifierReference uri)
            where T : ICommonBuilder<T>
        {
            return builder.WithUserInfo(uri.UserInfo).WithHost(uri.Host).WithPort(uri.Port).WithPrefixlessPathSegments(uri.PathSegments).WithQuery(uri.Query).WithFragment(uri.Fragment);
        }

        /// <summary>
        /// Parses and deconstructs a URI reference into this builder. Returns the scheme of the URI reference, if any.
        /// </summary>
        /// <param name="builder">The builder to modify.</param>
        /// <param name="uri">The URI reference to deconstruct.</param>
        public static string ApplyUriReference<T>(T builder, string uri)
            where T : ICommonBuilder<T>
        {
            Parser.ParseUriReference(uri, out string scheme, out string userInfo, out string host, out string port, out IReadOnlyList<string> pathSegements,
                out string query, out string fragment);
            builder.WithUserInfo(userInfo).WithHost(host).WithPort(port).WithPrefixlessPathSegments(pathSegements).WithQuery(query).WithFragment(fragment);
            return scheme;
        }

        /// <summary>
        /// Parses and deconstructs a URI reference into this builder, and verifies that the URI scheme matches what was expected.
        /// </summary>
        /// <param name="builder">The builder to modify.</param>
        /// <param name="uri">The URI reference to deconstruct.</param>
        /// <param name="expectedScheme">The expected URI scheme.</param>
        public static void ApplyUriReference<T>(T builder, string uri, string expectedScheme)
            where T : ICommonBuilder<T>
        {
            var scheme = ApplyUriReference(builder, uri);
            if (scheme != expectedScheme)
                throw new InvalidOperationException($"URI scheme \"{scheme}\" does not match expected scheme \"{expectedScheme ?? ""}\" in URI \"{uri}\".");
        }
    }
}