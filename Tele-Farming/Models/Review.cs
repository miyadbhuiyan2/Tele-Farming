using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tele_Farming.Models
{
    public class Review
    {


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int review_id { get; set; }

        public int meeting_id { get; set; }

        [ForeignKey("meeting_id")]

        public Meeting Meeting { get; set; }

        public double rating { get; set; }


    }
}
