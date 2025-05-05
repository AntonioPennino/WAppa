using System;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Item
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public bool IsComplete { get; set; }
    }
}