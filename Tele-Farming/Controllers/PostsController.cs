using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tele_Farming.Data;
using Tele_Farming.Models;

namespace Tele_Farming.Controllers
{
    public class PostsController : Controller
    {
        private readonly TeleFarmingContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public PostsController(TeleFarmingContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }
      
        public async Task<IActionResult> ViewPostPending( int? id )
        {
            //var posts = _context.Post.Where(p => p.post_id == id).FirstOrDefault();
            //posts.Post_Time = _context.Post_Time.Where(p => p.post_id == posts.post_id).ToList();
            //posts.Meeting = _context.Meeting.Include(m => m.Specialist).Where(p => p.post_id == posts.post_id).FirstOrDefault();

            var posts =  await _context.Post
                .Where(p => p.post_id == id)
                .Include(p => p.Post_Time)
                .Include(p => p.Post_Images)
                .Include(p => p.Meeting)
                .ThenInclude(p => p.Specialist)
                .FirstOrDefaultAsync();

            //posts.Meeting.Specialist = _context.Specialist.Where(p => p.specialist_id == posts.Meeting.specialist_id).FirstOrDefault();
            //using (var context = new TeleFarmingContext())
            //{
            //    var posts = context.Post
            //       .Where(post => post.post_id == id)
            //       .Include(post_time => post_time.Post_Time)
            //       .Include(meeting => meeting.Meeting)
            //       .ThenInclude(specialist => specialist.Specialist)
            //       .ToList();
            //    return View(posts);
            //}
            if (posts.Meeting != null)
            {
                return RedirectToAction("ViewPost", new { id = posts.farmer_id });
            }

            return View(posts);
        }

        public async Task<IActionResult> ViewPost( int? id )
        {
            //var posts = _context.Post.Where(p => p.post_id == id).FirstOrDefault();
            //posts.Post_Time = _context.Post_Time.Where(p => p.post_id == posts.post_id).ToList();
            //posts.Meeting = _context.Meeting.Include(m => m.Specialist).Where(p => p.post_id == posts.post_id).FirstOrDefault();

            //posts.Meeting.Specialist = _context.Specialist.Where(p => p.specialist_id == posts.Meeting.specialist_id).FirstOrDefault();
            //using (var context = new TeleFarmingContext())
            //{
            //    var posts = context.Post
            //       .Where(post => post.post_id == id)
            //       .Include(post_time => post_time.Post_Time)
            //       .Include(meeting => meeting.Meeting)
            //       .ThenInclude(specialist => specialist.Specialist)
            //       .ToList();
            //    return View(posts);
            //}

           var posts =  await _context.Post
                .Where(p => p.post_id == id)
                .Include(p => p.Post_Time)
                .Include(p => p.Post_Images)
                .Include(p => p.Meeting)
                .ThenInclude(p => p.Specialist)
                .FirstOrDefaultAsync();

            if( posts.Meeting != null)
            {
                return View(posts);
            }

            return RedirectToAction("ViewPostPending", new { id = posts.farmer_id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post( Combined_Read_Write_Posts c )
        {

            string wwwRootPath = _hostEnvironment.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(c.Post.Post_Images[0].ImageFile.FileName);
            string extension = Path.GetExtension(c.Post.Post_Images[0].ImageFile.FileName);
            fileName = fileName + "post" + c.Post.farmer_id + extension;
            string path = Path.Combine(wwwRootPath + "/Image/", fileName);
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await c.Post.Post_Images[0].ImageFile.CopyToAsync(fileStream);
            }

            c.Post.Post_Images[0].image_path = fileName;
            _context.Add(c.Post);

            var posts = _context.Post.Include("Farmer").Where(p => p.farmer_id == c.Post.farmer_id ).ToList();

            foreach (var x in c.Post.Post_Time)
            {
                x.post_id = posts[posts.Count-1].post_id;
            }

            foreach (var x in c.Post.Post_Images)
            {
                x.post_id = posts[posts.Count - 1].post_id;
            }


            List<Post_Time> post_time_list = new List<Post_Time>();
            c.Post.Post_Time = post_time_list;

            Post_Time post_time = new Post_Time();
            c.Post.Post_Time.Add(post_time);

            c.Post.Post_Time[0].time = c.time;
            foreach (var x in c.Post.Post_Time)
            {
                _context.Add(x);
            }

            //foreach (var x in c.Post.Post_Images)
            //{
            //    _context.Add(x);
            //}

            await _context.SaveChangesAsync();
            return RedirectToAction( "Post", new { id = c.Post.farmer_id } );
        }


        public ActionResult Post( int? id )
        {
            //var farmer = _context.Farmer.Include(m => m.Post).Single(m => m.farmer_id == id);
            //IList<Post> post_list = _context.Post.Include(c => c.Farmer).Where(c => c.farmer_id == id).ToList();
            var posts = _context.Post.Include("Farmer").Where(p => p.farmer_id == id).ToList();
            posts.Reverse();
            var farmers = _context.Farmer.Include("Post").Where(p => p.farmer_id == id).FirstOrDefault();

            foreach (var x in posts)
            {
                Farmer f = new Farmer();
                f.name = farmers.name;
                f.farmer_id = farmers.farmer_id;

                x.Farmer = f;
                x.Farmer.farmer_id = farmers.farmer_id;
            }

            Combined_Read_Write_Posts c = new Combined_Read_Write_Posts();
            c.PostList = posts;

            Post pobject = new Post();
            c.Post = pobject;

            List<Post_Time> post_time_list = new List<Post_Time>();
            for (var i = 0; i < 3; i++)
            {
                Post_Time p = new Post_Time();
                post_time_list.Add(p);
            }
            c.Post.Post_Time = post_time_list;


            List<Post_Images> post_image_list = new List<Post_Images>();
            for (var i = 0; i < 5; i++)
            {
                Post_Images p = new Post_Images();
                post_image_list.Add(p);
            }
            c.Post.Post_Images = post_image_list;

            //ICollection<Post> farmer_posts = farmer.Post;


            return View(c);
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            return View(await _context.Post.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .FirstOrDefaultAsync(m => m.post_id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("post_id,farmer_id,agent_id,title,description,post_status,is_accepted")] Post post )
        {
            //if (ModelState.IsValid)
            //{
                post.post_time = DateTime.Now;
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction( "Post", new { id = post.farmer_id } );
            //}
            //return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("post_id,farmer_id,agent_id,title,description,post_time,post_status,is_accepted")] Post post)
        {
            if (id != post.post_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.post_id))
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
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .FirstOrDefaultAsync(m => m.post_id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Post.FindAsync(id);
            _context.Post.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.post_id == id);
        }
    }
}
