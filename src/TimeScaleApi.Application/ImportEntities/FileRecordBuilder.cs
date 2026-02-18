using TimeScaleApi.Domain;

namespace TimeScaleApi.Application.ImportEntities;

public class FileRecordBuilder : IFileRecordBuilder
{
    private readonly DateTimeOffset MinDateTime;
    private readonly int MaxDataRecordCount;
    private readonly int MinDataRecordCount;

    private readonly List<DataRecord> _dataRecords = [];
    private readonly IMedianCalculator _medianCalculator;
    private string? _name;
    private DateTimeOffset _minDate = DateTimeOffset.MaxValue;
    private DateTimeOffset _maxDate = DateTimeOffset.MinValue;
    private double _minValue = double.MaxValue;
    private double _maxValue = double.MinValue;
    private long _executionTimeSum = 0;
    private double _valueSum = 0;

    public FileRecordBuilder(DateTimeOffset minDateTime, int maxDataRecordCount, int minDataRecordCount,
        IMedianCalculator medianCalculator)
    {
        MinDateTime = minDateTime;
        MaxDataRecordCount = maxDataRecordCount;
        MinDataRecordCount = minDataRecordCount;
        _medianCalculator = medianCalculator;
    }

    public string? AddDataRecord(DataRecord dataRecord)
    {
        if (_dataRecords.Count >= MaxDataRecordCount)
        {
            return AddingError("Maximum number of data records exceeded");
        }

        if (dataRecord.Date > DateTimeOffset.UtcNow)
        {
            return AddingError("Date is in the future");
        }

        if (dataRecord.Date < MinDateTime)
        {
            return AddingError("Date must be after {MinDateTime}");
        }

        if (dataRecord.Value < 0)
        {
            return AddingError("Value must be positive");
        }

        if (dataRecord.ExecutionTime < 0)
        {
            return AddingError("ExecutionTime must be positive");
        }

        UpdateMetrics(dataRecord);
        
        _dataRecords.Add(dataRecord);
        return null;
    }

    public IFileRecordBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public IFileRecordBuilder.BuildResult Build()
    {
        if (_dataRecords.Count < MinDataRecordCount)
        {
            return new IFileRecordBuilder.BuildResult.Failure(
                $"Minimum number of data records is {MinDataRecordCount}, actual: {_dataRecords.Count}");
        }

        if (_dataRecords.Count > MaxDataRecordCount)
        {
            return new IFileRecordBuilder.BuildResult.Failure(
                $"Maximum number of data records is {MaxDataRecordCount} actual: {_dataRecords.Count}");
        }

        var fileRecord = new FileRecord(
            _name ?? throw new InvalidOperationException("_name is null"),
            _maxDate - _minDate,
            _minDate,
            _executionTimeSum / (double)_dataRecords.Count,
            _valueSum / _dataRecords.Count,
            _medianCalculator.FindMedianValue(_dataRecords),
            _maxValue,
            _minValue);
        Console.Out.WriteLine("DeltaTime: " + (_maxDate - _minDate).ToString());
        return new IFileRecordBuilder.BuildResult.Success(fileRecord);
    }

    private void UpdateMetrics(DataRecord dataRecord)
    {
        if (dataRecord.Date < _minDate)
        {
            _minDate = dataRecord.Date;
        }
        if (dataRecord.Date > _maxDate)
        {
            Console.Out.WriteLine("Updating maxDate with: " + dataRecord.Date.ToString());
            _maxDate = dataRecord.Date;
        }
        
        _minValue = Math.Min(_minValue, dataRecord.Value);
        _maxValue = Math.Max(_maxValue, dataRecord.Value);
        _executionTimeSum += dataRecord.ExecutionTime;
        _valueSum += dataRecord.Value;
    }
    
    private string AddingError(string message)
    {
        return $"Error while adding data record with index: {_dataRecords.Count + 1}: {message}";
    }
}