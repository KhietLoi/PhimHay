namespace MovieWebsite.Models
{
    public class MovieListViewModel
    {
        public List<Movie> Movies {  get; set; } = new List<Movie>();
        public int CurrentPage { get; set; } // Trang hiện tại
        public int TotalPages { get; set; } // Tổng số trang
        public int TotalMovies { get; set; } // Tổng số bộ phim
        public int PageSize { get; set; } // Số lượng bộ phim mỗi trang
    }
}
