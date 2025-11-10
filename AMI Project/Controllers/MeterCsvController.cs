//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//[ApiController]
//[Route("api/[controller]")]
//public class MeterCsvController : ControllerBase
//{
//    private readonly IMeterCsvService _csvService;
//    public MeterCsvController(IMeterCsvService csvService)
//    {
//        _csvService = csvService;
//    }

//    [HttpPost("upload")]
//    [Consumes("multipart/form-data")]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status400BadRequest)]
//    public async Task<IActionResult> Upload([FromForm] IFormFile file)
//    {
//        if (file == null || file.Length == 0)
//            return BadRequest("CSV file is required");

//        try
//        {
//            var result = await _csvService.UploadCsvAsync(file);
//            return Ok(result);
//        }
//        catch (Exception ex)
//        {
//            return BadRequest(new { Message = ex.Message });
//        }
//    }
//}
