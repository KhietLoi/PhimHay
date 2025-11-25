using System;
using System.ComponentModel.DataAnnotations;

namespace MovieWebsite.Models
{
    public class Favorite
    {
        [Key]
        [Required]
        public string FavoriteId { get; set; } = Guid.NewGuid().ToString();

        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; } = DateTime.Now;

        [Required]
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;

        [Required]
        public string MovieId { get; set; } = string.Empty;
        public Movie Movie { get; set; } = null!;
    }
}
