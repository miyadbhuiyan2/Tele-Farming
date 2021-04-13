using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tele_Farming.Data;
using Tele_Farming.Models;

namespace Tele_Farming.Controllers
{
    public class AdminsController : Controller
    {
        private readonly TeleFarmingContext _context;

        public AdminsController(TeleFarmingContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> ApproveReject(int? id, int? result)
        {
            if (id == null)
            {
                return NotFound();
            }

            Specialist specialist = await _context.Specialist.FindAsync(id);
            if (specialist != null)
            {
                specialist.is_approved = (int)result;
                _context.Update(specialist);
                await _context.SaveChangesAsync();
                return RedirectToAction("AdminJoinRequests", "Admins");
            }
            else
                return NotFound();

            //return View(specialist);
        }


        public async Task<IActionResult> postStatusUpdate(int? id, int? cId, String? result)
        {
            if (id == null)
            {
                return NotFound();
            }
            Post post = await _context.Post.FindAsync(id);
            if (post != null)
            {
                post.post_status = result;
                _context.Update(post);
                await _context.SaveChangesAsync();

                Complain complain = await _context.Complain
                    .Where(x => x.complain_id == cId)
                    .Include(x => x.MeetingFailure)
                    .FirstOrDefaultAsync();

                Meeting meeting = await _context.Meeting.FindAsync(complain.MeetingFailure.meeting_id);

                complain.is_resolved = 1;
                _context.Update(complain);
                await _context.SaveChangesAsync();

                _context.Remove(complain);
                await _context.SaveChangesAsync();


                if( result.Equals("Finished"))
                {
                    meeting.meeting_status = "Finished";
                    _context.Update(meeting);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.Remove(meeting);
                    await _context.SaveChangesAsync();
                }   
                

                return RedirectToAction("AdminComplains", "Admins");
            }
            else
                return NotFound();
        }
        public async Task<IActionResult> AdminComplainView(int? id)
        {
            var complain = await _context.Complain
                .Where(p => p.complain_id == id)
                .Include(p => p.MeetingFailure)
                .ThenInclude(p => p.Meeting)
                .ThenInclude(p => p.Post)
                .ThenInclude(p => p.Farmer)
                .Include(p => p.MeetingFailure)
                .ThenInclude(p => p.Meeting)
                .ThenInclude(p => p.Post)
                .ThenInclude(p => p.Agent)
                .Include(p => p.MeetingFailure)
                .ThenInclude(p => p.Meeting)
                .ThenInclude(p => p.Post)
                .ThenInclude(p => p.FarmerDetails)
                .Include(p => p.MeetingFailure)
                .ThenInclude(p => p.Meeting)
                .ThenInclude(p => p.Specialist)
                .FirstOrDefaultAsync();

            return View(complain);
        }
        public async Task<IActionResult> AdminRequestView(int? id)
        {
            Specialist c = new Specialist();
            c = await _context.Specialist.FirstOrDefaultAsync(m => m.specialist_id == id);

            return View(c);
        }
        public async Task<IActionResult> AdminJoinRequests(String? type)
        {
            Combined_Read_Write_Posts c = new Combined_Read_Write_Posts();
            if (type == "ALL" || type == null)
            {
                c.RequestType = "ALL";
                c.SpecialistList = await _context.Specialist.Where(m => m.is_approved == 0).ToListAsync();
            }
            else
            {
                c.RequestType = type;
                c.SpecialistList = await _context.Specialist.Where(m => m.is_approved == 0 && m.Category == type).ToListAsync();
            }

            // c.SpecialistList = specialists;
            return View(c);
        }
        public async Task<IActionResult> AdminComplains()
        {
            Combined_Read_Write_Posts c = new Combined_Read_Write_Posts();
            c.ComplainList = await _context.Complain.Where(p => p.is_resolved == 0)
                .Include(p => p.MeetingFailure)
                .ThenInclude(p => p.Meeting)
                .ThenInclude(p => p.Post)
                .ThenInclude(p => p.Farmer)
                .Include(p => p.MeetingFailure)
                .ThenInclude(p => p.Meeting)
                .ThenInclude(p => p.Post)
                .ThenInclude(p => p.Agent)
                .Include(p => p.MeetingFailure)
                .ThenInclude(p => p.Meeting)
                .ThenInclude(p => p.Post)
                .ThenInclude(p => p.FarmerDetails)
                .Include(p => p.MeetingFailure)
                .ThenInclude(p => p.Meeting)
                .ThenInclude(p => p.Specialist)
                .ToListAsync();

            return View(c);
        }

        public async Task<IActionResult> AdminHomePostView(int? id)
        {
            Combined_Read_Write_Posts c = new Combined_Read_Write_Posts();
            c.Post = await _context.Post
               .Where(p => p.post_id == id)
               .FirstOrDefaultAsync();
            if (c.Post.agent_id != null)
            {
                c.PostedBy = "Agent";
                c.Post = await _context.Post
                    .Where(p => p.post_id == id)
                    .Include( p => p.Agent )
                    .Include(p => p.FarmerDetails)
                    .Include(p => p.Post_Time)
                    .Include(p => p.Post_Images)
                    .Include(p => p.Meeting)
                    .ThenInclude(p => p.Specialist)
                    .FirstOrDefaultAsync();
            }
            else
            {
                c.PostedBy = "Farmer";
                c.Post = await _context.Post
                 .Where(p => p.post_id == id)
                 .Include(p => p.Farmer)
                 .Include(p => p.Post_Time)
                 .Include(p => p.Post_Images)
                 .Include(p => p.Meeting)
                 .ThenInclude(p => p.Specialist)
                 .FirstOrDefaultAsync();
            }
            if (c.Post.Meeting != null)
            {
                c.hasMeeting = "YES";
            }
            else
            {
                c.hasMeeting = "NO";
            }

            return View(c);
        }

        public async Task<IActionResult> AdminHomeUserView(int? id, String userType)
        {
            Combined_Read_Write_Posts c = new Combined_Read_Write_Posts();
            if (userType == "Specialist")
            {
                c.ViewType = 1;
                c.Specialist = await _context.Specialist.FirstOrDefaultAsync(m => m.specialist_id == id);


            }
            else if (userType == "Farmer")
            {
                c.ViewType = 2;
                c.Farmer = await _context.Farmer.FirstOrDefaultAsync(m => m.farmer_id == id);
            }
            else
            {
                c.ViewType = 3;
                c.Agent = await _context.Agent.FirstOrDefaultAsync(m => m.agent_id == id);
            }

            return View(c);
        }


        public async Task<IActionResult> AdminHomeDetailsPage(int? id, String? SearchString)
        {
            // ViewData["specialist_id"] = SPECIALIST_ID;
            if (id == 1)
            {
                if (!String.IsNullOrEmpty(SearchString))
                {
                    var specialists = await _context.Specialist.Where(m => m.is_approved == 1 && m.name.Contains(SearchString)).ToListAsync();
                    Combined_Read_Write_Posts c = new Combined_Read_Write_Posts();
                    c.SpecialistList = specialists;
                    c.ViewType = 1;
                    return View(c);
                }
                else
                {
                    var specialists = await _context.Specialist.Where(m => m.is_approved == 1).ToListAsync();
                    Combined_Read_Write_Posts c = new Combined_Read_Write_Posts();
                    c.SpecialistList = specialists;
                    c.ViewType = 1;
                    return View(c);
                }


            }
            else if (id == 2)
            {

                if (!String.IsNullOrEmpty(SearchString))
                {
                    var farmers = await _context.Farmer.Where(m => m.name.Contains(SearchString)).ToListAsync();
                    Combined_Read_Write_Posts c = new Combined_Read_Write_Posts();
                    c.FarmerList = farmers;
                    c.ViewType = 2;
                    return View(c);
                }
                else
                {
                    var farmers = await _context.Farmer.ToListAsync();
                    Combined_Read_Write_Posts c = new Combined_Read_Write_Posts();
                    c.FarmerList = farmers;
                    c.ViewType = 2;
                    return View(c);
                }
            }
            else if (id == 3)
            {
                if (!String.IsNullOrEmpty(SearchString))
                {
                    var agents = await _context.Agent.Where(m => m.name.Contains(SearchString)).ToListAsync();
                    Combined_Read_Write_Posts c = new Combined_Read_Write_Posts();
                    c.AgentList = agents;
                    c.ViewType = 3;
                    return View(c);
                }
                else
                {
                    var agents = await _context.Agent.ToListAsync();
                    Combined_Read_Write_Posts c = new Combined_Read_Write_Posts();
                    c.AgentList = agents;
                    c.ViewType = 3;
                    return View(c);
                }

            }
            else
            {

                if (!String.IsNullOrEmpty(SearchString))
                {
                    var posts = await _context.Post.Where(m => m.title.Contains(SearchString)).ToListAsync();
                    Combined_Read_Write_Posts c = new Combined_Read_Write_Posts();
                    c.PostList = posts;
                    c.ViewType = 4;
                    return View(c);
                }
                else
                {
                    var posts = await _context.Post.ToListAsync();
                    Combined_Read_Write_Posts c = new Combined_Read_Write_Posts();
                    c.PostList = posts;
                    c.ViewType = 4;
                    return View(c);
                }

            }
        }


        public async Task<IActionResult> AdminHome()
        {
            var posts = await _context.Post.ToListAsync();
            var farmers = await _context.Farmer.ToListAsync();
            var agents = await _context.Agent.ToListAsync();
            var specialists = await _context.Specialist.Where(m => m.is_approved == 1).ToListAsync();

            Combined_Read_Write_Posts c = new Combined_Read_Write_Posts();
            c.PostList = posts;
            c.FarmerList = farmers;
            c.AgentList = agents;
            c.SpecialistList = specialists;

            return View(c);
        }
        // GET: Admins/Create


        // GET: Admins
        public async Task<IActionResult> Index()
        {
            return View(await _context.Admin.ToListAsync());
        }

        // GET: Admins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin
                .FirstOrDefaultAsync(m => m.admin_id == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // GET: Admins/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("admin_id,name,contact_number,email,password,profile_picture_path")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(admin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }

        // GET: Admins/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            return View(admin);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("admin_id,name,contact_number,email,password,profile_picture_path")] Admin admin)
        {
            if (id != admin.admin_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.admin_id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }

        // GET: Admins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin
                .FirstOrDefaultAsync(m => m.admin_id == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var admin = await _context.Admin.FindAsync(id);
            _context.Admin.Remove(admin);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminExists(int id)
        {
            return _context.Admin.Any(e => e.admin_id == id);
        }
    }
}
