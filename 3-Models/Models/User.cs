using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Talent;

public class User
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

  
    public string Password { get; set; } = null!;

    [Required]
    public int RoleId { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public Role? Role { get; set; }

    [InverseProperty("User")]
    public ICollection<Enrollment> Enrollments { get; } = new List<Enrollment>();

    [InverseProperty("User")]
    public ICollection<Progress> Progresses { get; } = new List<Progress>();

}
