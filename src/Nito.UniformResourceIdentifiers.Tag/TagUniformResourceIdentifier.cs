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

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// A TAG URI.
    /// </summary>
    public sealed class TagUniformResourceIdentifier : ComparableBase<TagUniformResourceIdentifier>, IUniformResourceIdentifierWithCustomComparison
    {
        private readonly NormalizedAuthorityName _authorityName;
        private readonly NormalizedDate _date;
        private readonly Lazy<UriParseResult> _uri;
        private readonly ComparerProxy _comparerProxy;

        /// <summary>
        /// The TAG scheme.
        /// </summary>
        public static string TagScheme { get; } = "tag";

        private static readonly Utility.DelegateFactory<TagUniformResourceIdentifier> Factory = (userInfo, host, port, pathSegments, query, fragment) =>
            Parse(Utility.ToString(TagScheme, userInfo, host, port, pathSegments, query, fragment));

        /// <summary>
        /// Registers this scheme with the factories.
        /// </summary>
        public static void Register() => Factories.RegisterSchemeFactory(TagScheme, Factory);

        static TagUniformResourceIdentifier()
        {
            DefaultComparer = UniformResourceIdentifier.Comparer;
        }

        /// <summary>
        /// Constructs a new URI instance.
        /// </summary>
        /// <param name="authorityName"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="specific">The specific identifier string. If this consists of path segments, then dot segments are normalized.</param>
        /// <param name="fragment">The fragment. This may be <c>null</c> to indicate no fragment, or the empty string to indicate an empty fragment.</param>
        public TagUniformResourceIdentifier(string authorityName, int year, int? month, int? day, string specific, string? fragment)
        {
            _ = specific ?? throw new ArgumentNullException(nameof(specific));

            // Technically, we do need to run remove_dot_segments here. Since `taggingEntity` cannot contain "/",
            //  and since remove_dot_segments will preserve the first segment on non-absolute paths, we can treat `specific` as though it were our path segments.
            specific = string.Join("/", Utility.RemoveDotSegments(specific.Split('/')));

            _authorityName = new NormalizedAuthorityName(authorityName);
            _date = new NormalizedDate(year, month, day);
            Specific = specific;
            Fragment = fragment;

            _uri = new Lazy<UriParseResult>(() => Parser.ParseUriReference(ToString()));
            _comparerProxy = new ComparerProxy(this);
        }

        string IUniformResourceIdentifier.Scheme => TagScheme;

        /// <summary>
        /// Gets the authority name of this URI, e.g., "example.com". This is never <c>null</c> or the empty string.
        /// </summary>
        public string AuthorityName => _authorityName.Value;

        /// <summary>
        /// Gets the year portion of the date of this URI, e.g., 2017.
        /// </summary>
        public int DateYear => _date.Year;

        /// <summary>
        /// Gets the month portion of the date of this URI, e.g., 4. May be <c>null</c> if the month portion is unspecified.
        /// </summary>
        public int? DateMonth => _date.Month;

        /// <summary>
        /// Gets the day portion of the date of this URI, e.g., 28. May be <c>null</c> if the day portion is unspecified.
        /// </summary>
        public int? DateDay => _date.Day;

        /// <summary>
        /// Gets the date of this URI, e.g., 2017-04-28.
        /// </summary>
        public DateTimeOffset Date => new DateTimeOffset(DateYear, DateMonth ?? 1, DateDay ?? 1, 0, 0, 0, TimeSpan.Zero);

        /// <summary>
        /// Gets the specific string of this URI, e.g., "my-identifier". May be the empty string.
        /// </summary>
        public string Specific { get; }

        /// <summary>
        /// Gets the fragment of this URI, e.g., "anchor-1". May be <c>null</c> or the empty string.
        /// </summary>
        public string? Fragment { get; }
        string? IUniformResourceIdentifierReference.UserInfo => _uri.Value.UserInfo;
        string? IUniformResourceIdentifierReference.Host => _uri.Value.Host;
        string? IUniformResourceIdentifierReference.Port => _uri.Value.Port;
        IReadOnlyList<string> IUniformResourceIdentifierReference.PathSegments => _uri.Value.PathSegments;
        string? IUniformResourceIdentifierReference.Query => _uri.Value.Query;
        string? IUniformResourceIdentifierReference.Fragment => _uri.Value.Fragment;

        /// <inheritdoc />
        bool IEquatable<IUniformResourceIdentifier>.Equals(IUniformResourceIdentifier other) => ComparableImplementations.ImplementEquals(DefaultComparer, this, other);

        /// <inheritdoc />
        int IComparable<IUniformResourceIdentifier>.CompareTo(IUniformResourceIdentifier other) => ComparableImplementations.ImplementCompareTo(DefaultComparer, this, other);

        /// <inheritdoc />
        public override string ToString() => TagUtil.ToString(AuthorityName, DateYear, DateMonth, DateDay, Specific, Fragment);

        IUniformResourceIdentifier IUniformResourceIdentifier.Resolve(RelativeReference relativeUri) => Utility.Resolve(this, relativeUri, Factory);

        IUniformResourceIdentifier IUniformResourceIdentifier.Resolve(IUniformResourceIdentifierReference referenceUri) => Utility.Resolve(this, referenceUri, Factory);

        /// <summary>
        /// Parses a URI.
        /// </summary>
        /// <param name="uri">The URI to parse.</param>
        public static TagUniformResourceIdentifier Parse(string uri) => new TagUniformResourceIdentifierBuilder(uri).Build();

        object IUniformResourceIdentifierWithCustomComparison.SchemeSpecificComparerProxy => _comparerProxy;

        private sealed class ComparerProxy : ComparableBase<ComparerProxy>
        {
            private readonly TagUniformResourceIdentifier _uri;

            static ComparerProxy()
            {
                DefaultComparer = ComparerBuilder.For<ComparerProxy>()
                    .OrderBy(x => x._uri.AuthorityName, StringComparer.Ordinal)
                    .ThenBy(x => x._uri.DateYear)
                    .ThenBy(x => x._uri.DateMonth)
                    .ThenBy(x => x._uri.DateDay)
                    .ThenBy(x => x._uri.Specific, StringComparer.Ordinal)
                    .ThenBy(x => x._uri.Fragment, StringComparer.Ordinal);
            }

            public ComparerProxy(TagUniformResourceIdentifier uri)
            {
                _uri = uri;
            }
        }
    }
}
