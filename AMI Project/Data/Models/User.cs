using System;
using System.Collections.Generic;

namespace AMI_Project.Data.Models;

public partial class User
{
    public long UserId { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? DisplayName { get; set; }

    public string? Phone { get; set; }

    public bool? EmailConfirmed { get; set; }

    public DateTime? CreatedAt { get; set; }

    public long? ConsumerId { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
