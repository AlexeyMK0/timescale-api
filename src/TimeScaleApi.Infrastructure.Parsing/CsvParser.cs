using System.Globalization;
using TimeScaleApi.Application.Abstractions;
using TimeScaleApi.Application.Abstractions.Parser;
using TimeScaleApi.Domain;

namespace TimeScaleApi.Infrastructure.Parsing;

// TODO: implement via CSV Helper
public class CsvParser : ICsvParser
{
    // private readonly string _parseFormat = "yyyy-MM-ddThh-mm-ss.ffff'Z'";
    private readonly string _parseFormat = "yyyy-MM-ddThh:mm:ss.ffff'Z'";

    public ParseResult ParseFile(Stream stream, string fileName)
    {
        using var reader = new StreamReader(stream);
        int currentIndex = 0;
        var records = new List<DataRecord>();
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            currentIndex++;
            string[] columns = line.Split(';');
            if (columns.Length != 3)
            {
                return LineReadingError(
                    currentIndex, "CSV expected 3 columns, actual: " + columns.Length);
            }

            if (!DateTimeOffset.TryParseExact(columns[0], _parseFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal, out var date))
            {
                return LineReadingError(currentIndex, "expected date at position 1, actual: " + columns[0]);
            }

            if (!long.TryParse(columns[1], out var executionTime))
            {
                return LineReadingError(currentIndex, "expected long at position 2, actual: " + columns[1]);
            }

            if (!double.TryParse(columns[2], out var value))
            {
                return LineReadingError(currentIndex, "expected double at position 3, actual: " + columns[2]);
            }

            var record = new DataRecord(fileName, date, executionTime, value);
            records.Add(record);
        }

        return new ParseResult.Success(records);
        // var config = new CsvConfiguration(CultureInfo.CurrentCulture)
        // {
        //     NewLine = Environment.NewLine,
        // };
        // using (var csv = new CsvReader(new StreamReader(stream), config))
        // {
        //     var records = csv.GetRecords<DataRecord>();
        // }
    }

    private ParseResult.Failure LineReadingError(int stringIndex, string reason)
    {
        return new ParseResult.Failure(
            $"Error reading CSV line at line: {stringIndex} " + reason);
    }
}