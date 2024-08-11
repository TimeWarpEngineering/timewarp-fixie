namespace TimeWarp.Fixie;

using global::Fixie;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

public delegate void ConfigureAdditionalServicesCallback(ServiceCollection serviceCollection);

/// <summary>
/// Fixie allows for the configuration of a custom test execution process. This is our base implementation.
/// </summary>
/// <remarks>This convention looks for all classes that are public and do not have the <see cref="NotTest"/> attribute
/// And all methods within those classes that are not named with the value in <see cref="TestingConvention.SetupLifecycleMethodName"/> are tests
/// </remarks>
[NotTest]
public sealed class TestExecution : IExecution
{
  private readonly ServiceProvider ServiceProvider;
  private readonly IReadOnlyList<string> CustomArguments;
  private readonly ConfigureAdditionalServicesCallback? ConfigureAdditionalServicesCallback;

  public TestExecution(IReadOnlyList<string> aCustomArguments, ConfigureAdditionalServicesCallback? configureAdditionalServicesCallback = null)
  {
    ConfigureAdditionalServicesCallback = configureAdditionalServicesCallback;
    var testServices = new ServiceCollection();
    ConfigureTestServices(testServices);
    ServiceProvider = testServices.BuildServiceProvider();
    CustomArguments = aCustomArguments;
  }

  /// <summary>
  /// This is required implementation of the IExecution interface
  /// </summary>
  /// <param name="testSuite"></param>
  /// <remarks>
  /// Each test is run in a new <see cref="IServiceScope"/> created by the registered <see cref="IServiceScopeFactory"/>
  /// For each test/method the following is executed:
  /// <see cref="Setup(object, TestClass)"/>
  /// <see cref="Run(TestSuite)"/>
  /// <see cref="Cleanup(object, TestClass)"/>
  /// </remarks>
  public async Task Run(TestSuite testSuite)
  {
    IServiceScopeFactory serviceScopeFactory = ServiceProvider.GetService<IServiceScopeFactory>()!;
    foreach (TestClass testClass in testSuite.TestClasses)
    {
      Console.WriteLine($"==== Executing Cases for the class {testClass.Type.FullName} ====");
      foreach (Test test in testClass.Tests)
      {
        if (test.Has(out SkipAttribute? skip))
        {
          await test.Skip(skip.Reason);
          continue;
        }
        using IServiceScope serviceScope = serviceScopeFactory.CreateScope();
        object instance = serviceScope.ServiceProvider.GetService(testClass.Type)!;


        if (test.HasParameters)
        {
          IEnumerable<object[]> inputs = test.GetAll<InputAttribute>().Select(aInput => aInput.Parameters);

          foreach (object[] parameters in inputs)
          {
            Console.WriteLine($"==== Executing test: {test.Name} with inputs ====");
            await TryLifecycleMethod(instance, testClass, TestingConvention.SetupLifecycleMethodName);
            await test.Run(instance, parameters);
            await TryLifecycleMethod(instance, testClass, TestingConvention.CleanupLifecycleMethodName);
          }
        }
        else
        {
          Console.WriteLine($"==== Executing test: {test.Name} ====");
          await TryLifecycleMethod(instance, testClass, TestingConvention.SetupLifecycleMethodName);
          await test.Run(instance);
          await TryLifecycleMethod(instance, testClass, TestingConvention.CleanupLifecycleMethodName);
        }
      }
    }

    await (serviceScopeFactory as IAsyncDisposable)!.DisposeAsync();
  }

  /// <summary>
  /// Registers all the items in the <see cref="ServiceCollection"/>
  /// </summary>
  /// <param name="serviceCollection"></param>
  public void ConfigureTestServices(ServiceCollection serviceCollection)
  {
    Console.WriteLine($"==== {nameof(ConfigureTestServices)} ====");
    ConfigureAdditionalServices(serviceCollection);

    // Configure any test class dependencies here.
    serviceCollection.AddSingleton(new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

    RegisterTests(serviceCollection);
  }

  /// <summary>
  /// Add the <see cref="TestServerApplication{TStartup}">applications</see> to be running as Singletons to the ServiceCollection
  /// </summary>
  /// <param name="serviceCollection"></param>
  public void ConfigureAdditionalServices(ServiceCollection serviceCollection)
  {
    ConfigureAdditionalServicesCallback?.Invoke(serviceCollection);
  }

  /// <summary>
  /// we use a service collection to create the test classes. This method registers them by scanning the
  /// entry assembly.
  /// </summary>
  /// <remarks>This Filter uses the same one used in <see cref="TestDiscovery"/> </remarks>
  /// <param name="serviceCollection"></param>
  private static void RegisterTests(IServiceCollection serviceCollection)
  {
    serviceCollection.Scan
    (
      typeSourceSelector => typeSourceSelector
        .FromEntryAssembly()
        .AddClasses(action: (classes) => classes.Where(TestDiscovery.TestClassFilter()))
        .AsSelf()
        .WithScopedLifetime()
    );
  }

  private static async Task TryLifecycleMethod(object instance, TestClass testClass, string methodName)
  {
    if (instance is null) { throw new ArgumentNullException(nameof(instance)); }

    MethodInfo? methodInfo = testClass.Type.GetMethod(methodName);
    if (methodInfo is not null)
    {
      Console.WriteLine($"==== Run Lifecycle method: {methodName} ====");
      await methodInfo.Call(instance);
    }
  }
}
