using System.Collections.Generic;
using FluentAssertions;
using Ribbanya.Utilities.Collections;
using Ribbanya.Utilities.;
using Xunit;

namespace Ribbanya.Utilities.Tests {
  public sealed class CSharpPortTests {
    [Fact]
    public void IndexOfTest() {
      var list = (IEnumerable<int>) new List<int> {10, 20, 30};
      var index = list.IndexOf(20);
      index.Should().Be(1);
    }

    [Fact]
    public void EnumerableEqualsTest() {
      var listA = (IEnumerable<int>) new List<int> {10, 20, 30};
      var listB = (IEnumerable<int>) new List<int> {10, 20, 30};
      listA.EnumerableEquals(listB).Should().BeTrue();
    }

    [Fact]
    public void SwapTest() {
      var a = 123;
      var b = 456;
      UtilityHelper.Swap(ref a, ref b);
      a.Should().Be(456);
      b.Should().Be(123);
    }

    [Fact]
    public void GetFullNameTest() {
      var member = typeof(CSharpPortTests).GetMethod(nameof(GetFullNameTest));
      member.GetFullName().Should().Be($"{typeof(CSharpPortTests).FullName}.{nameof(GetFullNameTest)}");
    }

    [Fact]
    public void GetDefaultValueTest() {
      var nonGeneric = typeof(CSharpPortTests).GetDefaultValue();
      var generic = typeof(CSharpPortTests).GetDefaultValue<CSharpPortTests>();
      nonGeneric.Should().BeNull();
      generic.Should().BeNull();
    }

    [Fact]
    public void OverloadFactoryTest() {
      var x = SpecialParameter.Unspecified;
      var y = OverloadFactory.Unspecified;
      x.Should().BeSameAs(y);
    }
  }
}