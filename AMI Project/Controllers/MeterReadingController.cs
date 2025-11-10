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

        public MeterReadingController(IMeterReadingService meterReadingService)
        {
            _meterReadingService = meterReadingService;
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

        [HttpGet("meter/{serialNo}")]
        public async Task<IActionResult> GetByMeterSerialNo(string serialNo, CancellationToken ct)
        {
            var readings = await _meterReadingService.GetByMeterSerialNoAsync(serialNo, ct);
            if (!readings.Any())
                return NotFound(new { message = $"No readings found for meter {serialNo}." });

            return Ok(readings);
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
