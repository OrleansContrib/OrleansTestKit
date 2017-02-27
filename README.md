<p align="center">
  <img src="https://github.com/dotnet/orleans/blob/gh-pages/assets/logo_full.png" alt="Orleans logo" width="600px"> 
</p>
=======

[![Build status](https://ci.appveyor.com/api/projects/status/k4crsho9d5vlbcgg/branch/master?svg=true)](https://ci.appveyor.com/project/dsarfati/orleanstestkit/branch/master)
[![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/dotnet/orleans?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)
*TODO:Update nuget*

Orleans TestKit is an easy to use unit testing tool for application code built using [Microsoft Orleans](http://dotnet.github.io/orleans/).
The Orleans TestKit is intended to augment the [existing testing framework](http://dotnet.github.io/orleans/Tutorials/Unit-Testing-Grains.html?q=test) already included with
Orleans with simple tests for grain logic only.

**The TestKit is constantly evolving to meet the demands of the team, if there is something you need please [ask us](https://github.com/OrleansContrib/OrleansTestKit/issues)
or create a new fork.**

Installation
=====
*TODO:Update nuget*
Installation is performed via [NuGet](https://www.nuget.org/packages?q=orleans). 

There is currently only one package, additional test framework (xUnit, NUnit...) specific ones may be added in the future.

In your test project:
```
PM> Install-Package TODO.Blah.Blah.Blah 
```

### Official Builds

The stable production-quality release is located [TODO: here](https://github.com/dotnet/orleans/releases/latest).

The latest clean build from Appveyor is located: [here](https://ci.appveyor.com/project/dsarfati/orleanstestkit/branch/master/artifacts)

Documentation
=============

Documentation is located [TODO:here](http://dotnet.github.io/orleans/)

Code Examples
=============

Create an interface for your grain:
```c#
public interface IHello : Orleans.IGrainWithIntegerKey
{
  Task<string> SayHello(string greeting);
}
```

Provide an implementation of that interface:
```c#
public class HelloGrain : Orleans.Grain, IHello
{
  Task<string> SayHello(string greeting)
  {
    return Task.FromResult($"You said: '{greeting}', I say: Hello!");
  }
}
```

Call the grain from your Web service (or anywhere else):
```c#
// Get a reference to the IHello grain with id '0'.
var friend = GrainClient.GrainFactory.GetGrain<IHello>(0);

// Send a greeting to the grain and await the response.
Console.WriteLine(await friend.SayHello("Good morning, my friend!"));
```

Community
=====

* Report bugs or ask questions by opening a new [GitHub Issue](https://github.com/OrleansContrib/OrleansTestKit/issues)
* [Chat on Gitter](https://gitter.im/dotnet/orleans)

License
=====
This project is licensed under the [MIT license](https://github.com/dotnet/orleans/blob/master/LICENSE).
