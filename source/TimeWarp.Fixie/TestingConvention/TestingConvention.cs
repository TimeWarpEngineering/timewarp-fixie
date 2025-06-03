namespace TimeWarp.Fixie;

using global::Fixie;

/// <summary>
/// Provides a base class for defining testing conventions with Fixie.
/// This class is specifically designed to be inherited to customize the test project lifecycle.
/// dotnet test frameworks have two phases:
/// <see cref="TestDiscovery">discovery</see>
/// <see cref="TestExecution">execution</see>
/// </summary>
/// <seealso href="https://github.com/fixie/fixie/wiki/Customizing-the-Test-Project-Lifecycle"/>
[NotTest]
public abstract class TestingConvention : ITestProject
{
  internal const string SetupLifecycleMethodName = "Setup";
  internal const string CleanupLifecycleMethodName = "Cleanup";
  internal const string ConfigureServicesMethodName = "ConfigureServices";

  private readonly ConfigureAdditionalServicesCallback? ConfigureAdditionalServicesCallback;

  /// <summary>
  /// Constructor for the TestingConvention
  /// </summary>
  /// <param name="configureAdditionalServicesCallback">A callback to configure additional services, if any.</param>
  protected TestingConvention(ConfigureAdditionalServicesCallback? configureAdditionalServicesCallback = null)
  {
    ConfigureAdditionalServicesCallback = configureAdditionalServicesCallback;
  }

  /// <inheritdoc />
  public void Configure(TestConfiguration testConfiguration, TestEnvironment testEnvironment)
  {
    var testDiscovery = new TestDiscovery(testEnvironment.CustomArguments);
    var testExecution = new TestExecution(testEnvironment.CustomArguments, ConfigureAdditionalServicesCallback);

    testConfiguration.Conventions.Add(testDiscovery, testExecution);
  }
}
