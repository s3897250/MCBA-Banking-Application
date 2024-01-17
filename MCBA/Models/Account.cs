using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace MCBA.Models
{
    public class Account
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Account Number")]
        public int AccountNumber { get; set; }

        [Display(Name = "Type")]
        public string AccountType { get; set; }

        public int CustomerID { get; set; }  // FK constraint
        public virtual Customer Customer { get; set; } // nav property

        [Column(TypeName = "money")]
        [DataType(DataType.Currency)]
        public decimal Balance { get; set; }

        [InverseProperty("Account")]
        public virtual List<Transaction> Transactions { get; set; }

        [InverseProperty("Account")]
        public virtual List<BillPay> BillPays { get; set; }
    }
}
