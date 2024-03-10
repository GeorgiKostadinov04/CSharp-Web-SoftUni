using System.ComponentModel.DataAnnotations;
using SeminarHub.Models.Category;
using static SeminarHub.Data.DataConstants.Constants;
namespace SeminarHub.Models.Seminar
{
    public class SeminarFormModel
    {
        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(SeminarTopicMaxLength, MinimumLength = SeminarTopicMinLength
            , ErrorMessage = TopicErrorMessage)]
        public string Topic { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(SeminarLecturerMaxLength, MinimumLength = SeminarLecturerMinLength
            , ErrorMessage = LecturerErrorMessage)]
        public string Lecturer { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(SeminarDetailsMaxLength, MinimumLength = SeminarDetailsMinlength
            , ErrorMessage = DetailsErrorMessage)]
        public string Details { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredErrorMessage)]
        public string DateAndTime { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        [Range(SeminarDurationMinValue, SeminarDurationMaxValue,
            ErrorMessage = DurationErrorMessage)]
        public int? Duration { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        public int CategoryId { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
    }
}
