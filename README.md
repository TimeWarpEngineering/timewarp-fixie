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

[Fixie](https://github.com/fixie/fixie/wiki) is a dotnet test framework similar to NUnit and xUnit, but with an emphasis on low-ceremony defaults and flexible customizations.

TimeWarp-fixie is a project that uses conventions to simplify using Fixie even further.

## Feature overview

* Dependency Injection support for test cases.
* No need to decorate test methods with [Test] attributes. Public methods are test cases by default.
* Skip - can mark tests to be skipped.
* Tags - Add tags to your tests and filter runs based on the tag.
* Inputs - Allow for parameterized tests. (similar to how "Theory" works in xUnit)
* Lifecycle Methods -  if the `Setup` or `Cleanup` methods are found on the test class they will be executed appropriately.
* NotTest - Can mark methods with `NotTest` attribute if they are not tests.
* Filter tests by name
* Filter tests by Tags

## Give a Star! :star:

If you like or are using this project please give it a star. Thank you!

## Installation

You can see the latest NuGet packages from the official [TimeWarp NuGet page](https://www.nuget.org/profiles/TimeWarp.Enterprises).

* [timewarp-fixie](https://www.nuget.org/packages/TimeWarp.Fixie/) [![nuget](https://img.shields.io/nuget/v/TimeWarp.Fixie?logo=nuget)](https://www.nuget.org/packages/TimeWarp.Fixie/)

## Usage

### Creating a New Test Project

Create a new test project:

```console
dotnet new classlib -n MyProject.Tests
```

Add NuGet packages to the project:

```console
dotnet add package TimeWarp.Fixie
dotnet add package Fixie.TestAdapter
```

Create a dotnet tool manifest:

```console
dotnet new tool-manifest
```

Add Fixie.Console to the manifest:

```console
dotnet tool install Fixie.Console
```

### Configuring Testing Convention

Inside your Fixie project, create a class that inherits from `Fixie.Conventions.TestingConvention`:

```csharp
class TestingConvention : TimeWarp.Fixie.TestingConvention { }
```

This will use the `TimeWarp.Fixie` convention.

### Creating a Sample Test

First, add FluentAssertions (you could use basic Asserts or any other assertion library):

```csharp
dotnet add package FluentAssertions
```

Create a sample test class named `ConventionTests.cs`:

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

### Executing the Tests

```console
dotnet fixie
```

## Features

### Dependency Injection

Tests are instantiated from the dependency injection container set up for tests, so you can use the same pattern for testing as for production apps.

### Configuring Services for the Execution Phase

To customize the services used in the execution phase, inherit from `TestingConvention` and override the service configuration:

```csharp
namespace TimeWarp.Architecture.Testing;

public class TimeWarpTestingConvention : TestingConvention
{
    public TimeWarpTestingConvention() : base(ConfigureAdditionalServicesCallback) { }

    private static void ConfigureAdditionalServicesCallback(ServiceCollection serviceCollection)
    {
        Console.WriteLine("ConfigureAdditionalServices");
        serviceCollection
            .AddSingleton<WebTestServerApplication>()
            .AddSingleton<ApiTestServerApplication>()
            .AddSingleton<SpaTestApplication<YarpTestServerApplication, TimeWarp.Architecture.Yarp.Server.Program>>()
            .AddSingleton<YarpTestServerApplication>();
    }
}
```

### No Need to Decorate Test Methods with [Test] Attributes

Public methods are test cases by convention:

```csharp
// Xunit style
[Test] // <==== Not needed with TimeWarp Fixie Convention
public void SomeTest()
{
    Assert.Fail();
}
```

```csharp
// TimeWarp Fixie Convention: all public methods are tests 
public void SomeTest()
{
    Assert.Fail();
}
```

### Skip - Mark Tests to Be Skipped

```csharp
[Skip("Reason for skipping")]
public static void SkipExample() => true.Should().BeFalse();
```

### Tags

You can add tags to any of your tests. We include some in the `TestTags` static class, but they are just strings, so you can add whatever you like:

```csharp
[TestTag(TestTags.Fast)]
[TestTag("Bug123")]
public static void TagExample() => true.Should().BeTrue();
```

### Parameterized Tests

Similar to how xUnit uses `[Theory]`, you can run a test for each set of parameters:

```csharp
[Input(5, 3, 2)]
[Input(8, 5, 3)]
public static void Subtract(int aX, int aY, int aExpectedDifference)
{
    int result = aX - aY;
    result.Should().Be(aExpectedDifference);
}
```

### Lifecycle Methods

If the `Setup` or `Cleanup` methods are found on the test class, they will be executed appropriately for each test:

```csharp
public class LifecycleExamples
{
    public static void AlwaysPass() => true.Should().BeTrue();

    [Input(5, 3, 2)]
    [Input(8, 5, 3)]
    public static void Subtract(int aX, int aY, int aExpectedDifference)
    {
        // Will run lifecycles around each Input
        int result = aX - aY;
        result.Should().Be(aExpectedDifference);
    }

    public static void Setup() => Console.WriteLine("Sample Setup");
    public static void Cleanup() => Console.WriteLine("Sample Cleanup");
}
```

### NotTest

If you have a class that needs to be public but does not contain tests, you can mark it as such with the `[NotTest]` attribute. For example:

```csharp
[NotTest]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class NotTest : Attribute { }
```

### Filtering Tests by Name

From Fixie's [docs](https://github.com/fixie/fixie/wiki/Command-Line-Arguments#filtering-with---tests):

The optional argument `--tests` (abbreviated `-t`) lets you specify which tests to run.

A full test name match
