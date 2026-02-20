using TimeScaleApi.Domain;

namespace TimeScaleApi.Application.ImportEntities;

// Implement fast median calculator
public class MedianCalculator : IMedianCalculator
{
    private class DataRecordValueComparer : IComparer<DataRecord>
    {
        public int Compare(DataRecord? x, DataRecord? y)
        {
            return Comparer<double>.Default.Compare(x.Value, y.Value);
        }
    }

    public double FindMedianValue(IEnumerable<DataRecord> values)
    {   
        var list = values.ToList();
        if (list.Count is 0)
        {
            throw new ArgumentOutOfRangeException(nameof(values), $"{nameof(values)} must contain at least one value");
        }

        list.Sort(new DataRecordValueComparer());
        double medianValue = list[list.Count / 2].Value;
        if (list.Count % 2 == 0)
        {
            medianValue = (medianValue + list[list.Count / 2 - 1].Value) / 2;
        }

        return medianValue;
    }
}