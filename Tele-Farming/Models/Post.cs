using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tele_Farming.Models
{
    public class Post
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int post_id { get; set; }
 
        public int? farmer_id { get; set; }

        [ForeignKey("farmer_id")]
        public Farmer Farmer { get; set; }


        public int? agent_id { get; set; }

        [ForeignKey("agent_id")]
        public Agent Agent { get; set; }


        [MaxLength(50)]
        public String title { get; set; }

        [MaxLength(300)]
        public String description { get; set; }


        [DataType(DataType.DateTime)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime post_time { get; set; }


        public String post_status { get; set; }

        public String is_accepted { get; set; }

        public String Category { get; set; }

        //[ForeignKey("Post")] //will create the foreignkey column here
        public List<Post_Images> Post_Images { get; set; }

        //[ForeignKey("Post")] //will create the foreignkey column here
        public List<Post_Time> Post_Time { get; set; }

        public Meeting Meeting { get; set; }

        public FarmerDetails FarmerDetails { get; set; }

        
        //public Payment Payment { get; set; }
        //public MeetingFailure MeetingFailure { get; set; }

    }
}
