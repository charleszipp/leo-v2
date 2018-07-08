using System;

namespace Leo.Core.Dates
{
    public interface IDateProvider
    {
        DateTime GetCurrentDateTime();
    }
}