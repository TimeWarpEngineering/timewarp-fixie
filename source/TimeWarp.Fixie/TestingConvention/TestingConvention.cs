﻿namespace TimeWarp.Fixie;

using global::Fixie;

/// <summary>
/// Fixie allows for the customization of the Test Project Lifecycle
/// Here we set our implementations of these phases
/// dotnet test frameworks have two phases:
/// <see cref="TestDiscovery">discovery</see>
/// <see cref="TestExecution">execution</see>
/// </summary>
/// <seealso href="https://github.com/fixie/fixie/wiki/Customizing-the-Test-Project-Lifecycle"/>
[NotTest]
public class TestingConvention : ITestProject
{
  internal const string SetupLifecycleMethodName = "Setup";
  internal const string CleanupLifecycleMethodName = "Cleanup";

  private readonly ConfigureAdditionalServicesCallback? ConfigureAdditionalServicesCallback;

  public TestingConvention(ConfigureAdditionalServicesCallback? configureAdditionalServicesCallback = null)
  {
    ConfigureAdditionalServicesCallback = configureAdditionalServicesCallback;
  }

  public void Configure(TestConfiguration aTestConfiguration, TestEnvironment aTestEnvironment)
  {
    var testDiscovery = new TestDiscovery(aTestEnvironment.CustomArguments);
    var testExecution = new TestExecution(aTestEnvironment.CustomArguments, ConfigureAdditionalServicesCallback);

    aTestConfiguration.Conventions.Add(testDiscovery, testExecution);
  }
}
