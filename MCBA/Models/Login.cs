using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCBA.Models;

public class Login
{
    [StringLength(8)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string LoginID { get; set; }

    public int CustomerID { get; set; }
    public virtual Customer Customer { get; set; } // Nav property

    [Required, StringLength(94)]
    public string PasswordHash { get; set; }
    

}
