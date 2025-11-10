using Microsoft.AspNetCore.Http;

public interface IMeterCsvService
{
    Task<MeterUploadResultDto> UploadCsvAsync(IFormFile file);
}
