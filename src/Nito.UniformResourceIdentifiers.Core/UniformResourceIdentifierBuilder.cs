using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static Nito.UniformResourceIdentifiers.Helpers.Util;
// ReSharper disable VirtualMemberNeverOverridden.Global

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// A builder base type for <see cref="UniformResourceIdentifierReference"/>-derived types.
    /// </summary>
    public abstract class UniformResourceIdentifierBuilder
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
        public virtual UniformResourceIdentifierBuilder WithUserInfo(string userInfo)
        {
            UserInfo = userInfo;
            return this;
        }

        /// <summary>
        /// Applies the host portion of the authority to this builder, overwriting any existing host.
        /// </summary>
        /// <param name="host">The host. May be <c>null</c> or the empty string.</param>
        public virtual UniformResourceIdentifierBuilder WithHost(string host)
        {
            Host = host;
            return this;
        }

        /// <summary>
        /// Applies the port portion of the authority to this builder, overwriting any existing port.
        /// </summary>
        /// <param name="port">The port. May be <c>null</c> or the empty string.</param>
        public virtual UniformResourceIdentifierBuilder WithPort(string port)
        {
            if (port != null && !IsValidPort(port))
                throw new ArgumentException("Invalid port " + port, nameof(port));
            Port = port;
            return this;
        }

        /// <summary>
        /// Applies the port portion of the authority to this builder, overwriting any existing port.
        /// </summary>
        /// <param name="port">The port.</param>
        public virtual UniformResourceIdentifierBuilder WithPort(uint port) => WithPort(port.ToString(CultureInfo.InvariantCulture));

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
        public virtual UniformResourceIdentifierBuilder WithPrefixlessPathSegments(IEnumerable<string> segments)
        {
            if (segments == null)
                throw new ArgumentNullException(nameof(segments));
            PathSegments.Clear();
            PathSegments.AddRange(segments);
            return this;
        }

        /// <summary>
        /// Applies the path to this builder, overwriting any existing path. This method does not automatically prefix a forward slash to the resulting path.
        /// </summary>
        /// <param name="segments">The path segments.</param>
        public virtual UniformResourceIdentifierBuilder WithPrefixlessPathSegments(params string[] segments) => WithPrefixlessPathSegments((IEnumerable<string>)segments);

        /// <summary>
        /// Applies the path to this builder, overwriting any existing path. This method automatically prefixes a forward slash to the resulting path.
        /// </summary>
        /// <param name="segments">The path segments.</param>
        public virtual UniformResourceIdentifierBuilder WithPathSegments(IEnumerable<string> segments)
        {
            if (segments == null)
                throw new ArgumentNullException(nameof(segments));
            return WithPrefixlessPathSegments(Enumerable.Repeat("", 1).Concat(segments));
        }

        /// <summary>
        /// Applies the path to this builder, overwriting any existing path. This method automatically prefixes a forward slash to the resulting path.
        /// </summary>
        /// <param name="segments">The path segments.</param>
        public virtual UniformResourceIdentifierBuilder WithPathSegments(params string[] segments) => WithPathSegments((IEnumerable<string>)segments);

        /// <summary>
        /// Applies the query portion of the authority to this builder, overwriting any existing query.
        /// </summary>
        /// <param name="query">The query.</param>
        public virtual UniformResourceIdentifierBuilder WithQuery(string query)
        {
            Query = query;
            return this;
        }

        /// <summary>
        /// Applies the fragment portion of the authority to this builder, overwriting any existing fragment.
        /// </summary>
        /// <param name="fragment">The fragment.</param>
        public virtual UniformResourceIdentifierBuilder WithFragment(string fragment)
        {
            Fragment = fragment;
            return this;
        }
    }
}
