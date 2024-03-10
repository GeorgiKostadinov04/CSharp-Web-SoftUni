using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using static Homies.Data.DataConstants.Constants;
namespace Homies.Models
{
    public class AllPageViewModel
    {

        public AllPageViewModel(int id, string name, DateTime start, string type, string organiser)
        {
            Id = id;
            Name = name;
            Type = type;
            Organiser = organiser;
            Start = start.ToString(DataFormat);
        }
        public int Id { get; set; }

        
        public string Name { get; set; } = string.Empty;

        public string Start { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        
        public string Organiser { get; set; }

    }
}
