# 001_Class-Specific-ConfigureServices.md

## Description

Implement support for class-specific static `ConfigureServices` method in TimeWarp.Fixie to allow test classes to register their own mock implementations and dependencies. This enables different test classes to use different mock implementations (e.g., MockA for TestClass1, MockB for TestClass2).

## Requirements

- Add support for a static `ConfigureServices(IServiceCollection services)` method on test classes
- Method called during service registration phase to allow per-class service overrides
- Must be non-breaking - existing tests continue to work unchanged
- Follow existing reflection-based convention pattern used by `Setup` and `Cleanup` lifecycle methods
- Class-specific registrations override global registrations for that test class

## Implementation Tasks

- [x] Modify [`RegisterTests`](source/TimeWarp.Fixie/TestingConvention/TestExecution.cs:119) method to scan for and invoke `ConfigureServices` on test classes
- [x] Add reflection logic similar to [`TryLifecycleMethod`](source/TimeWarp.Fixie/TestingConvention/TestExecution.cs:131) pattern
- [x] Add constant for method name (similar to [`SetupLifecycleMethodName`](source/TimeWarp.Fixie/TestingConvention/TestingConvention.cs:16))
- [x] Create integration tests
- [x] Update README.md with feature documentation and examples

## Example Usage

```csharp
public class UserServiceTests
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, MockUserRepository>();
        services.AddScoped<IEmailService, MockEmailService>();
    }

    public void Should_Create_User() { /* test implementation */ }
}

public class OrderServiceTests
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, FakeUserRepository>();
        services.AddScoped<IPaymentService, MockPaymentService>();
    }

    public void Should_Process_Order() { /* test implementation */ }
}
```

## Implementation Notes

[Include notes while task is in progress]
