using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tele_Farming.Models
{
    public class Admin
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int admin_id { get; set; }

        public String name { get; set; }

        public String contact_number { get; set; }

        public String email { get; set; }

        public String password { get; set; }

        public String profile_picture_path { get; set; }

        [NotMapped]
        [Display(Name = "Upload File")]
        public IFormFile ImageFile { get; set; }

    }
}
