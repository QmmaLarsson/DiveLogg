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
                .Include(d => d.Diver)
                .Include(d => d.DiveSupport);

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
                .Include(d => d.Diver)
                .Include(d => d.DiveSupport)
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

            //Sätter aktuell dag och tid
            var now = DateTime.Now;
            model.Dive.Date = new DateTime(
                now.Year,
                now.Month,
                now.Day,
                now.Hour,
                now.Minute,
                0);

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
            var dive = model.Dive;

            //Kontrollera obligatoriska fält
            if (dive.Date == default)
                ModelState.AddModelError("Dive.Date", "Datum måste anges");

            if (dive.Depth <= 0 || dive.Depth > 100 || !dive.Depth.HasValue)
                ModelState.AddModelError("Dive.Depth", "Djup måste vara mellan 0 och 100");

            if (dive.DiveTime <= 0 || !dive.DiveTime.HasValue)
                ModelState.AddModelError("Dive.DiveTime", "Dyktid måste anges");

            if (dive.ExposureTime <= 0 || !dive.ExposureTime.HasValue)
                ModelState.AddModelError("Dive.ExposureTime", "Expositionstid måste anges");

            if (string.IsNullOrWhiteSpace(dive.NitrogenLoad) || !System.Text.RegularExpressions.Regex.IsMatch(dive.NitrogenLoad, "^[A-Z]$"))
                ModelState.AddModelError("Dive.NitrogenLoad", "Kvävebelastning måste vara en bokstav A-Z");

            if (dive.Latitude < -90 || dive.Latitude > 90 || !dive.Latitude.HasValue)
                ModelState.AddModelError("Dive.Latitude", "Latitude måste vara mellan -90 och 90");

            if (dive.Longitude < -180 || dive.Longitude > 180 || !dive.Longitude.HasValue)
                ModelState.AddModelError("Dive.Longitude", "Longitude måste vara mellan -180 och 180");

            if (string.IsNullOrWhiteSpace(dive.LocationName))
                ModelState.AddModelError("Dive.LocationName", "Dykplats måste anges");

            if (!string.IsNullOrEmpty(dive.LocationName) && dive.LocationName.Length > 50)
                ModelState.AddModelError("Dive.LocationName", "Dykplats får max vara 50 tecken");

            if (!dive.DiveLeaderId.HasValue)
                ModelState.AddModelError("Dive.DiveLeaderId", "Dykledare måste anges");

            if (!dive.DiverId.HasValue)
                ModelState.AddModelError("Dive.DiverId", "Dykare måste anges");

            if (!string.IsNullOrEmpty(dive.Notes) && dive.Notes.Length > 200)
                ModelState.AddModelError("Dive.Notes", "Anteckningar får max vara 200 tecken");

            //Kontrollera så att en deltagare inte har flera roller vid samma dyk
            var selectedIds = new List<int>();
            if (dive.DiveLeaderId > 0) selectedIds.Add((int)dive.DiveLeaderId);
            if (dive.DiverId > 0) selectedIds.Add((int)dive.DiverId);
            if (dive.DiveSupportId.HasValue && dive.DiveSupportId.Value > 0)
                selectedIds.Add(dive.DiveSupportId.Value);

            if (selectedIds.Count != selectedIds.Distinct().Count())
            {
                ModelState.AddModelError(string.Empty, "En person kan inte ha flera roller i samma dyk");
            }

            //Kontrollera om ModelState är giltig
            if (!ModelState.IsValid)
            {
                CreateDropdowns(model);
                return View(model);
            }

            //Spara dyk
            _context.Add(dive);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Dive/Edit/5
        public async Task<IActionResult> Edit(int id)
        {

            //Hämtar in sparade dyket
            var dive = await _context.Dive
                .FirstOrDefaultAsync(d => d.Id == id);

            //Om dy inte hittas
            if (dive == null)
            {
                return NotFound();
            }

            var vm = new DiveEditViewModel
            {
                //Hämtar in värden från det sparade dyket till nya objektet vm(formuläret)
                Id = dive.Id,
                DiveDate = dive.Date,
                Depth = dive.Depth,
                DiveTime = dive.DiveTime,
                ExposureTime = dive.ExposureTime,
                NitrogenLoad = dive.NitrogenLoad,
                Latitude = dive.Latitude,
                Longitude = dive.Longitude,
                LocationName = dive.LocationName,
                Notes = dive.Notes,
                DiveLeaderId = dive.DiveLeaderId,
                DiverId = dive.DiverId,
                DiveSupportId = dive.DiveSupportId
            };

            //Kör funktionen PopulateDropdowns som fyller på dropdowns med korrekt värde
            PopulateDropdowns(vm);
            return View(vm);
        }


        // POST: Dive/Edit/5
        // Ändra i befintligt dyk och spara ändringar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DiveEditViewModel vm)
        {

            //Hämta in dyk
            var dive = await _context.Dive.FindAsync(vm.Id);

            if (dive == null)
            {
                return NotFound();
            }

            //Ändringar sparas till variabler i dyk 
            dive.Date = vm.DiveDate;
            dive.Depth = vm.Depth;
            dive.DiveTime = vm.DiveTime;
            dive.ExposureTime = vm.ExposureTime;
            dive.NitrogenLoad = vm.NitrogenLoad;
            dive.Latitude = vm.Latitude;
            dive.Longitude = vm.Longitude;
            dive.LocationName = vm.LocationName;
            dive.Notes = vm.Notes;
            dive.DiveLeaderId = vm.DiveLeaderId;
            dive.DiverId = vm.DiverId;
            dive.DiveSupportId = vm.DiveSupportId;

            // felhanteringar
            if (dive.Date == default)
                ModelState.AddModelError("Dive.Date", "Datum måste anges");

            if (dive.Depth <= 0 || dive.Depth > 100 || !dive.Depth.HasValue)
                ModelState.AddModelError("Dive.Depth", "Djup måste vara mellan 0 och 100");

            if (dive.DiveTime <= 0 || !dive.DiveTime.HasValue)
                ModelState.AddModelError("Dive.DiveTime", "Dyktid måste anges");

            if (dive.ExposureTime <= 0 || !dive.ExposureTime.HasValue)
                ModelState.AddModelError("Dive.ExposureTime", "Expositionstid måste anges");

            if (string.IsNullOrWhiteSpace(dive.NitrogenLoad) || !System.Text.RegularExpressions.Regex.IsMatch(dive.NitrogenLoad, "^[A-Z]$"))
                ModelState.AddModelError("Dive.NitrogenLoad", "Kvävebelastning måste vara en bokstav A-Z");

            if (dive.Latitude < -90 || dive.Latitude > 90 || !dive.Latitude.HasValue)
                ModelState.AddModelError("Dive.Latitude", "Latitude måste vara mellan -90 och 90");

            if (dive.Longitude < -180 || dive.Longitude > 180 || !dive.Longitude.HasValue)
                ModelState.AddModelError("Dive.Longitude", "Longitude måste vara mellan -180 och 180");

            if (string.IsNullOrWhiteSpace(dive.LocationName))
                ModelState.AddModelError("Dive.LocationName", "Dykplats måste anges");

            if (!string.IsNullOrEmpty(dive.LocationName) && dive.LocationName.Length > 50)
                ModelState.AddModelError("Dive.LocationName", "Dykplats får max vara 50 tecken");

            if (!dive.DiveLeaderId.HasValue)
                ModelState.AddModelError("Dive.DiveLeaderId", "Dykledare måste anges");

            if (!dive.DiverId.HasValue)
                ModelState.AddModelError("Dive.DiverId", "Dykare måste anges");

            if (!string.IsNullOrEmpty(dive.Notes) && dive.Notes.Length > 200)
                ModelState.AddModelError("Dive.Notes", "Anteckningar får max vara 200 tecken");

            //Kontrollera så att en deltagare inte har flera roller vid samma dyk
            var selectedIds = new List<int>();
            if (dive.DiveLeaderId > 0) selectedIds.Add((int)dive.DiveLeaderId);
            if (dive.DiverId > 0) selectedIds.Add((int)dive.DiverId);
            if (dive.DiveSupportId.HasValue && dive.DiveSupportId.Value > 0)
                selectedIds.Add(dive.DiveSupportId.Value);

            if (selectedIds.Count != selectedIds.Distinct().Count())
            {
                ModelState.AddModelError(string.Empty, "En person kan inte ha flera roller i samma dyk");
            }

            if (!ModelState.IsValid)
            {
                PopulateDropdowns(vm);
                return View(vm);
            }

            //Spara ändringar och återvänd till index för dyk
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
                .Include(d => d.Diver)
                .Include(d => d.DiveSupport)
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

        //Fyller dropdown-menyerna med personer
        private void CreateDropdowns(DiveCreateViewModel model)
        {
            model.DiveLeaders = new SelectList(
                _context.PersonRole
                    .Where(pr => pr.Role.Name == "DiveLeader")
                    .Select(pr => pr.Person),
                "Id", "Name");

            model.Divers = new SelectList(
                _context.PersonRole
                    .Where(pr => pr.Role.Name == "Diver")
                    .Select(pr => pr.Person),
                "Id", "Name");

            model.DiveSupports = new SelectList(
                _context.PersonRole
                    .Where(pr => pr.Role.Name == "DiveSupport")
                    .Select(pr => pr.Person),
                "Id", "Name");
        }

        private void PopulateDropdowns(DiveEditViewModel vm)
        {
            vm.DiveLeaders = new SelectList(
                _context.Person
                    .Where(p => p.PersonRoles.Any(pr => pr.Role.Name == "DiveLeader")),
                "Id",
                "Name",
                vm.DiveLeaderId
            );

            vm.Divers = new SelectList(
                _context.Person
                    .Where(p => p.PersonRoles.Any(pr => pr.Role.Name == "Diver")),
                "Id",
                "Name",
                vm.DiverId
            );

            vm.DiveSupports = new SelectList(
                _context.Person
                    .Where(p => p.PersonRoles.Any(pr => pr.Role.Name == "DiveSupport")),
                "Id",
                "Name",
                vm.DiveSupportId
            );
        }

    }
}
