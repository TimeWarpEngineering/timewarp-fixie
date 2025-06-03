namespace TimeWarp.Fixie.Tests;

using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;

public interface ITestService
{
  string GetMessage();
}

public class MockTestService : ITestService
{
  public string GetMessage() => "Mock Implementation";
}

public class ConfigureServicesTests
{
  private readonly ITestService TestService;

  public ConfigureServicesTests(ITestService testService)
  {
    TestService = testService;
  }

  public static void ConfigureServices(IServiceCollection services)
  {
    Console.WriteLine("ConfigureServices called for ConfigureServicesTests");
    services.AddScoped<ITestService, MockTestService>();
  }

  public void Should_Use_Mock_Service()
  {
    TestService.ShouldNotBeNull();
    TestService.GetMessage().ShouldBe("Mock Implementation");
  }

  public void Should_Have_Same_Service_Instance_In_Same_Test_Class()
  {
    TestService.ShouldNotBeNull();
    TestService.GetMessage().ShouldBe("Mock Implementation");
  }
}

public class AlternativeMockTestService : ITestService
{
  public string GetMessage() => "Alternative Mock Implementation";
}

public class AlternativeConfigureServicesTests
{
  private readonly ITestService TestService;

  public AlternativeConfigureServicesTests(ITestService testService)
  {
    TestService = testService;
  }

  public static void ConfigureServices(IServiceCollection services)
  {
    Console.WriteLine("ConfigureServices called for AlternativeConfigureServicesTests");
    services.AddScoped<ITestService, AlternativeMockTestService>();
  }

  public void Should_Use_Alternative_Mock_Service()
  {
    TestService.ShouldNotBeNull();
    TestService.GetMessage().ShouldBe("Alternative Mock Implementation");
  }
}
