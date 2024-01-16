using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCBA.Models;

public enum TransactionType
{
    Deposit = 'D',
    Withdrawal = 'W',
    Transfer = 'T',
    ServiceCharge = 'S'
}

public class Transaction
{
    [Key]
    public int TransactionID { get; set; }

    [Required]
    public TransactionType TransactionType { get; set; }

    [Required]
    public int AccountNumber { get; set; }
    public Account Account { get; set; }

    public int? DestinationAccountNumber { get; set; }
    public Account DestinationAccount { get; set; }

    [Required]
    [Column(TypeName = "money")]
    [DataType(DataType.Currency)]
    public decimal Amount { get; set; }

    [StringLength(30)]
    public string Comment { get; set; }

    [Required]
    public DateTime TransactionTimeUtc { get; set; }
}
