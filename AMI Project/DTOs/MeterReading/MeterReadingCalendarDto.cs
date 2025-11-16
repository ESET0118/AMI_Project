using System;
using System.Collections.Generic;

namespace AMI_Project.DTOs.MeterReadings
{
    public class MeterReadingCalendarDto
    {
        public string MeterSerialNo { get; set; } = null!;
        public int Year { get; set; }
        public int Month { get; set; }
        public IEnumerable<DailyMeterReadingDto> Days { get; set; } = new List<DailyMeterReadingDto>();
        public decimal MonthlyTotalKwh { get; set; }
        public bool HasMonthlyRecord { get; set; }
    }
}
