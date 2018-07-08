using System;

namespace Leo.Core.Dates
{
    public class LocalDateProvider : IDateProvider
    {
        public DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }
    }
}