using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MovieWebsite.Models
{
    public class Category
    {
        [Key]
        [Required]
        public string CategoryId { get;set; } = string.Empty;
        
        [Required(ErrorMessage = "Tên thể loại là bắt buộc.")]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        public ICollection<MovieCategory> MovieCategories { get; set; } = new List<MovieCategory>();
    }
}
