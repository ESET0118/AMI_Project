namespace AMI_Project.DTOs.OrgUnits
{
    public class OrgUnitReadDto
    {
        public int OrgUnitId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public string? ParentName { get; set; }
    }
}
