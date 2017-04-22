using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.UniformResourceIdentifiers.Helpers;
using static Nito.UniformResourceIdentifiers.Helpers.Util;

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// A TAG URI.
    /// </summary>
    public sealed class TagUniformResourceIdentifier : UniformResourceIdentifier
    {
        /// <summary>
        /// The TAG scheme.
        /// </summary>
        public static string TagScheme { get; } = "tag";

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
            : base(TagScheme, null, null, null, BuildPath(authorityName, year, month, day, specific), null, fragment)
        {
        }

        private static IEnumerable<string> BuildPath(string authorityName, int? year, int? month, int? day, string specific)
        {
            
        }

#if NO
        private static readonly DelegateFactory<HttpsUniformResourceIdentifier> Factory = CreateFactory(() => new HttpsUniformResourceIdentifierBuilder(), x => x.Build());

        /// <summary>
        /// Registers this scheme with the factories.
        /// </summary>
        public static void Register()
        {
            Factories.RegisterSchemeFactory(HttpsScheme, Factory);
        }

        /// <summary>
        /// Resolves a relative URI against this URI.
        /// </summary>
        /// <param name="relativeUri">The relative URI to resolve.</param>
        public new HttpsUniformResourceIdentifier Resolve(RelativeReference relativeUri) => Util.Resolve(this, relativeUri, Factory);

        /// <summary>
        /// Resolves a reference URI against this URI.
        /// </summary>
        /// <param name="referenceUri">The reference URI to resolve.</param>
        public override UniformResourceIdentifier Resolve(UniformResourceIdentifierReference referenceUri) => Util.Resolve(this, referenceUri, Factory);

        /// <summary>
        /// Parses an HTTPS URI.
        /// </summary>
        /// <param name="uri">The HTTPS URI to parse.</param>
        public new static HttpsUniformResourceIdentifier Parse(string uri)
        {
            return new HttpsUniformResourceIdentifierBuilder(uri).Build();
        }
#endif
    }
}
