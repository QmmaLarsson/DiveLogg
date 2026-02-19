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
    //Controlleer för hantering av Person objekt
    public class PersonController : Controller
    {
        //DbContext kommunicerar med databasen
        private readonly DiveLoggContext _context;

        public PersonController(DiveLoggContext context)
        {
            _context = context;
        }

        // GET: Person
        //Hämtar alla personer fån databasen och visar på skärmen
        public async Task<IActionResult> Index()
        {
            //Include hämtar relaterad gruppdata
            var persons = _context.Person.Include(p => p.Group);

            //Konverterar till lista och visar på skärm
            return View(await persons.ToListAsync());
        }

        // GET: Person/Details/5
        //Visar detaljer för en specifik person
        public async Task<IActionResult> Details(int? id)
        {

            //Kontroll om id finns
            if (id == null)
            {
                return NotFound();
            }

            //Hämtar person + grupp + roller
            var person = await _context.Person
                .Include(p => p.Group)
                .Include(p => p.PersonRoles)
                .ThenInclude(pr => pr.Role)
                .FirstOrDefaultAsync(m => m.Id == id);

            //Om person inte finns
            if (person == null)
            {
                return NotFound();
            }

            //Om allt går bra, returnera person
            return View(person);
        }

        // GET: Person/Create
        //Visar formulär för att skapa ny person
        public IActionResult Create()
        {

            //Skapar viewModel och fyller med alla roller
            var vm = new PersonCreateViewModel
            {
                AvailableRoles = _context.Role.ToList()
            };

            //Skapar dropdown för groups
            ViewData["GroupId"] = new SelectList(_context.Group, "Id", "Name");
            return View(vm);
        }

        // POST: Person/Create
        // Tar emot formulärdata och sparar som ny person i databasen
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PersonCreateViewModel vm)
        {
            //Om formulärdata är felaktig
            if (!ModelState.IsValid)
            {
                // fyller på rollerna igen om något gick fel
                vm.AvailableRoles = _context.Role.ToList();

                //Skapar ny dropdown för views
                ViewData["GroupId"] = new SelectList(_context.Group, "Id", "Name", vm.GroupId);
                return View(vm);
            }

            // Skapa ny person
            var person = new Person
            {
                Name = vm.Name,
                GroupId = vm.GroupId,

                //timestamp för datum sätts automatiskt
                CreatedAt = DateTime.UtcNow
            };

            //Lägger till i databas
            _context.Person.Add(person);

            //Sparar
            await _context.SaveChangesAsync();

            // Lägg till valda roller
            if (vm.SelectedRoleIds != null && vm.SelectedRoleIds.Any())
            {
                foreach (var roleId in vm.SelectedRoleIds)
                {
                    _context.PersonRole.Add(new PersonRole
                    {
                        PersonId = person.Id,
                        RoleId = roleId
                    });
                }
                await _context.SaveChangesAsync();
            }

            //Går tillbaka till listan
            return RedirectToAction(nameof(Index));
        }


        // GET: Person/Edit/5
        //Formulär för att redigera person
        public async Task<IActionResult> Edit(int? id)
        {
            //Kontroll om person existerar
            if (id == null)
            {
                return NotFound();
            }

            //Hämtar person med roller
            var person = await _context.Person
            .Include(p => p.PersonRoles)
            .FirstOrDefaultAsync(p => p.Id == id);

            if (person == null)
            {
                return NotFound();
            }

            //Skapar viewModel och fyller med data från person
            var vm = new PersonEditViewModel
            {
                Id = person.Id,
                Name = person.Name,
                GroupId = person.GroupId,
                CreatedAt = person.CreatedAt,
                AvailableRoles = _context.Role.ToList(),
                SelectedRoleIds = person.PersonRoles.Select(pr => pr.RoleId).ToList()
            };

            //dropdown för grupper
            ViewData["GroupId"] = new SelectList(_context.Group, "Id", "Name", person.GroupId);

            return View(vm);
        }

        // POST: Person/Edit/5
        // ppdatera person i databasen      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PersonEditViewModel vm)
        {
            
            if (!ModelState.IsValid)
            {
                vm.AvailableRoles = _context.Role.ToList();
                ViewData["GroupId"] = new SelectList(_context.Group, "Id", "Name", vm.GroupId);

                return View(vm);
            }

            //Hämtar person från databasen
            var person = await _context.Person
                .Include(p => p.PersonRoles)
                .FirstOrDefaultAsync(p => p.Id == vm.Id);

            if (person == null)
            {
                return NotFound();
            }

            //Uppdaterar data för namn/ grupp
            person.Name = vm.Name;
            person.GroupId = vm.GroupId;

            //Uppdaterar roller
            var existingRoleIds = person.PersonRoles.Select(pr => pr.RoleId).ToList();

            //Tar bort roller som inte längre är valda
            var rolesToRemove = person.PersonRoles.Where(pr => !vm.SelectedRoleIds.Contains(pr.RoleId)).ToList();
            _context.PersonRole.RemoveRange(rolesToRemove);

            //Lägger till nya roller
            var rolesToAdd = vm.SelectedRoleIds.Where(rid => !existingRoleIds.Contains(rid)).ToList();

            //Uppdaterar och registrerar valda roller till person
            foreach (var roleId in rolesToAdd)
            {
                _context.PersonRole.Add(new PersonRole
                {
                    PersonId = person.Id,
                    RoleId = roleId
                });
            }

            //Sparar ändringar
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Person/Delete/5
        //Sida för att bekräfta en delete
        public async Task<IActionResult> Delete(int? id)
        {

            //Om id inte hittas
            if (id == null)
            {
                return NotFound();
            }

            
            var person = await _context.Person
                .Include(p => p.Group)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: Person/Delete/5
        //Ta bort person från databasen
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //person får värdet av den valda personen utifrån Id
            var person = await _context.Person.FindAsync(id);

            //Om person hittas, radera person
            if (person != null)
            {
                _context.Person.Remove(person);
                await _context.SaveChangesAsync();
            }


            return RedirectToAction(nameof(Index));
        }

        private bool PersonExists(int id)
        {
            return _context.Person.Any(e => e.Id == id);
        }
    }
}
