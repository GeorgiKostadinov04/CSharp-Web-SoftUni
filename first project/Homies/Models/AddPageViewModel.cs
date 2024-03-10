using System.ComponentModel.DataAnnotations;
using static Homies.Data.DataConstants.Constants;
namespace Homies.Models
{
    public class AddPageViewModel
    {
        [Required(ErrorMessage = RequireErrorMessage)]
        [StringLength(EventNameMaxLength, MinimumLength = EventNameMinLength,
            ErrorMessage = StringLengthErrorMessage)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = RequireErrorMessage)]
        [StringLength(EventDescriptionMaxLength, MinimumLength = EventDescriptionMinLength,
            ErrorMessage = StringLengthErrorMessage)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = RequireErrorMessage)]
        public string Start { get; set; } = string.Empty;

        [Required(ErrorMessage = RequireErrorMessage)]
        public string End { get; set; } = String.Empty;

        [Required(ErrorMessage = RequireErrorMessage)]
        public int TypeId { get; set; }

        public IEnumerable<TypeViewModel> Types { get; set; } = new List<TypeViewModel>();
    }
}
