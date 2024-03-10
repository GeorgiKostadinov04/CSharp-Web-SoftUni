using System;
using System.ComponentModel.DataAnnotations;
using static SoftUniBazar.Data.DataConstants;

namespace SoftUniBazar.Models
{
    public class AdFormViewModel
    {
        public AdFormViewModel(int id, string name,string imageUrl,DateTime createdOn, string category, string description, decimal price, string owner)
        {
            Id = id;
            Name = name;
            ImageUrl = imageUrl;
            CreatedOn = createdOn.ToString(DateFormat);
            Category = category;
            Description = description;
            Price = price;
            Owner = owner;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }

        public string CreatedOn { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string Owner {  get; set; }
    }
}