# Orleans TestKit

[![Build status](https://ci.appveyor.com/api/projects/status/k4crsho9d5vlbcgg/branch/master?svg=true)](https://ci.appveyor.com/project/dsarfati/orleanstestkit/branch/master)
[![NuGet](https://img.shields.io/nuget/v/OrleansTestKit.svg?style=flat)](http://www.nuget.org/packages/OrleansTestKit/)
[![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/dotnet/orleans?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

Orleans TestKit is an easy to use unit testing tool for application code built using [Microsoft Orleans](http://dotnet.github.io/orleans/).
The Orleans TestKit is intended to augment the [existing testing framework](http://dotnet.github.io/orleans/Tutorials/Unit-Testing-Grains.html?q=test) already included with
Orleans with simple tests for grain logic only.

**The TestKit is constantly evolving to meet the demands of the team, if there is something you need please [ask us](https://github.com/OrleansContrib/OrleansTestKit/issues)
or create a new fork.**

Installation
=====

Installation is performed via [NuGet](https://www.nuget.org/packages/OrleansTestKit/). 

There is currently only one package, additional test framework (xUnit, NUnit...) specific ones may be added in the future.

In your test project:
```
PM> Install-Package Install OrleansTestKit
```

### Official Builds

The stable production-quality release is located on nuget

The latest clean build from Appveyor is located: [download](https://ci.appveyor.com/project/dsarfati/orleanstestkit/branch/master/artifacts) or [private nuget](https://ci.appveyor.com/nuget/orleanstestkit)

Documentation
=============

Samples are available in the [tests](https://github.com/OrleansContrib/OrleansTestKit/tree/master/test) 


Community
=====

* Report bugs or ask questions by opening a new [GitHub Issue](https://github.com/OrleansContrib/OrleansTestKit/issues)
* [Chat on Gitter](https://gitter.im/dotnet/orleans)

License
=====
This project is licensed under the [MIT license](https://github.com/dotnet/orleans/blob/master/LICENSE).
