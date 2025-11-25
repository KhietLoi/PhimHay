using System;
using System.ComponentModel.DataAnnotations;

namespace MovieWebsite.Models
{
    public class WatchHistory
    {
        [Key]
        [Required]
        public string WatchId { get; set; } = Guid.NewGuid().ToString();

        public DateTime WatchedAt { get; set; } = DateTime.Now;

        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;

        public string MovieId { get; set; } = string.Empty;
        public Movie Movie { get; set; } = null!;
    }
}
