using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Nito.UniformResourceIdentifiers.Implementation.Util;

namespace Core.UnitTests
{
    public class UtilTests
    {
        [Theory]
        [InlineData(new[] { "", "a", "g" }, new[] { "", "a", "b", "c", ".", "..", "..", "g" })] // See 5.2.4
        [InlineData(new[] { "mid", "6" }, new[] { "mid", "content=5", "..", "6" })] // See 5.2.4
        public void RemoveDotSegmentsRfcExamples(IEnumerable<string> expected, IEnumerable<string> input)
        {
            var result = RemoveDotSegments(input);
            Assert.Equal(expected, result);
        }
    }
}
