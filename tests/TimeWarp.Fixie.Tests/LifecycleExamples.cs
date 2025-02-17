namespace ConventionTest_;

using Shouldly;
using System;
using TimeWarp.Fixie;

public class LifecycleExamples
{
  public static void AlwaysPass() => true.ShouldBeTrue();

  [Input(5, 3, 2)]
  [Input(8, 5, 3)]
  public static void Subtract(int aX, int aY, int aExpectedDifference)
  {
    // Will run lifecycles around each Input
    int result = aX - aY;
    result.ShouldBe(aExpectedDifference);
  }

  public static void Setup() => Console.WriteLine("Sample Setup");
  public static void Cleanup() => Console.WriteLine("Sample Cleanup");
}
