using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nito.UniformResourceIdentifiers.Implementation;
using Nito.UniformResourceIdentifiers.Implementation.Builder;

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// A URI builder for TAG URIs.
    /// </summary>
    public sealed class TagUniformResourceIdentifierBuilder: IBuilderWithFragment<TagUniformResourceIdentifierBuilder>
    {
        private string _specific = "";
        private string? _authorityName, _fragment;
        private int? _year, _month, _day;
        
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
            _ = uri ?? throw new ArgumentNullException(nameof(uri));
            WithAuthorityName(uri.AuthorityName).WithDateYear(uri.DateYear).WithDateMonth(uri.DateMonth).WithDateDay(uri.DateDay).WithSpecific(uri.Specific).WithFragment(uri.Fragment);
        }

        /// <summary>
        /// Parses a URI string and constructs a builder.
        /// </summary>
        /// <param name="uri">The URI used to set the builder's initial values.</param>
        public TagUniformResourceIdentifierBuilder(string uri)
        {
            TagParser.Parse(uri, out var authorityName, out var year, out var month, out var day, out var specific, out var fragment);
            WithAuthorityName(authorityName).WithDateYear(year).WithDateMonth(month).WithDateDay(day).WithSpecific(specific).WithFragment(fragment);
        }

        /// <summary>
        /// Applies the authority name to this builder, overwriting any existing authority name. The authority name must be a dns hostname or an email address.
        /// </summary>
        /// <param name="authorityName">The authority name. If not <c>null</c>, then this must be a valid authority name.</param>
        public TagUniformResourceIdentifierBuilder WithAuthorityName(string? authorityName)
        {
            _authorityName = authorityName;
            return this;
        }

        /// <summary>
        /// Applies the date year to this builder, overwriting any existing date year.
        /// </summary>
        /// <param name="year">The date year. If not <c>null</c>, then this must be a four-digit year.</param>
        public TagUniformResourceIdentifierBuilder WithDateYear(int? year)
        {
            if (year != null && year < 0 || year > 9999)
                throw new ArgumentException("Invalid year " + year, nameof(year));
            _year = year;
            return this;
        }

        /// <summary>
        /// Applies the date month to this builder, overwriting any existing date month.
        /// </summary>
        /// <param name="month">The date month. If not <c>null</c>, then this must be a two-digit month.</param>
        public TagUniformResourceIdentifierBuilder WithDateMonth(int? month)
        {
            if (month != null && month < 0 || month > 99)
                throw new ArgumentException("Invalid month " + month, nameof(month));
            _month = month;
            return this;
        }

        /// <summary>
        /// Applies the date day to this builder, overwriting any existing date day.
        /// </summary>
        /// <param name="day">The date day. If not <c>null</c>, then this must be a two-digit day.</param>
        public TagUniformResourceIdentifierBuilder WithDateDay(int? day)
        {
            if (day != null && day < 0 || day > 99)
                throw new ArgumentException("Invalid day " + day, nameof(day));
            _day = day;
            return this;
        }

        /// <summary>
        /// Applies the date to this builder, overwriting any existing date (year/month/day).
        /// </summary>
        /// <param name="date">The date. If not <c>null</c>, then this must be a string of the form YYYY, YYYY-MM, or YYYY-MM-DD.</param>
        public TagUniformResourceIdentifierBuilder WithDate(string? date)
        {
            if (date == null)
            {
                _year = _month = _day = null;
                return this;
            }

            if (!TagUtil.TryParseDate(date, 0, date.Length, out var year, out var month, out var day))
                throw new ArgumentException("Invalid date " + date, nameof(date));
            return WithDateYear(year).WithDateMonth(month).WithDateDay(day);
        }

        /// <summary>
        /// Applies the date to this builder, overwriting any existing date (year/month/day).
        /// </summary>
        /// <param name="date">The date.</param>
        public TagUniformResourceIdentifierBuilder WithDate(DateTimeOffset? date)
        {
            if (date == null)
            {
                _year = _month = _day = null;
                return this;
            }

            var utcDate = date.Value.ToUniversalTime();
            return WithDateYear(utcDate.Year).WithDateMonth(utcDate.Month).WithDateDay(utcDate.Day);
        }

        /// <summary>
        /// Applies the specific string to this builder, overwriting any existing specific string.
        /// </summary>
        /// <param name="specific">The specific string.</param>
        public TagUniformResourceIdentifierBuilder WithSpecific(string specific)
        {
            _specific = specific ?? throw new ArgumentNullException(nameof(specific));
            return this;
        }

        /// <summary>
        /// Applies the fragment to this builder, overwriting any existing fragment.
        /// </summary>
        /// <param name="fragment">The fragment.</param>
        public TagUniformResourceIdentifierBuilder WithFragment(string? fragment)
        {
            _fragment = fragment;
            return this;
        }

        /// <summary>
        /// Builds the TAG URI instance.
        /// </summary>
        public TagUniformResourceIdentifier Build()
        {
            if (_year == null)
                throw new ArgumentException("Year is required.");
            if (string.IsNullOrEmpty(_authorityName))
                throw new ArgumentException("AuthorityName is required.");
            return new TagUniformResourceIdentifier(_authorityName!, _year.Value, _month, _day, _specific, _fragment);
        }
    }
}
