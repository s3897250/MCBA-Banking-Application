using System.ComponentModel.DataAnnotations;

namespace MCBA.Models
{
    public class EditProfileViewModel
    {
        
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
