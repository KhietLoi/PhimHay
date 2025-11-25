namespace MovieWebsite.Models
{
    public class CommentAdminViewModel
    {
        public string CommentId { get; set; }
        public string CommentText { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string MovieId { get; set; }
        public string MovieTitle { get; set; }
    }

}
