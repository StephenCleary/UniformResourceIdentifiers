![Logo](src/icon.png)

# Uniform Resource Identifiers [![Build status](https://github.com/StephenCleary/UniformResourceIdentifiers/workflows/Build/badge.svg)](https://github.com/StephenCleary/UniformResourceIdentifiers/actions?query=workflow%3ABuild) [![codecov](https://codecov.io/gh/StephenCleary/UniformResourceIdentifiers/branch/master/graph/badge.svg)](https://codecov.io/gh/StephenCleary/UniformResourceIdentifiers) [![NuGet version](https://badge.fury.io/nu/Nito.UniformResourceIdentifiers.svg)](https://www.nuget.org/packages/Nito.UniformResourceIdentifiers) [![API docs](https://img.shields.io/badge/API-dotnetapis-blue.svg)](http://dotnetapis.com/pkg/Nito.UniformResourceIdentifiers)

Because apparently this is hard, or something.

# Standards

These libraries provide standard-compliant URI encoders and parsers.

The core library implements:

- [RFC3986](https://tools.ietf.org/html/rfc3986) for `GenericUniformResourceIdentifier` and common algorithms.
- [RFC6874](https://tools.ietf.org/html/rfc6874) and [RFC5952](https://tools.ietf.org/html/rfc5952) for IPv6 hosts.
- [HTML5 `application/x-www-form-urlencoded`](https://www.w3.org/TR/html5) for encoding query values.

Scheme-specific libraries implement:

- [RFC7230](https://tools.ietf.org/html/rfc7230) - `http:` and `https:`
- [RFC4151](https://tools.ietf.org/html/rfc4151) - `tag:`

Possible future considerations: [RFC6570](https://tools.ietf.org/html/rfc6570).
