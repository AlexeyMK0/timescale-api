using TimeScaleApi.Domain;

namespace TimeScaleApi.Application.Abstractions.Parser;

public abstract record ParseResult
{
    public sealed record Success(IEnumerable<DataRecord> records) : ParseResult;

    public sealed record Failure(string Message) : ParseResult;
}