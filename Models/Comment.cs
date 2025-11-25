using System.ComponentModel.DataAnnotations;

namespace MovieWebsite.Models
{
    public class Comment
    {
        [Key]
        [Required]
        public string CommentId {  get; set; } = Guid.NewGuid().ToString();
        public string CommentText { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;

        public string MovieId { get; set; } = string.Empty;
        public Movie Movie { get; set; }=null!;

    }
}
