using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Talent;

public class User
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Missing Password")]
    [MaxLength(250, ErrorMessage = "Password can't exceeds 250 chars.")]
    public string Password { get; set; } = null!;

    [Required]
    public int RoleId { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public Role? Role { get; set; }

}
