using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Nito.UniformResourceIdentifiers.Helpers
{
    /// <summary>
    /// A builder that allows specifying user info.
    /// </summary>
    /// <typeparam name="T">The type of the builder.</typeparam>
    public interface IBuilderWithUserInfo<out T>
    {
        /// <summary>
        /// Applies the user information portion of the authority to this builder, overwriting any existing user information. Note that user information is deprecated for most schemes, since it is inherently insecure.
        /// </summary>
        /// <param name="userInfo">The user information. May be <c>null</c> or the empty string.</param>
        T WithUserInfo(string userInfo);
    }

    /// <summary>
    /// A builder that allows specifying a host.
    /// </summary>
    /// <typeparam name="T">The type of the builder.</typeparam>
    public interface IBuilderWithHost<out T>
    {
        /// <summary>
        /// Applies the host portion of the authority to this builder, overwriting any existing host.
        /// </summary>
        /// <param name="host">The host. May be <c>null</c> or the empty string.</param>
        T WithHost(string host);
    }

    /// <summary>
    /// A builder that allows specifying a port.
    /// </summary>
    /// <typeparam name="T">The type of the builder.</typeparam>
    public interface IBuilderWithPort<out T>
    {
        /// <summary>
        /// Applies the port portion of the authority to this builder, overwriting any existing port.
        /// </summary>
        /// <param name="port">The port. May be <c>null</c> or the empty string.</param>
        T WithPort(string port);
    }

    /// <summary>
    /// A builder that allows specifying path segments.
    /// </summary>
    /// <typeparam name="T">The type of the builder.</typeparam>
    public interface IBuilderWithPath<out T>
    {
        /// <summary>
        /// Applies the path to this builder, overwriting any existing path. This method does not automatically prefix a forward slash to the resulting path.
        /// </summary>
        /// <param name="pathSegments">The path segments.</param>
        T WithPrefixlessPathSegments(IEnumerable<string> pathSegments);
    }

    /// <summary>
    /// A builder that allows specifying a query string.
    /// </summary>
    /// <typeparam name="T">The type of the builder.</typeparam>
    public interface IBuilderWithQuery<out T>
    {
        /// <summary>
        /// Applies the query string to this builder, overwriting any existing query.
        /// </summary>
        /// <param name="query">The query.</param>
        T WithQuery(string query);
    }

    /// <summary>
    /// A builder that allows specifying a fragment string.
    /// </summary>
    /// <typeparam name="T">The type of the builder.</typeparam>
    public interface IBuilderWithFragment<out T>
    {
        /// <summary>
        /// Applies the fragment string to this builder, overwriting any existing fragment.
        /// </summary>
        /// <param name="fragment">The fragment.</param>
        T WithFragment(string fragment);
    }

    /// <summary>
    /// A builder with common options.
    /// </summary>
    /// <typeparam name="T">The type of the builder.</typeparam>
    public interface ICommonBuilder<out T> : IBuilderWithUserInfo<T>, IBuilderWithHost<T>, IBuilderWithPort<T>, IBuilderWithPath<T>, IBuilderWithQuery<T>, IBuilderWithFragment<T>
    {
    }

    /// <summary>
    /// Extension methods for builders.
    /// </summary>
    public static class BuilderExtensions
    {
        /// <summary>
        /// Applies the port portion of the authority to this builder, overwriting any existing port.
        /// </summary>
        /// <param name="this">The builder.</param>
        /// <param name="port">The port.</param>
        public static T WithPort<T>(this IBuilderWithPort<T> @this, uint port) => @this.WithPort(port.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Applies the path to this builder, overwriting any existing path. This method does not automatically prefix a forward slash to the resulting path.
        /// </summary>
        /// <param name="this">The builder.</param>
        /// <param name="segments">The path segments.</param>
        public static T WithPrefixlessPathSegments<T>(this IBuilderWithPath<T> @this, params string[] segments) =>
            @this.WithPrefixlessPathSegments(segments);

        /// <summary>
        /// Applies the path to this builder, overwriting any existing path. This method automatically prefixes a forward slash to the resulting path.
        /// </summary>
        /// <param name="this">The builder.</param>
        /// <param name="segments">The path segments.</param>
        public static T WithPathSegments<T>(this IBuilderWithPath<T> @this, IEnumerable<string> segments)
        {
            if (segments == null)
                throw new ArgumentNullException(nameof(segments));
            return @this.WithPrefixlessPathSegments(Enumerable.Repeat("", 1).Concat(segments));
        }

        /// <summary>
        /// Applies the path to this builder, overwriting any existing path. This method automatically prefixes a forward slash to the resulting path.
        /// </summary>
        /// <param name="this">The builder.</param>
        /// <param name="segments">The path segments.</param>
        public static T WithPathSegments<T>(this IBuilderWithPath<T> @this, params string[] segments) => @this.WithPathSegments((IEnumerable<string>)segments);
    }

    /// <summary>
    /// A builder base type for common <see cref="UniformResourceIdentifierReference"/>-derived types.
    /// </summary>
    public abstract class CommonUniformResourceIdentifierBuilderBase<T>: ICommonBuilder<T>
        where T : CommonUniformResourceIdentifierBuilderBase<T>
    {
        /// <summary>
        /// The user information portion of the authority. For most schemes, this is a deprecated component. May be <c>null</c> or the empty string.
        /// </summary>
        protected virtual string UserInfo { get; set; }

        /// <summary>
        /// The host portion of the authority. May be <c>null</c> or the empty string.
        /// </summary>
        protected virtual string Host { get; set; }

        /// <summary>
        /// The port portion of the authority. May be <c>null</c> or the empty string.
        /// </summary>
        protected virtual string Port { get; set; }

        /// <summary>
        /// The path segments. Never <c>null</c>, but may be empty. If you want an absolute path (i.e., starting with "/"), then this list should have an empty string as its first element.
        /// </summary>
        protected virtual List<string> PathSegments { get; } = new List<string>();

        /// <summary>
        /// The query. May be <c>null</c> or the empty string.
        /// </summary>
        protected virtual string Query { get; set; }

        /// <summary>
        /// The fragment. May be <c>null</c> or the empty string.
        /// </summary>
        protected virtual string Fragment { get; set; }

        /// <summary>
        /// Applies the user information portion of the authority to this builder, overwriting any existing user information. Note that user information is deprecated for most schemes, since it is inherently insecure.
        /// </summary>
        /// <param name="userInfo">The user information. May be <c>null</c> or the empty string.</param>
        public virtual T WithUserInfo(string userInfo)
        {
            UserInfo = userInfo;
            return (T)this;
        }

        /// <summary>
        /// Applies the host portion of the authority to this builder, overwriting any existing host.
        /// </summary>
        /// <param name="host">The host. May be <c>null</c> or the empty string.</param>
        public virtual T WithHost(string host)
        {
            Host = host;
            return (T)this;
        }

        /// <summary>
        /// Applies the port portion of the authority to this builder, overwriting any existing port.
        /// </summary>
        /// <param name="port">The port. May be <c>null</c> or the empty string.</param>
        public virtual T WithPort(string port)
        {
            if (port != null && !Util.IsValidPort(port))
                throw new ArgumentException("Invalid port " + port, nameof(port));
            Port = port;
            return (T)this;
        }

        /// <summary>
        /// Applies the port portion of the authority to this builder, overwriting any existing port.
        /// </summary>
        /// <param name="port">The port.</param>
        public virtual T WithPort(uint port) => WithPort(port.ToString(CultureInfo.InvariantCulture));

        // Valid path styles:
        //  http://example.com - "" - { "" }
        //  http://example.com/ - "/" - { "", "" }
        //  http://example.com/test - "/test" - { "", "test" }
        //  http://example.com/test/ - "/test/" - { "", "test", "" }
        //  http:/test - "/test" - { "", "test" }
        //  http:test - "test" - { "test" } - this is the special case!
        //  http: - "" - { "" }

        /// <summary>
        /// Applies the path to this builder, overwriting any existing path. This method does not automatically prefix a forward slash to the resulting path.
        /// </summary>
        /// <param name="segments">The path segments.</param>
        public virtual T WithPrefixlessPathSegments(IEnumerable<string> segments)
        {
            if (segments == null)
                throw new ArgumentNullException(nameof(segments));
            PathSegments.Clear();
            PathSegments.AddRange(segments);
            return (T)this;
        }

        /// <summary>
        /// Applies the path to this builder, overwriting any existing path. This method does not automatically prefix a forward slash to the resulting path.
        /// </summary>
        /// <param name="segments">The path segments.</param>
        public virtual T WithPrefixlessPathSegments(params string[] segments) => WithPrefixlessPathSegments((IEnumerable<string>)segments);

        /// <summary>
        /// Applies the path to this builder, overwriting any existing path. This method automatically prefixes a forward slash to the resulting path.
        /// </summary>
        /// <param name="segments">The path segments.</param>
        public virtual T WithPathSegments(IEnumerable<string> segments)
        {
            if (segments == null)
                throw new ArgumentNullException(nameof(segments));
            return WithPrefixlessPathSegments(Enumerable.Repeat("", 1).Concat(segments));
        }

        /// <summary>
        /// Applies the path to this builder, overwriting any existing path. This method automatically prefixes a forward slash to the resulting path.
        /// </summary>
        /// <param name="segments">The path segments.</param>
        public virtual T WithPathSegments(params string[] segments) => WithPathSegments((IEnumerable<string>)segments);

        /// <summary>
        /// Applies the query string to this builder, overwriting any existing query.
        /// </summary>
        /// <param name="query">The query.</param>
        public virtual T WithQuery(string query)
        {
            Query = query;
            return (T)this;
        }

        /// <summary>
        /// Applies the fragment string to this builder, overwriting any existing fragment.
        /// </summary>
        /// <param name="fragment">The fragment.</param>
        public virtual T WithFragment(string fragment)
        {
            Fragment = fragment;
            return (T)this;
        }

        /// <summary>
        /// Deconstructs a URI reference into this builder.
        /// </summary>
        /// <param name="uri">The URI reference to deconstruct.</param>
        protected virtual void ApplyUriReference(IUniformResourceIdentifierReference uri)
        {
            WithUserInfo(uri.UserInfo).WithHost(uri.Host).WithPort(uri.Port).WithPrefixlessPathSegments(uri.PathSegments).WithQuery(uri.Query).WithFragment(uri.Fragment);
        }

        /// <summary>
        /// Parses and deconstructs a URI reference into this builder. Returns the scheme of the URI reference, if any.
        /// </summary>
        /// <param name="uri">The URI reference to deconstruct.</param>
        protected virtual string ApplyUriReference(string uri)
        {
            string scheme, userInfo, host, port, query, fragment;
            IReadOnlyList<string> pathSegements;
            Parser.ParseUriReference(uri, out scheme, out userInfo, out host, out port, out pathSegements, out query, out fragment);
            WithUserInfo(userInfo).WithHost(host).WithPort(port).WithPrefixlessPathSegments(pathSegements).WithQuery(query).WithFragment(fragment);
            return scheme;
        }

        /// <summary>
        /// Parses and deconstructs a URI reference into this builder, and verifies that the URI scheme matches what was expected.
        /// </summary>
        /// <param name="uri">The URI reference to deconstruct.</param>
        /// <param name="expectedScheme">The expected URI scheme.</param>
        protected virtual void ApplyUriReference(string uri, string expectedScheme)
        {
            var scheme = ApplyUriReference(uri);
            if (scheme != expectedScheme)
                throw new InvalidOperationException($"URI scheme \"{scheme}\" does not match expected scheme \"{expectedScheme ?? ""}\" in URI \"{uri}\".");
        }
    }

    /// <summary>
    /// Utility methods for builders.
    /// </summary>
    public static class BuilderUtils
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
