using AMI_Project.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AMI_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly AMIDbContext _context;

        public DashboardController(AMIDbContext context)
        {
            _context = context;
        }

        // GET: api/Dashboard/summary
        [HttpGet("summary")]
        public IActionResult GetSummary()
        {
            var powerToday = _context.MeterReadings
                .Where(r => r.ReadingDateTime.Date == DateTime.Today)
                .Sum(r => (decimal?)r.ConsumptionKwh) ?? 0;

            return Ok(new
            {
                totalMeters = _context.Meters.Count(),
                totalUsers = _context.Users.Count(),
                powerToday = powerToday,
                tariffsActive = _context.Tariffs.Count()
            });
        }

        // GET: api/Dashboard/dailyUsage
        [HttpGet("dailyUsage")]
        public IActionResult GetDailyUsage()
        {
            var labels = Enumerable.Range(0, 7)
                .Select(d => DateTime.Today.AddDays(-6 + d).DayOfWeek.ToString().Substring(0, 3))
                .ToArray();

            var datasets = _context.Meters
                .Select(m => new
                {
                    label = m.MeterSerialNo,
                    data = _context.MeterReadings
                        .Where(r => r.MeterSerialNo == m.MeterSerialNo &&
                                    r.ReadingDateTime >= DateTime.Today.AddDays(-6))
                        .OrderBy(r => r.ReadingDateTime)
                        .Select(r => r.ConsumptionKwh)
                        .ToArray(),
                    color = "rgba(0,123,255,1)",
                    bgColor = "rgba(0,123,255,0.2)"
                })
                .Take(5)
                .ToArray();

            return Ok(new { labels, datasets });
        }

        // GET: api/Dashboard/topMeters
        [HttpGet("topMeters")]
        public IActionResult GetTopMeters()
        {
            var top = _context.Meters
                .Select(m => new
                {
                    label = m.MeterSerialNo,
                    value = _context.MeterReadings
                        .Where(r => r.MeterSerialNo == m.MeterSerialNo)
                        .Sum(r => (decimal?)r.ConsumptionKwh) ?? 0
                })
                .OrderByDescending(x => x.value)
                .Take(5)
                .ToArray();

            return Ok(new
            {
                labels = top.Select(x => x.label).ToArray(),
                data = top.Select(x => x.value).ToArray()
            });
        }

        // GET: api/Dashboard/recentReadings
        [HttpGet("recentReadings")]
        public IActionResult GetRecentReadings()
        {
            var readings = _context.MeterReadings
                .Include(r => r.MeterSerialNoNavigation)
                .OrderByDescending(r => r.ReadingDateTime)
                .Take(10)
                .Select(r => new
                {
                    meterSerial = r.MeterSerialNo,
                    date = r.ReadingDateTime.ToString("yyyy-MM-dd HH:mm"),
                    consumptionKwh = r.ConsumptionKwh,
                    voltage = r.Voltage,
                    ampere = r.Ampere,
                    powerFactor = r.PowerFactor
                })
                .ToList();

            return Ok(readings);
        }
    }
}
