# Uniform Resource Identifiers

Because apparently this is hard, or something.

[![AppVeyor](https://img.shields.io/appveyor/ci/StephenCleary/UniformResourceIdentifiers.svg?style=plastic)](https://ci.appveyor.com/project/StephenCleary/UniformResourceIdentifiers) [![Coveralls](https://img.shields.io/coveralls/StephenCleary/UniformResourceIdentifiers.svg?style=plastic)](https://coveralls.io/r/StephenCleary/UniformResourceIdentifiers)
[![NuGet Pre Release](https://img.shields.io/nuget/vpre/Nito.UniformResourceIdentifiers.svg?style=plastic)](https://www.nuget.org/packages/Nito.UniformResourceIdentifiers/)

[API Docs](http://dotnetapis.com/pkg/Nito.UniformResourceIdentifiers)

# Standards

These libraries provide standard-compliant URI encoders and parsers.

General URI types implement [RFC3986](https://tools.ietf.org/html/rfc3986) as [augmented](https://datatracker.ietf.org/doc/rfc3986/) by [RFC6874](https://tools.ietf.org/html/rfc6874) and [RFC5952](https://tools.ietf.org/html/rfc5952).

HTTP/HTTPS URI types implement [RFC7230](https://tools.ietf.org/html/rfc7230) with `application/x-www-form-urlencoded` from [HTML5](https://www.w3.org/TR/html5).

Possible future considerations: [RFC6570](https://tools.ietf.org/html/rfc6570).
