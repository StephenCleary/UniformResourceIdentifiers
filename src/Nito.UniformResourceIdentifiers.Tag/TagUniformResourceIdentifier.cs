using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.Comparers;
using Nito.Comparers.Util;
using Nito.UniformResourceIdentifiers.Implementation;

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// A TAG URI.
    /// </summary>
    public sealed class TagUniformResourceIdentifier : ComparableBase<TagUniformResourceIdentifier>, IUniformResourceIdentifierWithCustomComparison
    {
        private readonly Lazy<GenericUniformResourceIdentifier> _uri;
        private readonly ComparerProxy _comparerProxy;

        /// <summary>
        /// The TAG scheme.
        /// </summary>
        public static string TagScheme { get; } = "tag";

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
        public TagUniformResourceIdentifier(string authorityName, int? year, int? month, int? day, string specific, string fragment)
        {
            if (year == null)
                throw new InvalidOperationException(); // todo: message

            // TODO: dot-normalization? Yes, according to URI spec.
            // When parsing strings, may be best to parse into GURI, then take normalized string and parse *that*.
            AuthorityName = authorityName;
            DateYear = year.Value;
            DateMonth = month;
            DateDay = day;
            Specific = specific;
            Fragment = fragment;

            _uri = new Lazy<GenericUniformResourceIdentifier>(() => new GenericUniformResourceIdentifierBuilder(UriString()).Build());
            _comparerProxy = new ComparerProxy(this);
        }

        public string Scheme => TagScheme;
        public string AuthorityName { get; }
        public int DateYear { get; }
        public int? DateMonth { get; }
        public int? DateDay { get; }
        public DateTimeOffset Date => new DateTimeOffset(DateYear, DateMonth ?? 1, DateDay ?? 1, 0, 0, 0, TimeSpan.Zero);
        public string Specific { get; }
        public string Fragment { get; }
        string IUniformResourceIdentifierReference.UserInfo => _uri.Value.UserInfo;
        string IUniformResourceIdentifierReference.Host => _uri.Value.Host;
        string IUniformResourceIdentifierReference.Port => _uri.Value.Port;
        IReadOnlyList<string> IUniformResourceIdentifierReference.PathSegments => _uri.Value.PathSegments;
        string IUniformResourceIdentifierReference.Query => _uri.Value.Query;
        string IUniformResourceIdentifierReference.Fragment => _uri.Value.Fragment;

        public bool Equals(IUniformResourceIdentifier other) => ComparableImplementations.ImplementEquals(DefaultComparer, this, other);

        public int CompareTo(IUniformResourceIdentifier other) => ComparableImplementations.ImplementCompareTo(DefaultComparer, this, other);

        public override string ToString() => UriString();

        public string UriString() => TagUtil.ToString(AuthorityName, DateYear, DateMonth, DateDay, Specific, Fragment);

        IUniformResourceIdentifier IUniformResourceIdentifier.Resolve(RelativeReference relativeUri) => throw new NotImplementedException();

        IUniformResourceIdentifier IUniformResourceIdentifier.Resolve(IUniformResourceIdentifierReference referenceUri) => throw new NotImplementedException();

        public static TagUniformResourceIdentifier Parse(string uri) => throw new NotImplementedException();

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
