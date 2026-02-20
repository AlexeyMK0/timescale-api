using TimeScaleApi.Application.ImportEntities;
using TimeScaleApi.Domain;
using TimeScaleApiTests.Helpers;
using Xunit;

namespace TimeScaleApiTests;

public class MedianCalculatorTests
{
    private const string FileName = "CoolFile";
    
    private readonly DataRecord[] testDataOdd = new DataRecord[]
    {
        new DataRecord(FileName, DateTimeHelper.CreateDateTimeOffset("2026-02-19T11:10:54.0000Z"), 10L, 0.0),
        new DataRecord(FileName, DateTimeHelper.CreateDateTimeOffset("2026-02-18T11:10:54.0000Z"), 20L, 2.0),
        new DataRecord(FileName, DateTimeHelper.CreateDateTimeOffset("2026-02-18T11:10:54.0000Z"), 30L, 4.0)
    };
    
    private readonly DataRecord[] testDataEven = new DataRecord[]
    {
        new DataRecord(FileName, DateTimeHelper.CreateDateTimeOffset("2026-02-19T11:10:54.0000Z"), 10L, 0.0),
        new DataRecord(FileName, DateTimeHelper.CreateDateTimeOffset("2026-02-18T11:10:54.0000Z"), 20L, 2.0)
    };

    [Fact]
    public void OddLength_FindMedian_Correct()
    {
        // Arrange
        var medianFinder = new MedianCalculator();
        var testData = testDataOdd.ToList();
        
        // Act
        double median = medianFinder.FindMedianValue(testData);
        
        // Assert
        Assert.Equal(2.0, median);
    }
    
    [Fact]
    public void EvenLength_FindMedian_Correct()
    {
        // Arrange
        var medianFinder = new MedianCalculator();
        var testData = testDataEven.ToList();
        
        // Act
        double median = medianFinder.FindMedianValue(testData);
        
        // Assert
        Assert.Equal(1.0, median);
    }
}