using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeminarHub.Data;
using SeminarHub.Data.Models;
using SeminarHub.Models.Category;
using SeminarHub.Models.Delete;
using SeminarHub.Models.Details;
using SeminarHub.Models.Seminar;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Xml.Linq;
using static SeminarHub.Data.DataConstants.Constants;

namespace SeminarHub.Controllers
{
    [Authorize]
    public class SeminarController : Controller
    {
        private readonly SeminarHubDbContext data;

        public SeminarController(SeminarHubDbContext context)
        {
            data = context;
        }
        public async Task<IActionResult> All()
        {
            var model = await data.Seminars
                .AsNoTracking()
                .Select(s => new SeminarViewModel()
                {
                    Id = s.Id,
                    Topic = s.Topic,
                    Lecturer = s.Lecturer,
                    DateAndTime = s.DateAndTime.ToString(DateFormat),
                    Organizer = s.Organizer.UserName,
                    Category = s.Category.Name
                })
                .ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> Joined()
        {
            var currUserId = GetUserId();

            var semParticipants = await data.SeminarsParticipants
                .Where(sp=> sp.ParticipantId == currUserId)
                .Select(sp => new SeminarViewModel()
                {
                    Id = sp.Seminar.Id,
                    Topic = sp.Seminar.Topic,
                    Lecturer = sp.Seminar.Lecturer,
                    Category = sp.Seminar.Category.Name,
                    Organizer = sp.Seminar.Organizer.UserName,
                    DateAndTime = sp.Seminar.DateAndTime.ToString(DateFormat),
                })
                .ToListAsync();
            return View(semParticipants);
        }

        public async Task<IActionResult> Join(int id)
        {
            var seminar = await data.Seminars.FindAsync(id);

            if(seminar == null)
            {
                return BadRequest();
            }

            var currUserId = GetUserId();

            var newSeminarParticipant = new SeminarParticipant()
            {
                SeminarId = seminar.Id,
                ParticipantId = currUserId
            };

            if(await data.SeminarsParticipants.ContainsAsync(newSeminarParticipant))
            {
                return RedirectToAction(nameof(Joined), "Seminar");
            }

            await data.SeminarsParticipants.AddRangeAsync(newSeminarParticipant);
            await data.SaveChangesAsync();

            return RedirectToAction(nameof(Joined), "Seminar");
        }

        public async Task<IActionResult> Leave(int id)
        {
            var seminarToRemove = await data.Seminars.FindAsync(id);

            if(seminarToRemove == null)
            {
                return BadRequest();
            }
            
            var currUserId = GetUserId();

            var entry = await data.SeminarsParticipants
                .FirstAsync(sp=> sp.SeminarId == id && sp.ParticipantId == currUserId);

            data.SeminarsParticipants.Remove(entry);
            await data.SaveChangesAsync();

            return RedirectToAction(nameof(All), "Seminar");
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new SeminarFormModel()
            {
                Categories = GetCategories()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(SeminarFormModel model)
        {
            if (!GetCategories().Any(e => e.Id == model.CategoryId))
            {
                ModelState.AddModelError(nameof(model.CategoryId), "Category does not exist!");
            }

            DateTime dateAndTime = DateTime.Now;

            if (!DateTime.TryParseExact(
                model.DateAndTime,
                DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dateAndTime))
            {
                ModelState
                    .AddModelError(nameof(model.DateAndTime), $"Invalid date! Format must be: {DateFormat}");
            }

            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var currUserId = GetUserId();

            var seminarToAdd = new Seminar()
            {
                Topic = model.Topic,
                Lecturer = model.Lecturer,
                Details = model.Details,
                CategoryId = model.CategoryId,
                DateAndTime = dateAndTime,
                Duration = model.Duration,
                OrganizerId = currUserId
            };

            await data.Seminars.AddAsync(seminarToAdd);
            await data.SaveChangesAsync();

            return RedirectToAction(nameof(All), "Seminar");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var seminarToEdit = await data.Seminars.FindAsync(id);

            if (seminarToEdit == null)
            {
                return BadRequest();
            }

            var currUserId = GetUserId();

            if (seminarToEdit.OrganizerId != currUserId)
            {
                return Unauthorized();
            }

            var model = new SeminarFormModel()
            {
                Topic = seminarToEdit.Topic,
                Lecturer = seminarToEdit.Lecturer,
                Details = seminarToEdit.Details,
                DateAndTime = seminarToEdit.DateAndTime.ToString(DateFormat),
                Duration = seminarToEdit.Duration,
                CategoryId = seminarToEdit.CategoryId,
                Categories = GetCategories()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, SeminarFormModel model)
        {
            var seminarToEdit = await data.Seminars.FindAsync(id);

            if (seminarToEdit == null)
            {
                return BadRequest();
            }

            var currUserId = GetUserId();

            if (seminarToEdit.OrganizerId != currUserId)
            {
                return Unauthorized();
            }

            DateTime dateAndTime = DateTime.Now;

            if (!DateTime.TryParseExact(
                model.DateAndTime,
                DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dateAndTime))
            {
                ModelState
                    .AddModelError(nameof(model.DateAndTime), $"Invalid date! Format must be: {DateFormat}");
            }

            if(!ModelState.IsValid)
            {
                model.Categories = GetCategories();
                return View(model);
            }

            seminarToEdit.Topic = model.Topic;
            seminarToEdit.Lecturer = model.Lecturer;
            seminarToEdit.Details = model.Details;
            seminarToEdit.DateAndTime = dateAndTime;
            seminarToEdit.Duration = model.Duration;
            seminarToEdit.CategoryId = model.CategoryId;

            await data.SaveChangesAsync();
            return RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> Details(int id)
        {
            var model = await data.Seminars
                .Where(s => s.Id == id)
                .AsNoTracking()
                .Select(s => new DetailsViewModel()
                {
                    Id = s.Id,
                    Topic = s.Topic,
                    Lecturer = s.Lecturer,
                    Details = s.Details,
                    Duration = s.Duration,
                    DateAndTime = s.DateAndTime.ToString(DateFormat),
                    Organizer = s.Organizer.UserName,
                    Category = s.Category.Name
                })
                .FirstOrDefaultAsync();

            if(model == null)
            {
                return BadRequest();
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var model = await data.Seminars
                .Where(s=> s.Id == id)
                .AsNoTracking()
                .Select(s => new DeleteViewModel()
                {
                    Topic = s.Topic,
                    DateAndTime= s.DateAndTime
                })
                .FirstOrDefaultAsync();

            if(model == null)
            {
                return BadRequest();
            }

            return View(model);
        }

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seminarToDelete = await data.Seminars.FindAsync(id);

            if(seminarToDelete == null)
            {
                return BadRequest();
            }

            data.Seminars.Remove(seminarToDelete);
            await data.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        private string GetUserId()
           => User.FindFirstValue(ClaimTypes.NameIdentifier);

        private IEnumerable<CategoryViewModel> GetCategories()
            => data
                .Categories
                .Select(t => new CategoryViewModel()
                {
                    Id = t.Id,
                    Name = t.Name
                });
    }
}
