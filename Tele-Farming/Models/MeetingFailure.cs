using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tele_Farming.Models
{
    public class MeetingFailure
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int meeting_failure_id { get; set; }

        public int meeting_id { get; set; }

        [ForeignKey("meeting_id")]
        public Meeting Meeting { get; set; }


        public int complain_id { get; set; }
        [ForeignKey("complain_id")]
        public Complain Complain { get; set; }

    }
}