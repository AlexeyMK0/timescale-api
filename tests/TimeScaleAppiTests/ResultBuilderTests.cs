using TimeScaleApi.Application.ImportEntities;
using TimeScaleApi.Domain;
using TimeScaleApiTests.Helpers;
using Xunit;

namespace TimeScaleApiTests;

public class ResultBuilderTests
{
    private const string FileName = "CoolFile";
    
    private readonly DataRecord[] testData = new DataRecord[]
    {
        new DataRecord(FileName, DateTimeHelper.CreateDateTimeOffset("2026-02-19T11:10:54.0000Z"), 10L, 0.0),
        new DataRecord(FileName, DateTimeHelper.CreateDateTimeOffset("2026-02-18T11:10:54.0000Z"), 20L, 2.0)
    };
    
    private readonly DataRecord[] testDataOdd = new DataRecord[]
    {
        new DataRecord(FileName, DateTimeHelper.CreateDateTimeOffset("2026-02-19T11:10:54.0000Z"), 10L, 0.0),
        new DataRecord(FileName, DateTimeHelper.CreateDateTimeOffset("2026-02-18T11:10:54.0000Z"), 20L, 2.0),
        new DataRecord(FileName, DateTimeHelper.CreateDateTimeOffset("2026-02-18T11:10:54.0000Z"), 30L, 4.0)
    };
    
    [Fact]
    public void CorrectData_BuildRecord_Success()
    {
        // Arrange
        var builder = new FileRecordBuilder(DateTimeHelper.TestMinDateTime, 100, 1, new MedianCalculator());
        var fileRecord = new FileRecord(
            FileName,
            TimeSpan.FromDays(1),
            DateTimeHelper.CreateDateTimeOffset("2026-02-18T11:10:54.0000Z"),
            15,
            1.0,
            1.0,
            2.0,
            0.0); 
        
        // Act
        var firstAddError = builder.AddDataRecord(testData[0]);
        var secondAddError = builder.AddDataRecord(testData[1]);
        var result = builder.Build();
        
        // Assert
        Assert.Null(firstAddError);
        Assert.Null(secondAddError);
        var successResult = Assert.IsType<IFileRecordBuilder.BuildResult.Success>(result);
        Assert.Equal(fileRecord, successResult.Record);
    }
    
    [Fact]
    public void DifferentNames_AddData_FailureAdding()
    {
        // Arrange
        DataRecord[] localTestData = new DataRecord[]
        {
            new DataRecord("name1", DateTimeHelper.CreateDateTimeOffset("2026-02-19T11:10:54.0000Z"), 10L, 0.0),
            new DataRecord("different name", DateTimeHelper.CreateDateTimeOffset("2026-02-18T11:10:54.0000Z"), 20L, 2.0)
        };
        var builder = new FileRecordBuilder(DateTimeHelper.TestMinDateTime, 100, 1, new MedianCalculator());
        
        // Act
        var firstAdd = builder.AddDataRecord(localTestData[0]);
        var secondAdd = builder.AddDataRecord(localTestData[1]);
        
        // Assert
        Assert.Null(firstAdd);
        Assert.NotNull(secondAdd);
    }
    
    [Fact]
    public void AddLimitExceeded_AddData_FailureAdding()
    {
        // Arrange
        var builder = new FileRecordBuilder(DateTimeHelper.TestMinDateTime, 1, 1, new MedianCalculator());
        
        // Act
        var firstAdd = builder.AddDataRecord(testData[0]);
        var secondAdd = builder.AddDataRecord(testData[1]);
        
        // Assert
        Assert.Null(firstAdd);
        Assert.NotNull(secondAdd);
    }

    [Fact]
    void TimeIsInFuture_AddData_FailureAdding()
    {
        // Arrange
        DataRecord localData = new DataRecord(
            "name1",
            DateTimeOffset.MaxValue, // Future enough
            10L,
            0.0);
    
        var builder = new FileRecordBuilder(DateTimeHelper.TestMinDateTime, 100, 1, new MedianCalculator());
        
        // Act
        var firstAdd = builder.AddDataRecord(localData);
        
        // Assert
        Assert.NotNull(firstAdd);
    }
    
    [Fact]
    void TimeIsTooEarly_AddData_FailureAdding()
    {
        // Arrange
        DataRecord localData = new DataRecord(
            "name1",
            DateTimeOffset.MinValue, // Past enough
            10L,
            0.0);
    
        var builder = new FileRecordBuilder(DateTimeHelper.TestMinDateTime, 100, 1, new MedianCalculator());
        
        // Act
        var firstAdd = builder.AddDataRecord(localData);
        
        // Assert
        Assert.NotNull(firstAdd);
    }
    
    [Fact]
    void ValueIsNegative_AddData_FailureAdding()
    {
        // Arrange
        DataRecord localData = new DataRecord(
            "name1",
            DateTimeHelper.CreateDateTimeOffset("2026-02-19T11:10:54.0000Z"),
            10L,
            -10d);
    
        var builder = new FileRecordBuilder(DateTimeHelper.TestMinDateTime, 100, 1, new MedianCalculator());
        
        // Act
        var firstAdd = builder.AddDataRecord(localData);
        
        // Assert
        Assert.NotNull(firstAdd);
    }
    
    [Fact]
    void ExecutionTimeIsNegative_AddData_FailureAdding()
    {
        // Arrange
        DataRecord localData = new DataRecord(
            "name1",
            DateTimeHelper.CreateDateTimeOffset("2026-02-19T11:10:54.0000Z"),
            -10L,
            0.0);
    
        var builder = new FileRecordBuilder(DateTimeHelper.TestMinDateTime, 100, 1, new MedianCalculator());
        
        // Act
        var firstAdd = builder.AddDataRecord(localData);
        
        // Assert
        Assert.NotNull(firstAdd);
    }
    
    [Fact]
    void LackOfData_AddData_FailureAdding()
    {
        // Arrange
        DataRecord localData = new DataRecord(
            "name1",
            DateTimeHelper.CreateDateTimeOffset("2026-02-19T11:10:54.0000Z"),
            10L,
            0.0);
    
        var builder = new FileRecordBuilder(DateTimeHelper.TestMinDateTime, 100, 10, new MedianCalculator());
        
        // Act
        var firstAdd = builder.AddDataRecord(localData);
        var buildResult = builder.Build();
        
        // Assert
        Assert.Null(firstAdd);
        Assert.IsType<IFileRecordBuilder.BuildResult.Failure>(buildResult);
    }
}