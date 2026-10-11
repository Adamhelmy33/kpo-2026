using VyshKat.Core.Rules;

namespace VyshKat.Tests.Rules;

public sealed class EaseOfUseBeginnerRuleTests
{
    private readonly EaseOfUseBeginnerRule _rule = new();

    [Theory]
    [InlineData(1, false)]
    [InlineData(5, false)] // ниже порога
    [InlineData(6, true)]  // ровно на пороге: ≥ 6 подходит
    [InlineData(7, true)]  // выше порога
    [InlineData(10, true)]
    public void IsSuitableForBeginners_UsesThresholdSixInclusive(int easeOfUse, bool expected)
    {
        var scooter = TestData.Scooter(easeOfUse: easeOfUse);

        Assert.Equal(expected, _rule.IsSuitableForBeginners(scooter));
    }

    [Fact]
    public void DefaultThreshold_IsSix()
    {
        Assert.Equal(6, _rule.MinimumEaseOfUse);
        Assert.Equal("простота ≥ 6", _rule.Description);
    }

    [Fact]
    public void Rule_AppliesToAnyVehicleType()
    {
        Assert.True(_rule.IsSuitableForBeginners(TestData.Bicycle(easeOfUse: 6)));
        Assert.False(_rule.IsSuitableForBeginners(TestData.EBike(easeOfUse: 5)));
    }

    [Fact]
    public void CustomThreshold_CanMakeRuleStricter()
    {
        var strictRule = new EaseOfUseBeginnerRule(minimumEaseOfUse: 8);

        Assert.False(strictRule.IsSuitableForBeginners(TestData.Scooter(easeOfUse: 7)));
        Assert.True(strictRule.IsSuitableForBeginners(TestData.Scooter(easeOfUse: 8)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void Threshold_OutsideScale_IsRejected(int threshold)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new EaseOfUseBeginnerRule(threshold));
    }

    [Fact]
    public void NullVehicle_IsRejected()
    {
        Assert.Throws<ArgumentNullException>(() => _rule.IsSuitableForBeginners(null!));
    }
}
