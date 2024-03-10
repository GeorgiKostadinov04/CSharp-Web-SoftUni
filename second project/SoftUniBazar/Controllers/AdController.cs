using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftUniBazar.Data;
using SoftUniBazar.Models;
using System.Security.Claims;
using System.Xml.Linq;

namespace SoftUniBazar.Controllers
{
    public class AdController : Controller
    {

        private readonly BazarDbContext data;

        public AdController(BazarDbContext context)
        {
            data = context;
        }
        public async Task<IActionResult> All()
        {
            var model = await data.Ads
                .AsNoTracking()
                .Select(a => new AdFormViewModel(a.Id, a.Name, a.ImageUrl,a.CreatedOn, a.Category.Name, a.Description, a.Price, a.Owner.UserName))
                .ToListAsync();

            return View(model);
        }

        public async Task<IActionResult> Cart()
        {
            var userId = GetUserId();


            var model = await data.AdBuyers
                .Where(ad=> ad.BuyerId == userId)
                .Select(a => new AdFormViewModel(a.Ad.Id, a.Ad.Name, a.Ad.ImageUrl, 
                a.Ad.CreatedOn, a.Ad.Category.Name, a.Ad.Description, a.Ad.Price, a.Ad.Owner.UserName))
                .ToListAsync();

            return View(model);
        }

        public async Task<IActionResult> AddToCart(int id)
        {
            var adToAdd = await data
                .Ads.FindAsync(id);

            if(adToAdd == null)
            {
                return BadRequest();
            }

            string currentUserId = GetUserId();

            var entry = new AdBuyer()
            {
                AdId = adToAdd.Id,
                BuyerId = currentUserId,
            };

            if(await data.AdBuyers.ContainsAsync(entry))
            {
                return RedirectToAction("Cart", "Ad");
            }

            await data.AdBuyers.AddAsync(entry);
            await data.SaveChangesAsync();

            return RedirectToAction("Cart", "Ad");
        }

        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var adId = id;
            var currentUserId = GetUserId();

            var adToDelete = await data.Ads.FindAsync(adId);

            if(adToDelete == null)
            {
                return BadRequest();
            }

            var entry = data.AdBuyers.FirstOrDefault(ab=> ab.BuyerId == currentUserId && ab.AdId == adId);

            data.AdBuyers.Remove(entry);
            await data.SaveChangesAsync();

            return RedirectToAction("All", "Ad");
        }

        [HttpGet]

        public async Task<IActionResult> Add()
        {
            AdFormModel model = new AdFormModel()
            {
                Categories = GetCategories(),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AdFormModel adModel)
        {
            if(!GetCategories().Any(e=> e.Id == adModel.CategoryId))
            {
                ModelState.AddModelError(nameof(adModel.CategoryId), "Category does not exist!");
            }

            if(!ModelState.IsValid)
            {
                return View(adModel);
            }

            string currentUserId = GetUserId();

            var adToAdd = new Ad()
            {
                Name = adModel.Name,
                Description = adModel.Description,
                CreatedOn = DateTime.Now,
                CategoryId = adModel.CategoryId,
                Price = adModel.Price,
                OwnerId = currentUserId,
                ImageUrl = adModel.ImageUrl,
            };

            await data.Ads.AddAsync(adToAdd);
            await data.SaveChangesAsync();

            return RedirectToAction("All", "Ad");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var adToEdit = await data.Ads.FindAsync(id);

            if(adToEdit == null) 
            {
                return BadRequest();
            }

            var currentUserId = GetUserId();

            if(currentUserId != adToEdit.OwnerId)
            {
                return Unauthorized();
            }

            AdFormModel adFormModel = new AdFormModel
            {
                Name = adToEdit.Name,
                Description = adToEdit.Description,
                Price = adToEdit.Price,
                CategoryId = adToEdit.CategoryId,
                Categories = GetCategories(),
                ImageUrl = adToEdit.ImageUrl
            };

            return View(adFormModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, AdFormModel adFormModel)
        {
            var adToEdit = await data.Ads.FindAsync(id);

            if (adToEdit == null)
            {
                return BadRequest();
            }

            var currentUserId = GetUserId();

            if (currentUserId != adToEdit.OwnerId)
            {
                return Unauthorized();
            }

            if (!GetCategories().Any(e => e.Id == adFormModel.CategoryId))
            {
                ModelState.AddModelError(nameof(adFormModel.CategoryId), "Category does not exist!");
            }

            adToEdit.Name = adFormModel.Name;
            adToEdit.Description = adFormModel.Description;
            adToEdit.Price = adFormModel.Price;
            adToEdit.ImageUrl = adFormModel.ImageUrl;
            adToEdit.CategoryId = adFormModel.CategoryId;

            await data.SaveChangesAsync();
            return RedirectToAction("All", "Ad");
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
