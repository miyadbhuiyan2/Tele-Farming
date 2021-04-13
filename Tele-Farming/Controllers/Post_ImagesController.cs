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
    public class Post_ImagesController : Controller
    {
        private readonly TeleFarmingContext _context;

        public Post_ImagesController(TeleFarmingContext context)
        {
            _context = context;
        }

        // GET: Post_Images
        public async Task<IActionResult> Index()
        {
            return View(await _context.Post_Images.ToListAsync());
        }

        // GET: Post_Images/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post_Images = await _context.Post_Images
                .FirstOrDefaultAsync(m => m.id == id);
            if (post_Images == null)
            {
                return NotFound();
            }

            return View(post_Images);
        }

        // GET: Post_Images/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Post_Images/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,post_id,image_path")] Post_Images post_Images)
        {
            if (ModelState.IsValid)
            {
                _context.Add(post_Images);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post_Images);
        }

        // GET: Post_Images/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post_Images = await _context.Post_Images.FindAsync(id);
            if (post_Images == null)
            {
                return NotFound();
            }
            return View(post_Images);
        }

        // POST: Post_Images/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,post_id,image_path")] Post_Images post_Images)
        {
            if (id != post_Images.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post_Images);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Post_ImagesExists(post_Images.id))
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
            return View(post_Images);
        }

        // GET: Post_Images/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post_Images = await _context.Post_Images
                .FirstOrDefaultAsync(m => m.id == id);
            if (post_Images == null)
            {
                return NotFound();
            }

            return View(post_Images);
        }

        // POST: Post_Images/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post_Images = await _context.Post_Images.FindAsync(id);
            _context.Post_Images.Remove(post_Images);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Post_ImagesExists(int id)
        {
            return _context.Post_Images.Any(e => e.id == id);
        }
    }
}
