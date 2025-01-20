using System;

namespace VED.Utilities
{
    public class TimeFormatter
    {
        const decimal SEC = 1;
        const decimal MIL = SEC / 1000;
        const decimal MIN = SEC * 60;
        const decimal HOU = MIN * 60;
        const decimal DAY = HOU * 24;
    
        public enum Format
    {
        MMM,
        SS,
        SSMMM,
        MM,
        MMSS,
        MMSSMMM,
        HH,
        HHMM,
        HHMMSS,
        HHMMSSMMM,
        DD,
        DDHH,
        DDHHMM,
        DDHHMMSS,
        DDHHMMSSMMM,
    }
    
        private static decimal Modulus(decimal u, decimal d)
    {
        if (d == 0) return u;
        return u % d;
    }
    
        private static string FormatMilliseconds(decimal time)
        {
            return (time / MIL).ToString("000");
        }
    
        private static string FormatSeconds(Format format, decimal time, string spacer = ".")
        {
            decimal seconds = Math.Floor(time / SEC);
            decimal milliseconds = Math.Floor(Modulus(time, seconds * SEC) / MIL);
    
            switch (format)
            {
                case Format.SS    : return seconds.ToString("00");
                case Format.SSMMM : return seconds.ToString("00") + spacer + milliseconds.ToString("000");
                default: return seconds.ToString("00");
            }
        }
    
        private static string FormatMinutes(Format format, decimal time, string spacer = ".")
        {
            decimal minutes      = Math.Floor(time / MIN);
            decimal seconds      = Math.Floor(Modulus(time, minutes * MIN) / SEC);
            decimal milliseconds = Math.Floor(Modulus(Modulus(time, minutes * MIN), seconds * SEC) / MIL);
    
            switch (format)
            {
                case Format.MM      : return minutes.ToString("00");
                case Format.MMSS    : return minutes.ToString("00") + spacer + seconds.ToString("00");
                case Format.MMSSMMM : return minutes.ToString("00") + spacer + seconds.ToString("00") + spacer + milliseconds.ToString("000");
                default: return minutes.ToString("00");
            }
        }
    
        private static string FormatHours(Format format, decimal time, string spacer = ".")
        {
            decimal hours        = Math.Floor(time / HOU);
            decimal minutes      = Math.Floor(Modulus(time, hours * HOU) / MIN);
            decimal seconds      = Math.Floor(Modulus(Modulus(time, hours * HOU), minutes * MIN) / SEC);
            decimal milliseconds = Math.Floor(Modulus(Modulus(Modulus(time, hours * HOU), minutes * MIN), seconds * SEC) / MIL);
    
            switch (format)
            {
                case Format.HH        : return hours.ToString("00");
                case Format.HHMM      : return hours.ToString("00") + spacer + minutes.ToString("00");
                case Format.HHMMSS    : return hours.ToString("00") + spacer + minutes.ToString("00") + spacer + seconds.ToString("00");
                case Format.HHMMSSMMM : return hours.ToString("00") + spacer + minutes.ToString("00") + spacer + seconds.ToString("00") + spacer + milliseconds.ToString("000");
                default: return hours.ToString("00");
            }
        }
    
        private static string FormatDays(Format format, decimal time, string spacer = ".")
        {
            decimal days         = Math.Floor(time / DAY);
            decimal hours        = Math.Floor(Modulus(time, days * DAY) / HOU);
            decimal minutes      = Math.Floor(Modulus(Modulus(time, days * DAY), hours * HOU) / MIN);
            decimal seconds      = Math.Floor(Modulus(Modulus(Modulus(time, days * DAY), hours * HOU), minutes * MIN) / SEC);
            decimal milliseconds = Math.Floor(Modulus(Modulus(Modulus(Modulus(time, days * DAY), hours * HOU), minutes * MIN), seconds * SEC) / MIL);
    
            switch (format)
            {
                case Format.DD          : return days.ToString("00");
                case Format.DDHH        : return days.ToString("00") + spacer + hours.ToString("00");
                case Format.DDHHMM      : return days.ToString("00") + spacer + hours.ToString("00") + spacer + minutes.ToString("00");
                case Format.DDHHMMSS    : return days.ToString("00") + spacer + hours.ToString("00") + spacer + minutes.ToString("00") + spacer + seconds.ToString("00");
                case Format.DDHHMMSSMMM : return days.ToString("00") + spacer + hours.ToString("00") + spacer + minutes.ToString("00") + spacer + seconds.ToString("00") + spacer + milliseconds.ToString("000");
                default: return days.ToString("00");
            }
        }


        public static string FormatTime(Format format, float seconds, string spacer = ".") => FormatTime(format, (decimal)seconds, spacer);

        public static string FormatTime(Format format, decimal seconds, string spacer = ".")
        {
            switch (format)
            {
                case Format.MMM         : return FormatMilliseconds(seconds);
                case Format.SS          :
                case Format.SSMMM       : return FormatSeconds(format, seconds, spacer);
                case Format.MM          :
                case Format.MMSS        :
                case Format.MMSSMMM     : return FormatMinutes(format, seconds, spacer);
                case Format.HH          :
                case Format.HHMM        :
                case Format.HHMMSS      :
                case Format.HHMMSSMMM   : return FormatHours(format, seconds, spacer);
                case Format.DD          :
                case Format.DDHH        :
                case Format.DDHHMM      :
                case Format.DDHHMMSS    :
                case Format.DDHHMMSSMMM : return FormatDays(format, seconds, spacer);
                default : return FormatSeconds(Format.SS, seconds, spacer);
            }
        }
    }
}