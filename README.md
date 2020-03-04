# Orleans TestKit

[![GitHub build status](https://github.com/OrleansContrib/OrleansTestKit/workflows/Continuous%20Integration/badge.svg)](https://github.com/OrleansContrib/OrleansTestKit/actions) [![codecov test status](https://codecov.io/gh/OrleansContrib/OrleansTestKit/branch/master/graph/badge.svg)](https://codecov.io/gh/OrleansContrib/OrleansTestKit) [![NuGet package version](https://img.shields.io/nuget/v/OrleansTestKit.svg?style=flat)](http://www.nuget.org/packages/OrleansTestKit/) [![MIT license](https://img.shields.io/badge/license-MIT-yellow.svg)](https://github.com/OrleansContrib/OrleansTestKit/blob/master/LICENSE) [![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/dotnet/orleans?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

The Orleans TestKit is an easy-to-use toolkit for unit testing the grain logic of applications built with [Microsoft Orleans](http://dotnet.github.io/orleans/). The Orleans TestKit is intended to augment the [`TestCluster` unit testing approach](http://dotnet.github.io/orleans/Documentation/tutorials_and_samples/testing.html) provided by the official `Microsoft.Orleans.TestingHost` NuGet package.

**The Orleans TestKit is evolving to meet the demands of the team. If you have a question or need, please [ask us](https://github.com/OrleansContrib/OrleansTestKit/issues/new)!**

## Installation

Simply install the `OrleansTestKit` NuGet package in your project to get started. For example, run the following command in your Visual Studio Package Manager Console:

```
PM> Install-Package OrleansTestKit
```

## Documentation

Examples are provided by the included [test project](https://github.com/OrleansContrib/OrleansTestKit/tree/master/test).

### Known Limitations

When run within a test kit environment, code that calls the `GetPrimaryKey` extension methods sometimes result in an `ArgumentException` with the following message:

> Passing a half baked grain as an argument. It is possible that you instantiated a grain class explicitly, as a regular object and not via Orleans runtime or via proper test mocking.

See [issue #47](https://github.com/OrleansContrib/OrleansTestKit/issues/47) for a discussion and references to upstream issues.

Grains with States that do not have a parameterless constructor are not supported by the default IStorage implementation.

## Build Artifacts

The build artifacts for tagged commits are published to [NuGet](http://www.nuget.org/packages/OrleansTestKit/) and copied to [GitHub Releases](https://github.com/OrleansContrib/OrleansTestKit/releases).

The build artifacts for pull request commits and `master` branch commits are attached to the individual [Continuous Integration workflow logs](https://github.com/OrleansContrib/OrleansTestKit/actions).

## Community

- Report bugs and ask questions by opening a new [GitHub Issue](https://github.com/OrleansContrib/OrleansTestKit/issues/new)
- [Chat on Gitter](https://gitter.im/dotnet/orleans)

## License

This project is released under the [MIT license](https://github.com/OrleansContrib/OrleansTestKit/blob/master/LICENSE).
