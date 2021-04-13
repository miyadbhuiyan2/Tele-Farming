using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tele_Farming.Models;

namespace Tele_Farming.Data
{
    public class TeleFarmingContext : DbContext
    {
        public TeleFarmingContext()
        {
        }

        public TeleFarmingContext(DbContextOptions<TeleFarmingContext> options)
            : base(options)
        {

        }


        public DbSet<Farmer> Farmer { get; set; }

        public DbSet<Agent> Agent { get; set; }

        public DbSet<Post> Post { get; set; }

        public DbSet<Specialist> Specialist { get; set; }

        public DbSet<Admin> Admin { get; set; }

        public DbSet<Post_Images> Post_Images {get; set;}

        public DbSet<Post_Time> Post_Time { get; set; }

        public DbSet<Complain> Complain { get; set; }

        public DbSet<MeetingFailure> MeetingFailures { get; set; }

        public DbSet<Meeting> Meeting { get; set; }

        //public DbSet<MeetingFailure> MeetingFailure { get; set; }

        public DbSet<Payment> Payment { get; set; }

        public DbSet<Review> Review { get; set; } 

        public DbSet<Combined_Read_Write_Posts> Combined_Read_Write_Posts { get; set; }

        public DbSet<FarmerDetails> FarmerDetails { get; set; } 

        public object Configuration { get; }
    }
}
