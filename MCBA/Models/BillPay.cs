using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCBA.Models;

public enum Period
{
    OnceOff = 'O',
    Monthly = 'M'
}

public class BillPay
{
    [Key]
    public int BillPayID { get; set; }

    [Required]
    public int AccountNumber { get; set; }
    public Account Account { get; set; }

    [Required]
    public int PayeeID { get; set; }
    public Payee Payee { get; set; }

    [Required]
    [Column(TypeName = "money")]
    [DataType(DataType.Currency)]
    public decimal Amount { get; set; }

    [Required]
    public DateTime ScheduleTimeUtc { get; set; }

    [Required]
    public Period Period { get; set; }
}
