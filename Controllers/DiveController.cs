using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DiveLogg.Data;
using DiveLogg.Models;

namespace DiveLogg.Controllers
{
    public class DiveController : Controller
    {
        private readonly DiveLoggContext _context;

        public DiveController(DiveLoggContext context)
        {
            _context = context;
        }

        // GET: Dive
        public async Task<IActionResult> Index()
        {
            var diveLoggContext = _context.Dive.Include(d => d.DiveLeader);
            return View(await diveLoggContext.ToListAsync());
        }

        // GET: Dive/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dive = await _context.Dive
                .Include(d => d.DiveLeader)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dive == null)
            {
                return NotFound();
            }

            return View(dive);
        }

        // GET: Dive/Create
        public IActionResult Create()
        {
            ViewData["DiveLeaderId"] = new SelectList(_context.Person, "Id", "Name");
            return View();
        }

        // POST: Dive/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Depth,DiveTime,ExposureTime,NitrogenLoad,Latitude,Longitude,LocationName,Notes,DiveLeaderId")] Dive dive)
        {

            if (ModelState.IsValid)
            {
                _context.Add(dive);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DiveLeaderId"] = new SelectList(_context.Person, "Id", "Name", dive.DiveLeaderId);
            return View(dive);
        }

        // GET: Dive/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dive = await _context.Dive.FindAsync(id);
            if (dive == null)
            {
                return NotFound();
            }
            ViewData["DiveLeaderId"] = new SelectList(_context.Person, "Id", "Name", dive.DiveLeaderId);
            return View(dive);
        }

        // POST: Dive/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Depth,DiveTime,ExposureTime,NitrogenLoad,Latitude,Longitude,LocationName,Notes,DiveLeaderId")] Dive dive)
        {
            if (id != dive.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dive);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiveExists(dive.Id))
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
            ViewData["DiveLeaderId"] = new SelectList(_context.Person, "Id", "Name", dive.DiveLeaderId);
            return View(dive);
        }

        // GET: Dive/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dive = await _context.Dive
                .Include(d => d.DiveLeader)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dive == null)
            {
                return NotFound();
            }

            return View(dive);
        }

        // POST: Dive/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dive = await _context.Dive.FindAsync(id);
            if (dive != null)
            {
                _context.Dive.Remove(dive);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiveExists(int id)
        {
            return _context.Dive.Any(e => e.Id == id);
        }
    }
}
