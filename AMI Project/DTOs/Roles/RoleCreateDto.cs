using System.ComponentModel.DataAnnotations;

public class RoleCreateDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
}