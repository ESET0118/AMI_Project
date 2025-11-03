using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AMI_Project.Models;

[Table("User")]
[Index("Email", Name = "UQ__User__A9D105347699B070", IsUnique = true)]
public partial class User
{
    [Key]
    public long UserId { get; set; }

    [StringLength(256)]
    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    [StringLength(200)]
    public string? DisplayName { get; set; }

    [StringLength(50)]
    public string? Phone { get; set; }

    public bool EmailConfirmed { get; set; }

    [Precision(3)]
    public DateTime CreatedAt { get; set; }

    public long? ConsumerId { get; set; }

    [ForeignKey("ConsumerId")]
    [InverseProperty("Users")]
    public virtual Consumer? Consumer { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    [ForeignKey("UserId")]
    [InverseProperty("Users")]
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
