using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tele_Farming.Models
{
    public class Complain
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int complain_id { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime time { get; set; }

        public int is_resolved { get; set; }

        public String category { get; set; }

        public MeetingFailure MeetingFailure { get; set; }

    }
}
