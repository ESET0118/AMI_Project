using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AMI_Project.Models
{
    [Table("Role")]
    [Index("Name", Name = "UQ_Role_Name", IsUnique = true)]
    public partial class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = null!;

        // ✅ Correct many-to-many — remove [ForeignKey]
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
