using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tele_Farming.Models
{
    public class Meeting
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int meeting_id { get; set; }

        public int specialist_id { get; set; }

        [ForeignKey("specialist_id")]
        public Specialist Specialist { get; set; }

        public int post_id { get; set; }

        [ForeignKey("post_id")]
        public Post Post { get; set; }

        [Required(ErrorMessage="Zoom meeting link is required")]
        [RegularExpression(@"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)", ErrorMessage="Invalid Link")]
        public String meeting_link { get; set; }

        
        //[Range(10, 100, ErrorMessage="Max input range from 10-100" )]
        [StringLength(60, MinimumLength = 3)]
        public String short_message { get; set; }

        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //[Timestamp]
        //public byte[] meeting_time { get; set; }

        [Required(ErrorMessage = "Please select one time")]
        [DataType(DataType.DateTime, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime meeting_time { get; set; }

        public String meeting_status { get; set; }

        public Review Review { get; set; }

        public Payment Payment { get; set; }

        public MeetingFailure MeetingFailure { get; set; }

    }
}
