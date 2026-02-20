using TimeScaleApi.Domain;

namespace TimeScaleApi.Application.ImportEntities;

public interface IMedianCalculator
{
     double FindMedianValue(IEnumerable<DataRecord> values);
}