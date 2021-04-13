using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tele_Farming.Models
{
    //[Keyless]
    public class Post_Time
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int id { get; set; }

        public int post_id { get; set; }

        [ForeignKey("post_id")]
        public Post Post { get; set; }

        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //[Timestamp]
        //public byte[] time{ get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateTime), "1/1/2021", "1/1/2100", ErrorMessage = "Value for {0} must be between {1:d} and {2:d}")]
        public DateTime time { get; set; }


    }
}
