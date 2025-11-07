using System.ComponentModel.DataAnnotations;

namespace AMI_Project.DTOs.OrgUnits
{
    public class OrgUnitUpdateDto
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(20)]
        public string? Type { get; set; }

        public int? ParentId { get; set; }
    }
}
