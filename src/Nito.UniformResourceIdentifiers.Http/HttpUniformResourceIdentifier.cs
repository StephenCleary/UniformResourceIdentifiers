using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.Comparers;
using Nito.Comparers.Util;
using Nito.UniformResourceIdentifiers.Implementation;
using Nito.UniformResourceIdentifiers.Implementation.Components;
using static Nito.UniformResourceIdentifiers.Implementation.Util;

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// An HTTP URI.
    /// </summary>
    public sealed class HttpUniformResourceIdentifier : ComparableBase<HttpUniformResourceIdentifier>, IUniformResourceIdentifier
    {
        private NormalizedHttpHost _host;
        private NormalizedHttpPort _port;
        private NormalizedHttpPathSegments _pathSegments;

        /// <summary>
        /// The HTTP scheme.
        /// </summary>
        public static string HttpScheme { get; } = "http";

        private static readonly DelegateFactory<HttpUniformResourceIdentifier> Factory = (userInfo, host, port, pathSegments, query, fragment) =>
            BuilderUtil.ApplyUriReference(new HttpUniformResourceIdentifierBuilder(), userInfo, host, port, pathSegments, query, fragment).Build();

        /// <summary>
        /// Registers this scheme with the factories.
        /// </summary>
        public static void Register() => Factories.RegisterSchemeFactory(HttpScheme, Factory);

        static HttpUniformResourceIdentifier()
        {
            DefaultComparer = UniformResourceIdentifier.Comparer;
        }

        /// <summary>
        /// Constructs a new URI instance.
        /// </summary>
        /// <param name="host">The host name portion of the authority, if any. This is converted to lowercase. This may be <c>null</c> to indicate no host name, or the empty string to indicate an empty host name.</param>
        /// <param name="port">The port portion of the authority, if any. This may be <c>null</c> to indicate no port, or the empty string to indicate an empty port. This must be <c>null</c>, the empty string, or a valid port as defined by <see cref="Util.IsValidPort"/>.</param>
        /// <param name="pathSegments">The path segments. Dot segments are normalized. May not be <c>null</c>, neither may any element be <c>null</c>.</param>
        /// <param name="query">The query. This may be <c>null</c> to indicate no query, or the empty string to indicate an empty query.</param>
        /// <param name="fragment">The fragment. This may be <c>null</c> to indicate no fragment, or the empty string to indicate an empty fragment.</param>
        public HttpUniformResourceIdentifier(string host, string port, IEnumerable<string> pathSegments, string query, string fragment)
        {
            _host = new NormalizedHttpHost(host);
            _port = new NormalizedHttpPort(port);
            _pathSegments = new NormalizedHttpPathSegments(pathSegments, null, _host.Value, _port.Value);
            Query = query;
            Fragment = fragment;
        }

        string IUniformResourceIdentifier.Scheme => HttpScheme;

        string IUniformResourceIdentifierReference.UserInfo => null;

        /// <inheritdoc />
        public string Host => _host.Value;

        /// <inheritdoc />
        public string Port => _port.Value;

        /// <inheritdoc />
        public IReadOnlyList<string> PathSegments => _pathSegments.Value;

        /// <inheritdoc />
        public string Query { get; }

        /// <inheritdoc />
        public string Fragment { get; }

        /// <inheritdoc />
        public bool Equals(IUniformResourceIdentifier other) => ComparableImplementations.ImplementEquals(DefaultComparer, this, other);

        /// <inheritdoc />
        public int CompareTo(IUniformResourceIdentifier other) => ComparableImplementations.ImplementCompareTo(DefaultComparer, this, other);

        /// <inheritdoc />
        public override string ToString() => this.UriString();

        /// <summary>
        /// Resolves a relative URI against this URI.
        /// </summary>
        /// <param name="relativeUri">The relative URI to resolve.</param>
        public HttpUniformResourceIdentifier Resolve(RelativeReference relativeUri) => Util.Resolve(this, relativeUri, Factory);

        IUniformResourceIdentifier IUniformResourceIdentifier.Resolve(RelativeReference relativeUri) => Resolve(relativeUri);

        /// <summary>
        /// Resolves a reference URI against this URI.
        /// </summary>
        /// <param name="referenceUri">The reference URI to resolve.</param>
        public IUniformResourceIdentifier Resolve(IUniformResourceIdentifierReference referenceUri) => Util.Resolve(this, referenceUri, Factory);

        /// <summary>
        /// Parses an HTTP URI.
        /// </summary>
        /// <param name="uri">The HTTP URI to parse.</param>
        public static HttpUniformResourceIdentifier Parse(string uri) => new HttpUniformResourceIdentifierBuilder(uri).Build();
    }
}
