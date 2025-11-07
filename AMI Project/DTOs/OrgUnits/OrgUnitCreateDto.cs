using System.ComponentModel.DataAnnotations;

namespace AMI_Project.DTOs.OrgUnits
{
    public class OrgUnitCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Type { get; set; } = string.Empty;

        public int? ParentId { get; set; }
    }
}
