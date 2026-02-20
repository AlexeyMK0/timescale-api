using System.Globalization;

namespace TimeScaleApiTests.Helpers;

public class DateTimeHelper
{
    public const string TestDateFormat = "yyyy-MM-ddThh:mm:ss.ffff'Z'";
    
    public static readonly DateTimeOffset TestMinDateTime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);  
    
    public static DateTimeOffset CreateDateTimeOffset(string dateTime)
    {
        return DateTimeOffset.ParseExact(
            dateTime,
            TestDateFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal);
    }
}