using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nito.UniformResourceIdentifiers.Helpers;

namespace Nito.UniformResourceIdentifiers
{
    /// <summary>
    /// A URI builder base type for HTTP and HTTPS URIs.
    /// </summary>
    public abstract class HttpUniformResourceIdentifierBuilderBase : UniformResourceIdentifierBuilder
    {
        /// <summary>
        /// User information is no longer valid on HTTP or HTTPS URIs, as of RFC7230. This method throws <c>InvalidOperationException</c> if <paramref name="userInfo"/> is not <c>null</c>.
        /// </summary>
        /// <param name="userInfo">The user information. Must be <c>null</c>.</param>
        public override UniformResourceIdentifierBuilder WithUserInfo(string userInfo)
        {
            if (userInfo != null)
                throw new InvalidOperationException("HTTP/HTTPS URIs can no longer have UserInfo portions, as of RFC7230.");
            UserInfo = null;
            return this;
        }

        // TODO: Query kvp support.
        // TODO: Fragment path segment support? Other query/fragment support?
    }
}
