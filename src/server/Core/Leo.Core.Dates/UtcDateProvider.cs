using System;

namespace Leo.Core.Dates
{
    public class UtcDateProvider : IDateProvider
    {
        public DateTime GetCurrentDateTime()
        {
            return DateTime.UtcNow;
        }
    }
}