using System;
using System.Globalization;


public static class DateUtils 
{

    public static long UTCMilliseconds()
    {
        return new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
    }

    public static DateTime DateTimeNow()
    {
        return DateTime.Now;
    }

    public static long? ConvertUTCStringToTsMilliseconds(string timeText, string dateFormat = "yyyy-MM-dd HH:mm:ss")
    {
        if (timeText == null || timeText.Trim() == "") return null;

        // Specify that the input time is in UTC
        var ts = DateTime.ParseExact(timeText, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
        return new DateTimeOffset(ts).ToUnixTimeMilliseconds();
    }

    public static string ConvertMillisecondsToUTCString(long? millisec, string dateFormat = "yyyy-MM-dd HH:mm:ss")
    {
        if (millisec == null) return null;
        return DateTimeOffset.FromUnixTimeMilliseconds((long)millisec).UtcDateTime.ToString(dateFormat);
    }

    public static string ConvertMillisecondsToLocalString(long? millisec, string dateFormat = "yyyy-MM-dd HH:mm:ss")
    {
        if (millisec == null) return null;


        return DateTimeOffset.FromUnixTimeMilliseconds((long)millisec).ToLocalTime().ToString(dateFormat);

    }

    public static string ConvertUTCToLocalString(DateTime dateTime, string dateFormat = "yyyy-MM-dd HH:mm:ss", CultureInfo culture = null)
    {
        if (culture == null)
        {
            return dateTime.ToLocalTime().ToString(dateFormat);
        }

        return dateTime.ToLocalTime().ToString(dateFormat, culture);
    }

    public static DateTime? ConvertUTCStringToUTCDate(string timeText, string dateFormat = "yyyy-MM-dd HH:mm:ss")
    {
        if (timeText == null || timeText.Trim() == "") return null;

        // Specify that the input time is in UTC
        return DateTime.ParseExact(timeText, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();

    }

    public static string ConvertUTCDateToUTCString(DateTime dateTime, string dateFormat = "yyyy-MM-dd HH:mm:ss", CultureInfo culture = null)
    {
        if (culture == null) { 
            return dateTime.ToString(dateFormat);
        }
        // CultureInfo.CreateSpecificCulture("en-US")

        return dateTime.ToString(dateFormat, culture);
    }

}
