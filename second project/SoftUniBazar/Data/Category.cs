using System.ComponentModel.DataAnnotations;
using System.Security.Policy;
using static SoftUniBazar.Data.DataConstants;
namespace SoftUniBazar.Data
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(CategoryNameMaxLength)]
        public string Name { get; set; } = string.Empty;

        public IList<Ad> Ads { get; set; } = new List<Ad>(); 
    }
}

