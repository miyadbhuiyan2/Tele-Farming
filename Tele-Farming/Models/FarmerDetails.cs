using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tele_Farming.Models
{
    public class FarmerDetails
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int farmer_details_id { get; set; }

        public int? post_id { get; set; }

        [ForeignKey("post_id")]
        public Post Post { get; set; }

        public String name { get; set; }

        public String contact_number { get; set; }

    }
}
