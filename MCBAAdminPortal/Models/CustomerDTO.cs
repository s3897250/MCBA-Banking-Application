using System.ComponentModel.DataAnnotations;

namespace MCBAAdminPortal.Models
{
    public class CustomerDTO
    {
        public int CustomerID { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Address { get; set; }

        [StringLength(40)]
        public string City { get; set; }

        [StringLength(4)]
        public string PostCode { get; set; }
    }
}
