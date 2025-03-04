using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Talent;

public class Role
{
    [Key]
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    [InverseProperty("Role")]
    public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
}
