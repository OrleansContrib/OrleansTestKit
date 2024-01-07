# Orleans TestKit

[![GitHub build status](https://github.com/OrleansContrib/OrleansTestKit/workflows/Continuous%20Integration/badge.svg)](https://github.com/OrleansContrib/OrleansTestKit/actions) [![NuGet pre-release package version](https://img.shields.io/nuget/vpre/OrleansTestKit.svg?style=flat)](https://www.nuget.org/packages/OrleansTestKit/) [![NuGet stable package version](https://img.shields.io/nuget/v/OrleansTestKit.svg?style=flat)](https://www.nuget.org/packages/OrleansTestKit/) [![MIT license](https://img.shields.io/badge/license-MIT-yellow.svg)](https://github.com/OrleansContrib/OrleansTestKit/blob/main/LICENSE) [![Discord](https://img.shields.io/discord/333727978460676096?color=4db798&label=Discord%20Chat&logoColor=4db798)](https://aka.ms/orleans-discord)

- [Orleans TestKit](#orleans-testkit)
  - [About](#about)
  - [Getting Started](#getting-started)
  - [Contributing](#contributing)
    - [Visual Studio](#visual-studio)
    - [Visual Studio Code](#visual-studio-code)
  - [Community](#community)

## About

The Orleans TestKit is a community-maintained library providing [mock objects](https://wikipedia.org/wiki/Mock_object) that facilitate unit testing grains in applications built on the [Microsoft Orleans](https://learn.microsoft.com/dotnet/orleans/) framework. It provides a _simulated grain activation context_, leveraging [Moq](https://github.com/moq/moq4) to generate test doubles for dependencies such as persistent state, reminders, timers, and streams. By simulating a grain activation context, you focus on testing the behavior of a single grain in isolation.

The official [integration testing approach](https://learn.microsoft.com/dotnet/orleans/implementation/testing) leverages the `TestCluster`. The `TestCluster` is a fully functional, in-memory cluster. It is faster to start than a regular cluster and provides a complete runtime. However, it may require complex configuration and custom-developed dependencies to test particular scenarios. That having been said, there are important caveats to the Orleans TestKit approach.

The simulated grain activation context does not provide the single-threaded execution model of the Microsoft Orleans runtime. It is up to you to ensure the grain activation is used appropriately. Unfortunately, this may result in abnormal method execution or behaviors that are impossible to reproduce, especially in reentrant grains.

It is also important to note that mock-based testing presents risk of coupling your test cases to the internal implementation details of the grain. This may make your code difficult to refactor and your tests brittle (see Martin Fowler's article [Mocks Aren't Stubs](https://martinfowler.com/articles/mocksArentStubs.html)).

It is recommended that you consider developing a mixture of tests based on both the Orleans TestKit and the `TestCluster`.

## Getting Started

There are two branches and major versions of the Orleans TestKit. The [`main`](https://github.com/OrleansContrib/OrleansTestKit/tree/main) branch provides Orleans TestKit 4, a stable version supporting Orleans 7. The [`3.x`](https://github.com/OrleansContrib/OrleansTestKit/tree/3.x) branch provides Orleans TestKit 3, a stable version supporting Microsoft Orleans 3.

If you are using Microsoft Orleans 7, install the latest, stable [`OrleansTestKit`](https://www.nuget.org/packages/OrleansTestKit) NuGet package in your test project. For example, run the following command in your Visual Studio Package Manager Console:

```pwsh
Install-Package OrleansTestKit
```

If you are using Microsoft Orleans 3, install the latest, stable version less than 4.0 of the [`OrleansTestKit`](https://www.nuget.org/packages/OrleansTestKit) NuGet package in your test project. For example, run the following command in your Visual Studio Package Manager Console, replacing `3.x.x` with the latest version of the NuGet package less than 4.0:

```pwsh
Install-Package OrleansTestKit -Version 3.x.x
```

Refer to the [unit tests](https://github.com/OrleansContrib/OrleansTestKit/tree/main/test) project to learn how to create test fixtures using the Orleans TestKit.

## Contributing

Either Visual Studio or Visual Studio Code may be used for development. Visual Studio provides a richer experience, especially when it comes to debugging. Visual Studio Code providers a lightweight experience and still works with the majority of the tooling.

### Visual Studio

1. In Visual Studio, open the `OrleansTestKit.sln` solution.

1. Recommended: Install the [CodeMaid extension](https://marketplace.visualstudio.com/search?term=codemaid&target=VS&category=All%20categories&vsVersion=&sortBy=Relevance).

### Visual Studio Code

1. In Visual Studio Code, open the folder containing the `OrleansTestKit.sln` solution file.

1. Recommended: Open Visual Studio Code's extensions panel, and install all of the recommended extensions.

## Community

- Chat about all things Orleans on the official [Discord server](https://aka.ms/orleans-discord).
- Report bugs and ask questions about the Orleans TestKit by opening a new [GitHub Issue](https://github.com/OrleansContrib/OrleansTestKit/issues/new). Please be sure to note which version of the Orleans TestKit you are using.

## License

This project is released under the [MIT license](https://github.com/OrleansContrib/OrleansTestKit/blob/main/LICENSE).
