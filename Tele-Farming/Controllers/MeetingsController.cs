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
    public class MeetingsController : Controller
    {
        private readonly TeleFarmingContext _context;

        public MeetingsController(TeleFarmingContext context)
        {
            _context = context;
        }

        // GET: Meetings
        public async Task<IActionResult> Index()
        {
            var teleFarmingContext = _context.Meeting.Include(m => m.Post).Include(m => m.Specialist);
            return View(await teleFarmingContext.ToListAsync());
        }

        // GET: Meetings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meeting = await _context.Meeting
                .Include(m => m.Post)
                .Include(m => m.Specialist)
                .FirstOrDefaultAsync(m => m.meeting_id == id);
            if (meeting == null)
            {
                return NotFound();
            }

            return View(meeting);
        }

        // GET: Meetings/Create
        public IActionResult Create()
        {
            ViewData["post_id"] = new SelectList(_context.Post, "post_id", "post_id");
            ViewData["specialist_id"] = new SelectList(_context.Specialist, "specialist_id", "specialist_id");
            return View();
        }

        // POST: Meetings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("meeting_id,specialist_id,post_id,meeting_link,short_message,meeting_time,meeting_status")] Meeting meeting)
        {
            if (ModelState.IsValid)
            {
                _context.Add(meeting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["post_id"] = new SelectList(_context.Post, "post_id", "post_id", meeting.post_id);
            ViewData["specialist_id"] = new SelectList(_context.Specialist, "specialist_id", "specialist_id", meeting.specialist_id);
            return View(meeting);
        }

        // GET: Meetings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meeting = await _context.Meeting.FindAsync(id);
            if (meeting == null)
            {
                return NotFound();
            }
            ViewData["post_id"] = new SelectList(_context.Post, "post_id", "post_id", meeting.post_id);
            ViewData["specialist_id"] = new SelectList(_context.Specialist, "specialist_id", "specialist_id", meeting.specialist_id);
            return View(meeting);
        }

        // POST: Meetings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("meeting_id,specialist_id,post_id,meeting_link,short_message,meeting_time,meeting_status")] Meeting meeting)
        {
            if (id != meeting.meeting_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(meeting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MeetingExists(meeting.meeting_id))
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
            ViewData["post_id"] = new SelectList(_context.Post, "post_id", "post_id", meeting.post_id);
            ViewData["specialist_id"] = new SelectList(_context.Specialist, "specialist_id", "specialist_id", meeting.specialist_id);
            return View(meeting);
        }

        // GET: Meetings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meeting = await _context.Meeting
                .Include(m => m.Post)
                .Include(m => m.Specialist)
                .FirstOrDefaultAsync(m => m.meeting_id == id);
            if (meeting == null)
            {
                return NotFound();
            }

            return View(meeting);
        }

        // POST: Meetings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var meeting = await _context.Meeting.FindAsync(id);
            _context.Meeting.Remove(meeting);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MeetingExists(int id)
        {
            return _context.Meeting.Any(e => e.meeting_id == id);
        }
    }
}
