using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace MCBA.Models
{
    public enum AccountType
    {
        Checking = 'C',
        Saving = 'S'
    }

    public class Account
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Account Number")]
        public int AccountNumber { get; set; }

        [Display(Name = "Type")]
        public AccountType AccountType { get; set; }

        public int CustomerID { get; set; }

        [Column(TypeName = "money")]
        [DataType(DataType.Currency)]
        public decimal Balance { get; set; }

        // Navigation properties
        public virtual Customer Customer { get; set; }

        // Transactions where this account is the source
        public virtual List<Transaction> Transactions { get; set; }

        // Transactions where this account is the destination
        [InverseProperty("DestinationAccount")]
        public virtual List<Transaction> DestinationTransactions { get; set; }

        // BillPays associated with this account
        public virtual List<BillPay> BillPays { get; set; }
    }
}
