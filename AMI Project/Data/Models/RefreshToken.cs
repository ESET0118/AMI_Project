using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AMI_Project.Models;

[Table("RefreshToken")]
public partial class RefreshToken
{
    [Key]
    public long RefreshTokenId { get; set; }

    public long UserId { get; set; }

    [StringLength(500)]
    public string Token { get; set; } = null!;

    [Precision(3)]
    public DateTime ExpiresAt { get; set; }

    [Precision(3)]
    public DateTime CreatedAt { get; set; }

    [StringLength(100)]
    public string? CreatedByIp { get; set; }

    [Precision(3)]
    public DateTime? RevokedAt { get; set; }

    [StringLength(500)]
    public string? ReplacedByToken { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("RefreshTokens")]
    public virtual User User { get; set; } = null!;
}
