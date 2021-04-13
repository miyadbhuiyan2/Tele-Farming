using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace Tele_Farming.Models
{
    public class Farmer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int farmer_id { get; set; }

        [Required(ErrorMessage = "name is required.")]
        [RegularExpression(@"(^[a-zA-Z]+(([',. -][a-zA-Z ])?[a-zA-Z]*)*$)", ErrorMessage = "Invalid Name")]
        public String name { get; set; }

        [RegularExpression(@"^(?:\+?88)?01[13-9]\d{8}$", ErrorMessage="Invalid Contact Number" )]
        [Required(ErrorMessage = "contact number is required.")]
        public String contact_number { get; set; }

        [RegularExpression(@"^(?:\+?88)?01[13-9]\d{8}$", ErrorMessage = "Invalid Bkash Number")]
        public String bkash_number { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Required(ErrorMessage = "email is required.")]
        public String email { get; set; }

        [Required(ErrorMessage = "password is required.")]
        public String password { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Confirm Password required")]
        [CompareAttribute("password", ErrorMessage = "Password doesn't match.")]
        public string confirm_password { get; set; }

        public String profile_picture_path { get; set; }

        [NotMapped]
        [Display(Name = "Upload File")]
        public IFormFile ImageFile { get; set; }

        public List<Post> Post { get; set; }

        public String PasswordResetCode { get; set; }

        //public String emailVerified { get; set; }

    }
}
