using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCBA.Models;

public class Payee
{
    public int PayeeID { get; set; }

    [Required, StringLength(50)]
    public string Name { get; set; }

    [Required, StringLength(50)]
    public string Address { get; set; }

    [Required, StringLength(40)]
    public string City { get; set; }

    [Required, StringLength(3, MinimumLength = 2)]
    public string State { get; set; }

    [Required, StringLength(4)]
    public string Postcode { get; set; }

    [Required, StringLength(14)]
    public string Phone { get; set; }
}
