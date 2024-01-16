using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCBA.Models;

public class Login
{
    [Key, StringLength(8)]
    public string LoginID { get; set; }

    public int CustomerID { get; set; }

    [Required, StringLength(94)]
    public string PasswordHash { get; set; }

    // Navigation property
    public virtual Customer Customer { get; set; }
}
