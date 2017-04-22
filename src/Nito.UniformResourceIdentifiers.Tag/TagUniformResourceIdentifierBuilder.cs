using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nito.UniformResourceIdentifiers.Helpers;

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// A URI builder for TAG URIs.
    /// </summary>
    public sealed class TagUniformResourceIdentifierBuilder
    {
        private string AuthorityName { get; set; }
        
        private int? Year { get; set; }
        private int? Month { get; set; }
        private int? Day { get; set; }

        private string Specific { get; set; }

        /// <summary>
        /// The fragment. May be <c>null</c> or the empty string.
        /// </summary>
        private string Fragment { get; set; }

        /// <summary>
        /// Constructs an empty builder.
        /// </summary>
        public TagUniformResourceIdentifierBuilder() { }

        /// <summary>
        /// Constructs a builder from an existing TAG URI.
        /// </summary>
        /// <param name="uri">The URI used to set the builder's initial values.</param>
        public TagUniformResourceIdentifierBuilder(TagUniformResourceIdentifier uri)
        {
            WithAuthorityName(uri)
            ApplyUriReference(uri);
        }

        /// <summary>
        /// Parses a URI string and constructs a builder.
        /// </summary>
        /// <param name="uri">The URI used to set the builder's initial values.</param>
        public TagUniformResourceIdentifierBuilder(string uri)
        {
            ApplyUriReference(uri, TagUniformResourceIdentifier.TagScheme);
        }

        /// <summary>
        /// Builds the HTTPS URI instance.
        /// </summary>
        public TagUniformResourceIdentifierBuilder Build() => new TagUniformResourceIdentifier(AuthorityName, Year, Month, Day, Specific, Fragment);

        /// <summary>
        /// Applies the authority name to this builder, overwriting any existing authority name. The authority name must be a dns hostname or an email address.
        /// </summary>
        /// <param name="authorityName">The authority name. If not <c>null</c>, then this must be a valid authority name.</param>
        public TagUniformResourceIdentifierBuilder WithAuthorityName(string authorityName)
        {
            if (authorityName != null && !TagUtil.IsValidAuthorityName(authorityName))
                throw new ArgumentException("Invalid authority name " + authorityName, nameof(authorityName));
            AuthorityName = authorityName;
            return this;
        }

        public TagUniformResourceIdentifierBuilder WithDateYear(int? year)
        {
            if (year != null && year < 0 || year > 9999)
                throw new ArgumentException("Invalid year " + year, nameof(year));
            Year = year;
            return this;
        }

        public TagUniformResourceIdentifierBuilder WithDateMonth(int? month)
        {
            if (month != null && month < 0 || month > 99)
                throw new ArgumentException("Invalid month " + month, nameof(month));
            Month = month;
            return this;
        }

        public TagUniformResourceIdentifierBuilder WithDateDay(int? day)
        {
            if (day != null && day < 0 || day > 99)
                throw new ArgumentException("Invalid day " + day, nameof(day));
            Day = day;
            return this;
        }

        public TagUniformResourceIdentifierBuilder WithDate(string date)
        {
            if (date == null)
            {
                Year = Month = Day = null;
                return this;
            }

            if (!TagUtil.TryParseDate(date, out var year, out var month, out var day))
                throw new ArgumentException("Invalid date " + date, nameof(date));
            return WithDateYear(year).WithDateMonth(month).WithDateDay(day);
        }

        public TagUniformResourceIdentifierBuilder WithDate(DateTimeOffset? date)
        {
            if (date == null)
            {
                Year = Month = Day = null;
                return this;
            }

            var utcDate = date.Value.ToUniversalTime();
            return WithDateYear(utcDate.Year).WithDateMonth(utcDate.Month).WithDateDay(utcDate.Day);
        }

        public TagUniformResourceIdentifierBuilder WithSpecific(string specific)
        {
            Specific = specific;
            return this;
        }

        /// <summary>
        /// Applies the fragment string to this builder, overwriting any existing fragment.
        /// </summary>
        /// <param name="fragment">The fragment.</param>
        public TagUniformResourceIdentifierBuilder WithFragment(string fragment)
        {
            Fragment = fragment;
            return this;
        }

        /// <summary>
        /// Deconstructs a URI reference into this builder.
        /// </summary>
        /// <param name="uri">The URI reference to deconstruct.</param>
        protected virtual void ApplyUriReference(UniformResourceIdentifierReference uri)
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
}
