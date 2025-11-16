using AMI_Project.DTOs.MeterReadings;
using AMI_Project.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AMI_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MeterReadingController : ControllerBase
    {
        private readonly IMeterReadingService _meterReadingService;
        private readonly ILogger<MeterReadingController> _logger;

        public MeterReadingController(IMeterReadingService meterReadingService, ILogger<MeterReadingController> logger)
        {
            _meterReadingService = meterReadingService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var readings = await _meterReadingService.GetAllAsync(ct);
            return Ok(readings);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id, CancellationToken ct)
        {
            var reading = await _meterReadingService.GetByIdAsync(id, ct);
            if (reading == null)
                return NotFound(new { message = $"Meter reading with ID {id} not found." });

            return Ok(reading);
        }

        // GET calendar for a meter for year/month
        [HttpGet("meter/{serialNo}/calendar")]
        public async Task<IActionResult> GetCalendarForMonth([FromRoute] string serialNo, [FromQuery] int year, [FromQuery] int month, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(serialNo)) return BadRequest("serialNo required");
            if (year <= 0 || month < 1 || month > 12) return BadRequest("Invalid year or month");

            var calendar = await _meterReadingService.GetCalendarForMonthAsync(serialNo, year, month, ct);
            return Ok(calendar);
        }

        // GET monthly summary (optional)
        [HttpGet("meter/{serialNo}/monthly")]
        public async Task<IActionResult> GetMonthly([FromRoute] string serialNo, [FromQuery] int year, [FromQuery] int month, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(serialNo)) return BadRequest("serialNo required");
            if (year <= 0 || month < 1 || month > 12) return BadRequest("Invalid year or month");

            var monthly = await _meterReadingService.GetMonthlyAsync(serialNo, year, month, ct);
            return Ok(monthly);
        }

        // Bulk create/update daily readings for a month
        [HttpPost("meter/{serialNo}/bulk")]
        public async Task<IActionResult> CreateBulkForMonth([FromRoute] string serialNo, [FromQuery] int year, [FromQuery] int month, [FromBody] BulkMeterReadingCreateDto dto, CancellationToken ct)
        {
            if (dto == null) return BadRequest("Request body required");
            if (serialNo != dto.MeterSerialNo) return BadRequest("serialNo mismatch");
            if (year != dto.Year || month != dto.Month) return BadRequest("year/month mismatch");

            var createdOrUpdated = await _meterReadingService.CreateBulkAsync(dto, ct);
            return Ok(createdOrUpdated);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MeterReadingCreateDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _meterReadingService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.MeterReadingId }, created);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] MeterReadingUpdateDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _meterReadingService.UpdateAsync(id, dto, ct);
            if (updated == null)
                return NotFound(new { message = $"Meter reading with ID {id} not found." });

            return Ok(updated);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id, CancellationToken ct)
        {
            var existing = await _meterReadingService.GetByIdAsync(id, ct);
            if (existing == null)
                return NotFound(new { message = $"Meter reading with ID {id} not found." });

            await _meterReadingService.DeleteAsync(id, ct);
            return NoContent();
        }
    }
}
