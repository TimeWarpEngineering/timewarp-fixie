[![Dotnet](https://img.shields.io/badge/dotnet-6.0-blue)](https://dotnet.microsoft.com)
[![Stars](https://img.shields.io/github/stars/TimeWarpEngineering/timewarp-fixie?logo=github)](https://github.com/TimeWarpEngineering/timewarp-fixie)
[![Discord](https://img.shields.io/discord/715274085940199487?logo=discord)](https://discord.gg/7F4bS2T)
[![nuget](https://img.shields.io/nuget/v/TimeWarp.Fixie?logo=nuget)](https://www.nuget.org/packages/TimeWarp.Fixie/)
[![nuget](https://img.shields.io/nuget/dt/TimeWarp.Fixie?logo=nuget)](https://www.nuget.org/packages/TimeWarp.Fixie/)
[![Issues Open](https://img.shields.io/github/issues/TimeWarpEngineering/timewarp-fixie?logo=github)](https://github.com/TimeWarpEngineering/timewarp-fixie/issues)
[![Forks](https://img.shields.io/github/forks/TimeWarpEngineering/timewarp-fixie)](https://github.com/TimeWarpEngineering/timewarp-fixie)
[![License](https://img.shields.io/github/license/TimeWarpEngineering/timewarp-fixie?logo=github)](https://unlicense.org)
[![Twitter](https://img.shields.io/twitter/url?style=social&url=https%3A%2F%2Fgithub.com%2FTimeWarpEngineering%2Ftimewarp-fixie)](https://twitter.com/intent/tweet?url=https://github.com/TimeWarpEngineering/timewarp-fixie)

[![Twitter](https://img.shields.io/twitter/follow/StevenTCramer.svg)](https://twitter.com/intent/follow?screen_name=StevenTCramer)
[![Twitter](https://img.shields.io/twitter/follow/TheFreezeTeam1.svg)](https://twitter.com/intent/follow?screen_name=TheFreezeTeam1)

# timewarp-fixie

![TimeWarp Logo](assets/Logo.png)

The TimeWarp testing convention for the excellent test framework [Fixie](https://github.com/fixie/fixie/wiki).

## Features

* No need to mark tests. Public methods are test cases by default.
* Skip - can mark tests to be skipped.
* Tags - Add tags to your tests and filter runs based on the tag.
* Inputs - Allow for parameterized tests. (similar to how "Theory" works in xUnit)
* Lifecycle Methods -  if the `Setup` or `Cleanup` methods are found on the test class they will be executed appropriately.
* NotTest - Can mark methods with `NotTest` attribute if they are not tests.

## Give a Star! :star:

If you like or are using this project please give it a star. Thank you!

## Installation

You can see the latest NuGet packages from the official [TimeWarp NuGet page](https://www.nuget.org/profiles/TimeWarp.Enterprises).

* [timewarp-fixie](https://www.nuget.org/packages/TimeWarp.Fixie/) [![nuget](https://img.shields.io/nuget/v/TimeWarp.Fixie?logo=nuget)](https://www.nuget.org/packages/TimeWarp.Fixie/)

## Usage

Create a new test project.

```console
dotnet new classlib -n MyProject.Tests
```

Add Nugets to the project 

```console
dotnet add package TimeWarp.Fixie
dotnet add package Fixie.TestAdapter --version 3.2.0
```

Create a dotnet tool manifest.

```console
dotnet new tool-manifest
```

Add Fixie.Console to the manifest

```console
dotnet tool install Fixie.Console
```

Inside your fixie project create a class that inherits from `Fixie.Conventions.TestConvention`

```csharp
class TestProject : TimeWarp.Fixie.TestingConvention { }
```
This will then use the TimeWarp.Fixie convention.

Create a sample test.
First we will add FluentAssertions you could use basic Asserts or any other assertion library.

```csharp
dotnet add package FluentAssertions --version 6.7.0
```

Create a sample test case.

```csharp
namespace ConventionTest_;

using FluentAssertions;
using TimeWarp.Fixie;

[TestTag(TestTags.Fast)]
public class SimpleNoApplicationTest_Should_
{
  public static void AlwaysPass() => true.Should().BeTrue();

  [Skip("Demonstrates skip attribute")]
  public static void SkipExample() => true.Should().BeFalse();

  [TestTag(TestTags.Fast)]
  public static void TagExample() => true.Should().BeTrue();

  [Input(5, 3, 2)]
  [Input(8, 5, 3)]
  public static void Subtract(int aX, int aY, int aExpectedDifference)
  {
    int result = aX - aY;
    result.Should().Be(aExpectedDifference);
  }
}
```

Execute the tests:

```console
dotnet fixie
```

### How to filter tests by Name

From fixie [docs](https://github.com/fixie/fixie/wiki/Command-Line-Arguments#filtering-with---tests) 

The optional argument `--tests` (abbreviated `-t`) lets you specify which tests to run.

A full test name match will run that single test:

```console
dotnet fixie MyTestProject --tests Full.Namespace.MyTestClass.MyTestMethod
```

To avoid having to type the full namespace or method name, there is an implicit wildcard at the start and end of the pattern. Here we run an entire test class:

```console
dotnet fixie MyTestProject --tests MyTestClass
```
There is an implicit lowercase letter wildcard, whenever a capital letter is followed by a non-lowercase character. In other words, you can type "MTC" to match "MyTestClass". Here we run a select few related tests within that class:

```console
dotnet fixie MyTestProject --tests MTC.ShouldValidateThat
```

Although unnecessary in most realistic cases, an explicit `*` wildcard will match any sequence of zero or more characters:

```console
dotnet fixie MyTestProject --tests MTC.*Validate
```

When all tests are run by omitting the `--tests` argument, passing tests are omitted for brevity. However, because a `--tests` pattern may in fact be more inclusive than the developer intended, the console output will include *passing* test names in addition to other test results as feedback whenever this argument is used.

### How to filter tests by Tags

If you want to only run tests with a given tag/s you can do this by passing in the `--Tag` parameter after `--`.
If you want to run more than one Tag pass the parameter multiple times.

Examples:


```console
dotnet fixie --no-build -- --Tag Fast --Tag Smoke
```


```console
dotnet fixie -- --Tag Smoke
```



## Unlicense

[![License](https://img.shields.io/github/license/TimeWarpEngineering/timewarp-fixie?logo=github)](https://unlicense.org)

## Contributing

Time is of the essence.  Before developing a Pull Request I recommend opening a [discussion](https://github.com/TimeWarpEngineering/timewarp-fixie/discussions).

Please feel free to make suggestions and help out with the [documentation](https://timewarpengineering.github.io/timewarp-fixie/).
Please refer to [Markdown](http://daringfireball.net/projects/markdown/) for how to write markdown files.

## Contact

Sometimes the github notifications get lost in the shuffle.  If you file an [issue](https://github.com/TimeWarpEngineering/timewarp-fixie/issues) and don't get a response in a timely manner feel free to ping on our [Discord server](https://discord.gg/A55JARGKKP).

[![Discord](https://img.shields.io/discord/715274085940199487?logo=discord)](https://discord.gg/7F4bS2T)

## References

https://github.com/fixie/fixie

### Commands used

```PowerShell
dotnet new sln
dotnet new classlib -n timewarp-fixie
dotnet new classlib -n TimeWarp.Fixie.Tests
dotnet sln add .\source\timewarp-fixie\timewarp-fixie.csproj
dotnet new tool-manifest
dotnet tool install dotnet-cleanup
dotnet tool install Fixie.Console
dotnet cleanup -y
```
