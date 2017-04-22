using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.UniformResourceIdentifiers;

namespace Core.UnitTests
{
    public sealed class TestUniformResourceIdentifier : UniformResourceIdentifier
    {
        public TestUniformResourceIdentifier(IEnumerable<string> pathSegments, string scheme = "scheme", string userInfo = "user", string host = "host", string port = "8080", string query = "query", string fragment = "fragment")
            : base(scheme, userInfo, host, port, pathSegments, query, fragment)
        {
        }
    }
}
