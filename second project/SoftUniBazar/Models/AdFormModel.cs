using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static SoftUniBazar.Data.DataConstants;
namespace SoftUniBazar.Models
{
    public class AdFormModel
    {
        [Required(ErrorMessage = "Ad name must be between 5 and 25 characters.")]
        [StringLength(AdNameMaxLength, MinimumLength = AdNameMinLength)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Ad description must be between 15 and 250 characters.")]
        [StringLength(AdDescriptionMaxLength, MinimumLength = AdDescriptionMinLength)]
        public string Description { get; set; } = null!;

        [Required]
        [DisplayName("Image Url")]
        public string ImageUrl { get; set; } = null!;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
    }
}