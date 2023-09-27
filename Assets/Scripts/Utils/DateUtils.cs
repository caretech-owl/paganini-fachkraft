using System;
using System.Globalization;


public static class DateUtils 
{
    public static long? ConvertStringToTsMilliseconds(string timeText, string dateFormat= "yyyy-MM-dd HH:mm:ss")
    {
        if (timeText == null || timeText.Trim() == "") return null;

        var ts = DateTime.ParseExact(timeText, dateFormat, CultureInfo.InvariantCulture);
        return new DateTimeOffset(ts).ToUnixTimeMilliseconds();
    }

    public static string ConvertMillisecondsToString(long? millisec, string dateFormat = "yyyy-MM-dd HH:mm:ss")
    {
        if (millisec == null) return null;
        return DateTimeOffset.FromUnixTimeMilliseconds((long)millisec).UtcDateTime.ToString(dateFormat);
    }

    

}
