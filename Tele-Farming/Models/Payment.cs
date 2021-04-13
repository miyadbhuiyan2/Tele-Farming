using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tele_Farming.Models
{
    public class Payment
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int payment_id { get; set; }

        //public int post_id { get; set; }

        //[ForeignKey("post_id")]
        //public Post Post { get; set; }

        public int meeting_id { get; set; }

        [ForeignKey("meeting_id")]
        public Meeting Meeting { get; set; }

        public double amount { get; set; }

        public int payment_status { get; set; }

    }
}
