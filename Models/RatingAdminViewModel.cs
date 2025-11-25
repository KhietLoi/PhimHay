namespace MovieWebsite.Models
{
    public class RatingAdminViewModel
    {
        public string RatingId { get; set; }
        public int Stars { get; set; }
        public DateTime RateAt { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string MovieId { get; set; }
        public string MovieTitle { get; set; }
    }
}
