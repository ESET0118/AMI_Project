using AMI_Project.DTOs.Meters;
using AMI_Project.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AMI_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MeterCsvController : ControllerBase
    {
        private readonly IMeterCsvService _service;

        public MeterCsvController(IMeterCsvService service)
        {
            _service = service;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] MeterUploadResultDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.UploadAndImportAsync(dto, ct);

                return Ok(new
                {
                    message = $"Imported {result.ImportedMeters.Count()} meters with {result.Warnings.Count()} warnings.",
                    importedMeters = result.ImportedMeters,
                    warnings = result.Warnings
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "An error occurred while processing the CSV file.",
                    details = ex.Message
                });
            }
        }

    }
}