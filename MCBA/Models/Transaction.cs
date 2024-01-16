using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCBA.Models;

public class Transaction
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int TransactionID { get; set; }

    [Required]
    public char TransactionType { get; set; }

    [ForeignKey("Account")]
    public int AccountNumber { get; set; }
    public Account Account { get; set; }

    [ForeignKey("DestinationAccount")]
    public int? DestinationAccountNumber { get; set; }
    public Account DestinationAccount { get; set; }

    [Column(TypeName = "money")]
    public decimal Amount { get; set; }

    [StringLength(30)]
    public string Comment { get; set; }

    public DateTime TransactionTimeUtc { get; set; }
}
