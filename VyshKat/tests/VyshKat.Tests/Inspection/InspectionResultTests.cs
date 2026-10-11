using VyshKat.Core.Inspection;

namespace VyshKat.Tests.Inspection;

public sealed class InspectionResultTests
{
    [Fact]
    public void Rejected_WithoutReason_IsNotAllowed()
    {
        Assert.Throws<ArgumentException>(() => InspectionResult.Rejected());
        Assert.Throws<ArgumentException>(() => InspectionResult.Rejected(" ", ""));
    }

    [Fact]
    public void Rejected_TrimsReasonsAndDropsBlankOnes()
    {
        var result = InspectionResult.Rejected("  нет света ", "");

        Assert.Equal("нет света", Assert.Single(result.Remarks));
    }
}
