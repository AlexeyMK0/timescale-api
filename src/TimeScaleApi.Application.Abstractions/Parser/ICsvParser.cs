namespace TimeScaleApi.Application.Abstractions.Parser;

public interface ICsvParser
{   
    ParseResult ParseFile(Stream stream, string fileName);
}