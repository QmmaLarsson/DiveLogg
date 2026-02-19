using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DiveLogg.Data;
using DiveLogg.Models;
using DiveLogg.ViewModels;

namespace DiveLogg.Controllers
{
    public class DiveController : Controller
    {
        private readonly DiveLoggContext _context;

        public DiveController(DiveLoggContext context)
        {
            _context = context;
        }

        // GET: Dive (hämtar även dykledare, dykare och dykskötare samt namnen på de personer som har rollerna)
        public async Task<IActionResult> Index()
        {
            var diveLoggContext = _context.Dive
                .Include(d => d.DiveLeader)
                .Include(d => d.DiveParticipants)
                    .ThenInclude(dp => dp.Person);

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
            var model = new DiveCreateViewModel();
            CreateDropdowns(model);
            return View(model);
        }

        // POST: Dive/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DiveCreateViewModel model)
        {
            //Kontrollera att dykare är vald
            if (!model.Participants[0].PersonId.HasValue || model.Participants[0].PersonId.Value == 0)
            {
                ModelState.AddModelError("Participants[0].PersonId", "Dykare måste väljas");
            }

            //Kontrollera att ingen person har flera roller
            var selectedPersons = new List<int> { model.Dive.DiveLeaderId };

            selectedPersons.AddRange(
                model.Participants
                     .Where(p => p.PersonId.HasValue)
                     .Select(p => p.PersonId!.Value)
            );

            //Om en person har mer än en roll visas ett felmeddelande
            if (selectedPersons.Count != selectedPersons.Distinct().Count())
            {
                ModelState.AddModelError("", "En person kan bara ha en roll per dyk.");
            }

            if (ModelState.IsValid)
            {
                //Spara dyket
                _context.Add(model.Dive);
                await _context.SaveChangesAsync();

                //Spara deltagare
                foreach (var participant in model.Participants)
                {
                    if (participant.PersonId.HasValue)
                    {
                        _context.DiveParticipant.Add(new DiveParticipant
                        {
                            DiveId = model.Dive.Id,
                            PersonId = participant.PersonId.Value,
                            RoleId = participant.RoleId
                        });
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            CreateDropdowns(model);

            return View(model);
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

        //Fyller dropdown-menyerna med personer
        private void CreateDropdowns(DiveCreateViewModel model)
        {
            //Hämtar alla personer med rollen dykledare
            model.DiveLeaders = new SelectList(
                _context.PersonRole
                    .Where(pr => pr.RoleId == 2)
                    .Select(pr => pr.Person),
                "Id", "Name");

            //Hämtar alla personer med rollen dykare
            model.Divers = new SelectList(
                _context.PersonRole
                    .Where(pr => pr.RoleId == 1)
                    .Select(pr => pr.Person),
                "Id", "Name");

            //Hämtar alla personer med rollen dykskötare
            model.DiveSupports = new SelectList(
                _context.PersonRole
                    .Where(pr => pr.RoleId == 3)
                    .Select(pr => pr.Person),
                "Id", "Name");
        }
    }
}
