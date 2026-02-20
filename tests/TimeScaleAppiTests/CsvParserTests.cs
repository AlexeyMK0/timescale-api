using System.Text;
using Microsoft.Extensions.Options;
using TimeScaleApi.Application.Abstractions.Parser;
using TimeScaleApi.Domain;
using TimeScaleApi.Infrastructure.Parsing.CsvParser;
using TimeScaleApiTests.Helpers;
using Xunit;

namespace TimeScaleApiTests;

public class CsvParserTests
{
    private readonly IOptions<ParserSettings> _parserSettings = Options.Create(new ParserSettings
    {
        DateFormat = DateTimeHelper.TestDateFormat,
        SkipHeader = false,
    });

    private static Stream CreateStream(string streamContent)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(streamContent));
    }

    [Fact]
    public void ValidInput_ParseData_Success()
    {
        // Arrange
        string fileName = "coolFile";
        var csvFile = """
                      2026-02-19T11:10:54.0000Z;10;0
                      2026-02-18T11:10:54.0000Z;20;2
                      """;
        var testData = new DataRecord[]
        {
            new DataRecord(fileName, DateTimeHelper.CreateDateTimeOffset("2026-02-19T11:10:54.0000Z"), 10L, 0.0),
            new DataRecord(fileName, DateTimeHelper.CreateDateTimeOffset("2026-02-18T11:10:54.0000Z"), 20L, 2.0)
        };

        var parser = new CsvParser(_parserSettings);
        using var stream = CreateStream(csvFile);

        // Act
        ParseResult result = parser.ParseFile(stream, fileName);

        // Assert
        var successParse = Assert.IsType<ParseResult.Success>(result);
        var records = successParse.records.ToArray();
        Assert.Equal(testData, records);
    }

    [Fact]
    public void InvalidDateFormat_ParseData_Failure()
    {
        // Arrange
        string fileName = "coolFile";
        var csvFile = """
                      IvalidFormat:10:54.0000Z;10;0
                      2026-02-18T11:10:54.0000Z;20;2
                      """;

        var parser = new CsvParser(_parserSettings);
        using var stream = CreateStream(csvFile);

        // Act
        ParseResult result = parser.ParseFile(stream, fileName);

        // Assert
        Assert.IsType<ParseResult.Failure>(result);
    }

    [Fact]
    public void InvalidTimeFormat_ParseData_Failure()
    {
        // Arrange
        string fileName = "coolFile";
        var csvFile = """
                      2026-02-19T11:10:54.0000Z;InvalidTime;0
                      2026-02-18T11:10:54.0000Z;20;2
                      """;

        var parser = new CsvParser(_parserSettings);
        using var stream = CreateStream(csvFile);

        // Act
        ParseResult result = parser.ParseFile(stream, fileName);

        // Assert
        Assert.IsType<ParseResult.Failure>(result);
    }

    [Fact]
    public void InvalidValueFormat_ParseData_Failure()
    {
        // Arrange
        string fileName = "coolFile";
        var csvFile = """
                      2026-02-19T11:10:54.0000Z;10;InvalidValue
                      2026-02-18T11:10:54.0000Z;20;2
                      """;

        var parser = new CsvParser(_parserSettings);
        using var stream = CreateStream(csvFile);

        // Act
        ParseResult result = parser.ParseFile(stream, fileName);

        // Assert
        Assert.IsType<ParseResult.Failure>(result);
    }
}