using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieWebsite.Models
{
    public class MovieCategory
    {
        [Required]
        public string MovieId { get; set; } = string.Empty;
        
        //public Movie Movie { get; set; } = null!;

        [Required]
        public string CategoryId { get; set; } = string.Empty;

        //public Category Category { get; set; } = null!;

        // Navigation properties
        [ForeignKey("MovieId")]
        public virtual Movie Movie { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}
