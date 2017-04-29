# Uniform Resource Identifiers

Because apparently this is hard, or something.

[![AppVeyor](https://img.shields.io/appveyor/ci/StephenCleary/UniformResourceIdentifiers.svg?style=plastic)](https://ci.appveyor.com/project/StephenCleary/UniformResourceIdentifiers) [![Coveralls](https://img.shields.io/coveralls/StephenCleary/UniformResourceIdentifiers.svg?style=plastic)](https://coveralls.io/r/StephenCleary/UniformResourceIdentifiers)
[![NuGet Pre Release](https://img.shields.io/nuget/vpre/Nito.UniformResourceIdentifiers.svg?style=plastic)](https://www.nuget.org/packages/Nito.UniformResourceIdentifiers/)

[API Docs](http://dotnetapis.com/pkg/Nito.UniformResourceIdentifiers)

# Standards

These libraries provide standard-compliant URI encoders and parsers.

The core library implements:

- [RFC3986](https://tools.ietf.org/html/rfc3986) for `GenericUniformResourceIdentifier` and common algorithms.
- [RFC6874](https://tools.ietf.org/html/rfc6874) and [RFC5952](https://tools.ietf.org/html/rfc5952) for IPv6 hosts.
- [HTML5 `application/x-www-form-urlencoded`](https://www.w3.org/TR/html5) for encoding query values.

Scheme-specific libraries implement:

- [RFC7230](https://tools.ietf.org/html/rfc7230) - HTTP/HTTPS
- [RFC4151](https://tools.ietf.org/html/rfc4151) - TAG

Possible future considerations: [RFC6570](https://tools.ietf.org/html/rfc6570).
