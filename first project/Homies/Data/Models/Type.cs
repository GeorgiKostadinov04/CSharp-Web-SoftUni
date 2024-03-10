using System.ComponentModel.DataAnnotations;
using System.Security.Policy;
using static Homies.Data.DataConstants.Constants;
namespace Homies.Data.Models
{
    public class Type
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(TypeNameMaxLength)]
        public string Name { get; set; } = string.Empty;

        public IList<Event> Events { get; set; } = new List<Event>();
    }
}
