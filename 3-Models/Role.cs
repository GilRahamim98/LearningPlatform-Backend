using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Talent;

public class Role
{
    [Key]
    public int RoleId { get; set; }

    [Required(ErrorMessage = "Missing RoleName")]
    public string RoleName { get; set; } = null!;

    [InverseProperty("Role")]
    public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
}
