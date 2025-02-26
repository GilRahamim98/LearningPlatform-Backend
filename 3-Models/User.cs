using System.ComponentModel.DataAnnotations;
namespace Talent;

public class User
{
    [Key]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Missing Name")]
    [MinLength(2, ErrorMessage = "Name should be at least 2 chars.")]
    [MaxLength(50, ErrorMessage = "Name can't exceeds 50 chars.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Missing Email")]
    [EmailAddress]
    [MaxLength(100, ErrorMessage = "Email can't exceeds 100 chars.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Missing Password")]
    [MinLength(6, ErrorMessage = "Password should be at least 6 chars.")]
    [MaxLength(250, ErrorMessage = "Password can't exceeds 250 chars.")]
    public string Password { get; set; } = null!;

}
