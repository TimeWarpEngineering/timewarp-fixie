namespace TimeWarp.Fixie.Tests;

using Microsoft.Extensions.DependencyInjection;
using System;

class TestProject : TimeWarp.Fixie.TestingConvention
{

  public TestProject() : base(ConfigureAdditionalServicesCallback) { }

  private static void ConfigureAdditionalServicesCallback(ServiceCollection serviceCollection)
  {
    Console.WriteLine("ConfigureAdditionalServices");
    // One would configure their Appplication Objects here as well as any other test services
  }
}
