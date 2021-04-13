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
    public class Post_TimeController : Controller
    {
        private readonly TeleFarmingContext _context;

        public Post_TimeController(TeleFarmingContext context)
        {
            _context = context;
        }

        // GET: Post_Time
        public async Task<IActionResult> Index()
        {
            return View(await _context.Post_Time.ToListAsync());
        }

        // GET: Post_Time/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post_Time = await _context.Post_Time
                .FirstOrDefaultAsync(m => m.id == id);
            if (post_Time == null)
            {
                return NotFound();
            }

            return View(post_Time);
        }

        // GET: Post_Time/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Post_Time/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,post_id,time")] Post_Time post_Time)
        {
            if (ModelState.IsValid)
            {
                _context.Add(post_Time);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post_Time);
        }

        // GET: Post_Time/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post_Time = await _context.Post_Time.FindAsync(id);
            if (post_Time == null)
            {
                return NotFound();
            }
            return View(post_Time);
        }

        // POST: Post_Time/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,post_id,time")] Post_Time post_Time)
        {
            if (id != post_Time.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post_Time);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Post_TimeExists(post_Time.id))
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
            return View(post_Time);
        }

        // GET: Post_Time/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post_Time = await _context.Post_Time
                .FirstOrDefaultAsync(m => m.id == id);
            if (post_Time == null)
            {
                return NotFound();
            }

            return View(post_Time);
        }

        // POST: Post_Time/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post_Time = await _context.Post_Time.FindAsync(id);
            _context.Post_Time.Remove(post_Time);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Post_TimeExists(int id)
        {
            return _context.Post_Time.Any(e => e.id == id);
        }
    }
}
