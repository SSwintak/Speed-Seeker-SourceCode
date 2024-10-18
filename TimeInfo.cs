using Sirenix.Utilities;
using System;

[Serializable]
public struct TimeInfo
{
    public int days;
    public int hours;
    public int minutes;
    public int seconds;
    public int milliseconds;

    public double TotalMinutes => ((days * 24 + hours) * 60 + minutes) + seconds / 60.0 + milliseconds / 60000.0;

    public static TimeInfo Zero { get; }

    public TimeInfo(int days, int hours, int minutes, int seconds, int milliseconds)
    {
        this.days = days;
        this.hours = hours;
        this.minutes = minutes;
        this.seconds = seconds;
        this.milliseconds = milliseconds;
    }

    public TimeInfo(TimeInfo timeInfo)
    {
        this.days = timeInfo.days;
        this.hours = timeInfo.hours;
        this.minutes = timeInfo.minutes;
        this.seconds = timeInfo.seconds;
        this.milliseconds = timeInfo.milliseconds;
    }

    public TimeInfo (TimeSpan timeSpan)
    {
        days = timeSpan.Days;
        hours = timeSpan.Hours;
        minutes = timeSpan.Minutes;
        seconds = timeSpan.Seconds;
        milliseconds = timeSpan.Milliseconds;
    }

    public static bool operator <(TimeInfo timeInfo1, TimeInfo timeInfo2)
    {
        if (timeInfo1.TotalMinutes < timeInfo2.TotalMinutes) return true;
        return false;
    }
    public static bool operator >(TimeInfo timeInfo1, TimeInfo timeInfo2)
    {
        if (timeInfo1.TotalMinutes > timeInfo2.TotalMinutes) return true;
        return false;
    }

    public override string ToString()
    {
        return $"{days}:{hours}:{minutes}:{seconds}:{milliseconds}";
    }

    /// <summary>
    /// Returns TimeInfo in the specified formay or default if null
    /// </summary>
    /// <remarks>
    /// "d" prints only days    
    /// "h" prints only hours
    /// "m" prints only minutes
    /// "s" prints only seconds
    /// "ms" prints only milliseconds
    /// "hh:mm:ss" prints hours, minutes, seconds
    /// </remarks>
    public string ToString(string format)
    {
        switch (format)
        {
            case "d":
                return $"{days}:";
            case "h":
                return $"{hours}:";
            case "m":
                return $"{minutes}:";
            case "s":
                return $"{seconds}:";
            case "ms":
                return $"{milliseconds}";
            case "hh:mm:ss":
                return $"{hours:00}:{minutes:00}:{seconds:00}";
            default:
                return ToString(); // Default format
        }
    }
}
