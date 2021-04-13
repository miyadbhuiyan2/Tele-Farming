using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tele_Farming.Models
{
    [Keyless]
    public class Combined_Read_Write_Posts
    {

        public Post Post { get; set; }

        public List<Post> PostList { get; set; }

        public Farmer Farmer { get; set; }

        public Agent Agent { get; set; }


        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime time { get; set; }

        public Specialist Specialist { get; set; }

        public FarmerDetails FarmerDetails { get; set; }


        public Meeting Meeting { get; set; }

        public List<Meeting> MeetingList { get; set; }


        public String day { get; set; }

        public String month { get; set; }

        public String year { get; set; }


        public String hour { get; set; }

        public String minute { get; set; }

        public String am_pm { get; set; }

        //[NotMapped]
        //public Nest.ISearchResponse<Post> SearchPost { get; set; }
        public String ComplainedBy { get; set; }
        public String hasMeeting { get; set; }
        public String PostedBy { get; set; }
        public String RequestType { get; set; }
        public List<Complain> ComplainList { get; set; }
        public int ViewType { get; set; }

        public List<Farmer> FarmerList { get; set; }
        public List<Agent> AgentList { get; set; }
        public List<Specialist> SpecialistList { get; set; }

    }
}
