using System.Globalization;

namespace ZarmallStore.Application.Utils
{
    public static class DateConvertor
    {
        public static string ToShamsi(this DateTime value)
        {
            PersianCalendar persianCalendar = new PersianCalendar();
            return persianCalendar.GetYear(value) + "/" +
                   persianCalendar.GetMonth(value).ToString("00") + "/" +
                   persianCalendar.GetDayOfMonth(value).ToString("00");
        }

        public static string ToShamsi(this DateTime? value)
        {
            PersianCalendar persianCalendar = new PersianCalendar();
            return persianCalendar.GetYear((DateTime)value) + "/" +
                   persianCalendar.GetMonth((DateTime)value).ToString("00") + "/" +
                   persianCalendar.GetDayOfMonth((DateTime)value).ToString("00");
        }
    }
}
