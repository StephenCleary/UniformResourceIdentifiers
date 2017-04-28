using System;

namespace Nito.UniformResourceIdentifiers.Implementation.Components
{
    /// <summary>
    /// Container for a normalized date.
    /// </summary>
    public struct NormalizedDate
    {
        /// <summary>
        /// Normalizes a date.
        /// </summary>
        public NormalizedDate(int year, int? month, int? day)
        {
            if (month == null && day != null)
                throw new ArgumentException("Month must be specified if day is specified.", nameof(month));
            if (year < 0 || year > 9999)
                throw new ArgumentOutOfRangeException(nameof(year), "Year must be in the range [0, 9999].");
            try
            {
                var _ = new DateTimeOffset(year, month ?? 1, day ?? 1, 0, 0, 0, TimeSpan.Zero);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new ArgumentOutOfRangeException($"Date is invalid: {year}-{month}-{day}", ex);
            }
            Year = year;
            Month = month;
            Day = day;
        }

        /// <summary>
        /// The normalized year.
        /// </summary>
        public int Year { get; }

        /// <summary>
        /// The normalized month.
        /// </summary>
        public int? Month { get; }

        /// <summary>
        /// The normalized day.
        /// </summary>
        public int? Day { get; }
    }
}