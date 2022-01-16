using DeviceEmulator.Model.Values;
using Shouldly;
using System;
using Xunit;

namespace DeviceEmulatorUnitTests.Model
{
    public class FractionTests
    {
        private double _epsilon = 0.000001;

        [Fact]
        public void Fraction_MustBeBetween_Zero_And_Whole()
        {
            Fraction.FromFraction(0).ShouldNotBeNull();
            Fraction.FromFraction(1).ShouldNotBeNull();
            Fraction.FromPercentage(0).ShouldNotBeNull();
            Fraction.FromPercentage(100).ShouldNotBeNull();
            Should.Throw<ArgumentException>(() => Fraction.FromFraction(-_epsilon));
            Should.Throw<ArgumentException>(() => Fraction.FromFraction(1 + _epsilon));
            Should.Throw<ArgumentException>(() => Fraction.FromPercentage(-_epsilon));
            Should.Throw<ArgumentException>(() => Fraction.FromPercentage(100 + _epsilon));
        }

        [Fact]
        public void BaseValueConversions_ShouldWork()
        {
            var b1 = 0.123456;
            var b100 = 12.3456;
            var b100r = 12.35;
            Fraction.FromFraction(b1).ShouldSatisfyAllConditions(
                f => f.Base1Value.ShouldBe(b1),
                f => f.Base100Value.ShouldBe(b100),
                f => f.Base100ValueRounded.ShouldBe(b100r));
            Fraction.FromPercentage(b100).ShouldSatisfyAllConditions(
                f => f.Base1Value.ShouldBe(b1),
                f => f.Base100Value.ShouldBe(b100),
                f => f.Base100ValueRounded.ShouldBe(b100r));
        }

        [Fact]
        public void Fraction_ShouldBe_Comparable()
        {
            var bigger = Fraction.FromFraction(0.9);
            var middle = Fraction.FromFraction(0.5);
            var smaller = Fraction.FromFraction(0.1);
            var b2 = bigger;
            (bigger > middle).ShouldBeTrue();
            (bigger > smaller).ShouldBeTrue();
            (middle > smaller).ShouldBeTrue();
            (bigger >= middle).ShouldBeTrue();
            (bigger >= smaller).ShouldBeTrue();
            (middle >= smaller).ShouldBeTrue();
            (bigger >= b2).ShouldBeTrue();
            (bigger < middle).ShouldBeFalse();
            (bigger < smaller).ShouldBeFalse();
            (middle < smaller).ShouldBeFalse();
            (bigger <= middle).ShouldBeFalse();
            (bigger <= smaller).ShouldBeFalse();
            (middle <= smaller).ShouldBeFalse();
            (bigger <= b2).ShouldBeTrue();
            (smaller < bigger).ShouldBeTrue();
            (bigger <= b2).ShouldBeTrue();
        }
    }
}
