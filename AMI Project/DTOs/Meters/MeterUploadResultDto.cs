namespace AMI_Project.DTOs.Meter
{
    public class MeterUploadResultDto
    {
        public int InsertedCount { get; set; }
        public int UpdatedCount { get; set; }
        public int FailedCount { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
