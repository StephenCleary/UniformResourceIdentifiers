using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Nito.UniformResourceIdentifiers;
using Xunit;

namespace Core.UnitTests
{
    public class UniformResourceIdentifierReferenceTests
    {
        [Theory]
        [InlineData("foo://example.com/", typeof(GenericUniformResourceIdentifier))]
        [InlineData("example.com/", typeof(RelativeReference))]
        [InlineData("/example.com/", typeof(RelativeReference))]
        [InlineData("g:h", typeof(GenericUniformResourceIdentifier))]
        [InlineData("./g:h", typeof(RelativeReference))]
        public void Parse_ResultsInCorrectType(string uri, Type expectedType)
        {
            var result = UniformResourceIdentifierReference.Parse(uri);
            Assert.IsType(expectedType, result);
        }
    }
}
