using AMI_Project.DTOs.Meter;
using AMI_Project.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AMI_Project.Controllers
{
    [ApiController]
    [Route("api/meters")]
    public class MeterController : ControllerBase
    {
        private readonly IMeterService _meterService;

        public MeterController(IMeterService meterService)
        {
            _meterService = meterService;
        }

        // GET: /api/meters/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAllMeters(CancellationToken ct)
        {
            var result = await _meterService.GetAllMetersAsync(ct);
            return Ok(result);
        }

        // GET: /api/meters
        [HttpGet]
        public async Task<IActionResult> GetMeters([FromQuery] MeterFilterDto filter, CancellationToken ct)
        {
            var result = await _meterService.GetMetersAsync(filter, ct);
            return Ok(result);
        }

        // GET: /api/meters/{serialNo}
        [HttpGet("{serialNo}")]
        public async Task<IActionResult> GetMeterBySerial(string serialNo, CancellationToken ct)
        {
            var meter = await _meterService.GetBySerialAsync(serialNo, ct);
            if (meter == null)
                return NotFound($"Meter with Serial No {serialNo} not found.");
            return Ok(meter);
        }

        // POST: /api/meters
        [HttpPost]
        public async Task<IActionResult> CreateMeter([FromBody] MeterCreateDto dto, CancellationToken ct)
        {
            var created = await _meterService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetMeterBySerial), new { serialNo = created.MeterSerialNo }, created);
        }

        // PUT: /api/meters/{serialNo}
        [HttpPut("{serialNo}")]
        public async Task<IActionResult> UpdateMeter(string serialNo, [FromBody] MeterUpdateDto dto, CancellationToken ct)
        {
            var updated = await _meterService.UpdateAsync(serialNo, dto, ct);
            return Ok(updated);
        }

        // DELETE: /api/meters/{serialNo}
        [HttpDelete("{serialNo}")]
        public async Task<IActionResult> DeleteMeter(string serialNo, CancellationToken ct)
        {
            await _meterService.DeleteAsync(serialNo, ct);
            return NoContent();
        }


        
    }
}
