using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AMI_Project.DTOs.Meters
{
    public class MeterUploadResultDto
    {
        [Required(ErrorMessage = "CSV file is required.")]
        [FromForm(Name = "file")] // 👈 allows uploading as `file`
        public IFormFile CsvFile { get; set; } = null!;
    }
}
