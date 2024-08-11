namespace TimeWarp.Fixie;

using global::Fixie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/// <summary>
/// Fixie allows for the configuration of a custom test discovery process. This is our implementation.
/// </summary>
/// <remarks>This convention looks for all classes that are public and do not have the <see cref="NotTest"/> attribute
/// And all methods within those classes that are not named with the value in <see cref="TestingConvention.SetupLifecycleMethodName"/> are tests
/// </remarks>
[NotTest]
public sealed class TestDiscovery : IDiscovery
{
  private readonly IReadOnlyList<string> CustomArguments;

  public TestDiscovery(IReadOnlyList<string> customArguments)
  {
    CustomArguments = customArguments;
  }

  /// <inheritdoc/>
  public IEnumerable<Type> TestClasses(IEnumerable<Type> concreteClasses) =>
    concreteClasses
      .Where(TestClassFilter())
      .Where(TagClassFilter());

  /// <inheritdoc/>
  public IEnumerable<MethodInfo> TestMethods(IEnumerable<MethodInfo> publicMethods) =>
    publicMethods
      .Where(TestMethodFilter())
      .Where(TagMethodFilter());

  internal static Func<Type, bool> TestClassFilter() =>
    type => type.IsPublic && !type.Has<NotTest>();

  private Func<Type, bool> TagClassFilter() =>
    type =>
      CustomArguments.Count == 0 ||
        type
          .GetCustomAttributes<TestTagAttribute>()
          .Select(testTagAttribute => testTagAttribute.Tag)
          .Intersect(CustomArguments)
          .Any();

  private static Func<MethodInfo, bool> TestMethodFilter() =>
    methodInfo =>
      !methodInfo.IsSpecialName &&
      methodInfo.Name != TestingConvention.SetupLifecycleMethodName &&
      methodInfo.Name != TestingConvention.CleanupLifecycleMethodName;

  private Func<MethodInfo, bool> TagMethodFilter() =>
    methodInfo =>
      CustomArguments.Count == 0 ||
        methodInfo
          .GetCustomAttributes<TestTagAttribute>()
          .Select(testTagAttribute => testTagAttribute.Tag)
          .Intersect(CustomArguments)
          .Any();
}
