namespace MovieWebsite.Models
{
    public class HomePageViewModel
    {
        public List<Movie> FeaturedMovies { get; set; } = null!;
        public List<Movie> Movies { get; set; } = new List<Movie>();
    }
}
