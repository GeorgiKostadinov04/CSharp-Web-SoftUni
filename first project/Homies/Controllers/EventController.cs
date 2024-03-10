using Homies.Data;
using Homies.Data.Models;
using Homies.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;
using System.Security.Claims;
using static Homies.Data.DataConstants.Constants;
namespace Homies.Controllers
{
    [Authorize]
    public class EventController : Controller
    {
        private readonly HomiesDbContext data;

        public EventController(HomiesDbContext context)
        {
            data = context;
        }


        public async Task<IActionResult> All()
        {
            var model = await data.Events
                .AsNoTracking()
                .Select(e => new AllPageViewModel(e.Id, e.Name, e.Start, e.Type.Name, e.Organiser.UserName))
                .ToListAsync();

            return View(model);

        }

        public async Task<IActionResult> Joined()
        {
            var userId = GetUserId();

            var model = await data.EventParticipants
                .AsNoTracking()
                .Where(ep => ep.HelperId == userId)
                .Select(ep => new AllPageViewModel(ep.Event.Id, ep.Event.Name, ep.Event.Start, ep.Event.Type.Name, ep.Event.Organiser.UserName))
                .ToListAsync();

            return View(model);
        }

        public async Task<IActionResult> Join(int id)
        {
            var e = await data.Events
                .Where(e => e.Id == id)
                .Include(e => e.EventParticipants)
                .FirstOrDefaultAsync();

            if(e == null)
            {
                return BadRequest();
            }

            var userId = GetUserId();

            if(!e.EventParticipants.Any(p=>p.HelperId == userId))
            {
                e.EventParticipants.Add(new Data.Models.EventParticipant()
                {
                    EventId = e.Id,
                    HelperId = userId
                });

                await data.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Joined));
        }

        public async Task<IActionResult> Leave(int id)
        {
            var e = await data.Events
                .Where(e => e.Id == id)
                .Include(e => e.EventParticipants)
                .FirstOrDefaultAsync();

            if (e == null)
            {
                return BadRequest();
            }

            string userId = GetUserId();

            var ep = e.EventParticipants
                .FirstOrDefault(ep => ep.HelperId == userId);

            if (ep == null)
            {
                return BadRequest();
            }

            e.EventParticipants.Remove(ep);

            await data.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]

        public async Task<IActionResult> Add()
        {
            var model = new AddPageViewModel();
            model.Types = await GetTypes();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddPageViewModel model)
        {
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;

            if (!DateTime.TryParseExact(
                model.Start,
                DataFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out start))
            {
                ModelState
                    .AddModelError(nameof(model.Start), $"Invalid date! Format must be: {DataFormat}");
            }

            if (!DateTime.TryParseExact(
                model.End,
                DataFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out end))
            {
                ModelState
                    .AddModelError(nameof(model.End), $"Invalid date! Format must be: {DataFormat}");
            }

            if(!ModelState.IsValid) 
            {
                model.Types = await GetTypes();

                return View(model);
            }

            var entity = new Event()
            {
                CreatedOn = DateTime.Now,
                Description = model.Description,
                Name = model.Name,
                OrganiserId = GetUserId(),
                TypeId = model.TypeId,
                Start = start,
                End = end
            };

            await data.Events.AddAsync(entity);
            await data.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var e = await data.Events.FindAsync(id);

            if(e == null)
            {
                return BadRequest();
            }

            var userId = GetUserId();

            if(e.OrganiserId != userId)
            {
                return Unauthorized();
            }

            var model = new AddPageViewModel()
            {
                Name = e.Name,
                Description = e.Description,
                Start = e.Start.ToString(DataFormat),
                End = e.End.ToString(DataFormat),
                TypeId = e.TypeId
            };

            model.Types = await GetTypes();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, AddPageViewModel model)
        {
            var e = await data.Events
                .FindAsync(id);

            if( e == null)
            {
                return BadRequest();
            }

            var userId = GetUserId();

            if(e.OrganiserId != userId)
            {
                return Unauthorized();
            }

            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;

            if (DateTime.TryParseExact(
                model.Start,
                DataFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, out start))
            {
                ModelState.AddModelError(nameof(model.Start), $"Invalid date! Format must be {DataFormat}");
            }

            if (DateTime.TryParseExact(
                model.End,
                DataFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, out end))
            {
                ModelState.AddModelError(nameof(model.End), $"Invalid date! Format must be {DataFormat}");
            }

            if(!ModelState.IsValid) 
            {
                model.Types = await GetTypes();
                return View(model);
            }

            e.Start = start; 
            e.End = end;
            e.Description = model.Description;
            e.Name = model.Name;
            e.TypeId = model.TypeId;


            await data.SaveChangesAsync();
            return RedirectToAction(nameof(All));
        }

        private async Task<IEnumerable<TypeViewModel>> GetTypes()
        {
            return await data.Types
                .AsNoTracking()
                .Select(t => new TypeViewModel
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToListAsync();
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value?? string.Empty;
        }
    }
}
