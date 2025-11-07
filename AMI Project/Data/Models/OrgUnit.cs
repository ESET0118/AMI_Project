using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AMI_Project.Models;

[Table("OrgUnit")]
public partial class OrgUnit
{
    [Key]
    public int OrgUnitId { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Type { get; set; } = null!;

    [StringLength(100)]
    public string Name { get; set; } = null!;

    public int? ParentId { get; set; }

    [InverseProperty("OrgUnit")]
    public virtual ICollection<Consumer> Consumers { get; set; } = new List<Consumer>();

    [InverseProperty("Parent")]
    public virtual ICollection<OrgUnit> InverseParent { get; set; } = new List<OrgUnit>();

    [ForeignKey("ParentId")]
    [InverseProperty("InverseParent")]
    public virtual OrgUnit? Parent { get; set; }
}
